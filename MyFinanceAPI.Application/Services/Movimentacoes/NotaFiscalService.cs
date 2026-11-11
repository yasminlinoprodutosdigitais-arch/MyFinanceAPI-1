using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Card #76, fase 6: lê a NFC-e do Paraná pelo link do QR Code e devolve os itens, já com a categoria das regras. Só consulta
/// o endereço da Fazenda: o link vem de fora, então nunca é usado como veio (sem redirecionamento e com host fixo).
/// </summary>
public partial class NotaFiscalService(
    IRegraClassificacaoRepository regras, INotaFiscalRepository notas, IExtratoBancarioItemRepository itens,
    IPessoaMovimentacaoRepository pessoas, IUnitOfWork unitOfWork) : INotaFiscalService
{
    private const string Host = "www.fazenda.pr.gov.br";
    private const string Caminho = "/nfce/qrcode";
    private static readonly CultureInfo PtBr = new("pt-BR");
    private static readonly HttpClient Http = CriarCliente();

    private static HttpClient CriarCliente()
    {
        var cliente = new HttpClient(new SocketsHttpHandler { AllowAutoRedirect = false }) { Timeout = TimeSpan.FromSeconds(20) };
        cliente.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        return cliente;
    }

    public async Task<NotaFiscalDTO> LerAsync(int userId, string? link)
    {
        var endereco = EnderecoDaConsulta(link);
        string html;
        try
        {
            using var resposta = await Http.GetAsync(endereco);
            if (!resposta.IsSuccessStatusCode)
                throw new RegraNegocioException("A Fazenda não devolveu esta nota. Tente de novo em instantes.");
            html = await resposta.Content.ReadAsStringAsync();
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            throw new RegraNegocioException("Não consegui falar com a Fazenda agora. Tente de novo em instantes.");
        }

        var nota = Interpretar(html);
        nota.Link = endereco.ToString();
        var ativas = await regras.ListarAtivasAsync(userId);
        foreach (var item in nota.Itens)
            item.CategoriaId = ClassificadorRegras.Encontrar(ativas, item.Nome, null, null)?.CategoriaId;
        return nota;
    }

    public async Task<NotaFiscalDTO?> ObterAsync(int userId, int itemId)
    {
        var nota = await notas.ObterPorItemAsync(itemId, userId);
        return nota == null ? null : new NotaFiscalDTO
        {
            Estabelecimento = nota.Estabelecimento,
            Total = nota.Total,
            Link = nota.Link,
            Itens = nota.Itens.Select(i => new NotaFiscalItemDTO
            {
                Nome = i.Nome, Quantidade = i.Quantidade, Valor = i.Valor, CategoriaId = i.CategoriaId, CategoriaNome = i.Categoria?.Nome
            }).ToList()
        };
    }

    public Task RemoverAsync(int userId, int itemId) => notas.RemoverPorItemAsync(itemId, userId);

    public Task<NotaSalvaDTO> SalvarAsync(int userId, int itemId, NotaFiscalDTO nota) =>
        unitOfWork.ExecuteAsync(async () =>
        {
            var item = await itens.GetByIdAsync(itemId) ?? throw new KeyNotFoundException("Item de extrato não encontrado.");
            if (item.UsuarioId != userId)
                throw new UnauthorizedAccessException("Usuário não autorizado para alterar este item.");
            if (item.TipoLancamento != TipoLancamentoExtrato.Saida)
                throw new RegraNegocioException("Só uma saída pode ter nota.");
            if (nota.Itens.Count == 0)
                throw new RegraNegocioException("A nota não tem itens.");
            if (await itens.TemCategoriaForaDoUsuarioAsync(nota.Itens.Where(i => i.CategoriaId is > 0).Select(i => i.CategoriaId!.Value).ToList(), userId))
                throw new RegraNegocioException("Categoria não encontrada.");

            await notas.SubstituirAsync(new NotaFiscal
            {
                UsuarioId = userId,
                ExtratoBancarioItemId = itemId,
                Estabelecimento = nota.Estabelecimento,
                Total = nota.Total,
                Link = string.IsNullOrWhiteSpace(nota.Link) ? null : EnderecoDaConsulta(nota.Link).ToString(),
                Itens = nota.Itens.Select((i, n) => new NotaFiscalItem
                {
                    UsuarioId = userId, Nome = i.Nome, Quantidade = i.Quantidade, Valor = i.Valor, CategoriaId = i.CategoriaId is > 0 ? i.CategoriaId : null, Ordem = n
                }).ToList()
            });

            return await ClassificarAsync(item, nota, userId);
        });

    private async Task<NotaSalvaDTO> ClassificarAsync(ExtratoBancarioItem item, NotaFiscalDTO nota, int userId)
    {
        if (nota.Total != item.Valor)
            return new NotaSalvaDTO { Mensagem = "Nota salva. O total dela é diferente do lançamento, então a categoria não foi alterada." };
        if (item.Dividido)
            return new NotaSalvaDTO { Mensagem = "Nota salva. O lançamento já estava dividido e a divisão foi mantida." };

        var grupos = nota.Itens.GroupBy(i => i.CategoriaId is > 0 ? i.CategoriaId : null).Select(g => (Categoria: g.Key, Valor: g.Sum(i => i.Valor))).ToList();
        if (grupos.Count == 1)
        {
            if (grupos[0].Categoria == null)
                return new NotaSalvaDTO { Mensagem = "Nota salva, sem categoria nos itens." };
            item.CategoriaId = grupos[0].Categoria;
            await itens.UpdateAsync(item);
            return new NotaSalvaDTO { Classificado = true, Mensagem = "Nota salva e lançamento classificado." };
        }

        // Quem entra nos acertos continua devendo: a divisão não pode apagar a pessoa do lançamento.
        var pessoa = item.PessoaMovimentacaoId is int id ? await pessoas.ObterPorIdAsync(id, userId) : null;
        var pessoaDasPartes = pessoa is { EhTitular: false } ? pessoa.Id : (int?)null;
        await itens.SubstituirPartesAsync(item.Id, grupos.Select((g, n) => new ExtratoItemParte
        {
            ExtratoBancarioItemId = item.Id, UsuarioId = userId, Valor = g.Valor, CategoriaId = g.Categoria, PessoaMovimentacaoId = pessoaDasPartes, Ordem = n
        }).ToList());
        return new NotaSalvaDTO { Classificado = true, Mensagem = $"Nota salva e lançamento dividido em {grupos.Count} partes." };
    }

    /// <summary>O endereço fixo da Fazenda com a consulta do link; recusa qualquer outro destino.</summary>
    public static Uri EnderecoDaConsulta(string? link)
    {
        if (!Uri.TryCreate(link?.Trim(), UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https")
            || !uri.Host.Equals(Host, StringComparison.OrdinalIgnoreCase)
            || !uri.AbsolutePath.TrimEnd('/').Equals(Caminho, StringComparison.OrdinalIgnoreCase)
            || !uri.Query.StartsWith("?p=", StringComparison.OrdinalIgnoreCase))
            throw new RegraNegocioException("Este link não é de uma nota do Paraná. Por enquanto só leio NFC-e do PR.");
        return new Uri($"https://{Host}{Caminho}{uri.Query}");
    }

    /// <summary>Lê a página de consulta (puro, para testar).</summary>
    public static NotaFiscalDTO Interpretar(string html)
    {
        var nota = new NotaFiscalDTO
        {
            Estabelecimento = Texto(Estabelecimento().Match(html).Groups[1].Value),
            Itens = LinhaItem().Matches(html).Select(m => new NotaFiscalItemDTO
            {
                Nome = Texto(m.Groups["nome"].Value),
                Quantidade = Numero(m.Groups["qtd"].Value),
                Valor = Numero(m.Groups["valor"].Value)
            }).ToList()
        };
        var total = TotalPagar().Match(html);
        if (nota.Itens.Count == 0 || !total.Success)
            throw new RegraNegocioException("Não consegui ler os itens desta nota.");
        nota.Total = Numero(total.Groups[1].Value);
        return nota;
    }

    private static string Texto(string s) => WebUtility.HtmlDecode(s).Trim();
    private static decimal Numero(string s) => decimal.Parse(s.Trim(), NumberStyles.Number, PtBr);

    [GeneratedRegex("class=\"txtTopo\"[^>]*>([^<]*)<")]
    private static partial Regex Estabelecimento();
    [GeneratedRegex("class=\"txtTit2\"[^>]*>(?<nome>[^<]*)<.*?class=\"Rqtd\"[^>]*>.*?</strong>\\s*(?<qtd>[\\d.,]+).*?class=\"valor\"[^>]*>\\s*(?<valor>[\\d.,]+)", RegexOptions.Singleline)]
    private static partial Regex LinhaItem();
    [GeneratedRegex("Valor a pagar[^<]*</label>\\s*<span[^>]*>\\s*([\\d.,]+)", RegexOptions.IgnoreCase)]
    private static partial Regex TotalPagar();
}
