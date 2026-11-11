using System.Globalization;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Notificacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

/// <summary>
/// Card #73: os avisos são calculados na hora, com os mesmos serviços das telas de origem (os números batem), e nada é
/// gravado além do "lido até". Cada aviso tem a data em que passou a valer (<see cref="AvisoDTO.Desde"/>): é "novo"
/// quando isso é depois do "lido até". "Hoje" é o dia em São Paulo.
/// </summary>
public class NotificacaoService(
    IPreferenciaNotificacaoRepository preferencias,
    ITransacaoService transacoes,
    IResumoContasService resumoContas,
    IItemListaService itensLista,
    IAcertosService acertos,
    TimeProvider relogio) : INotificacaoService
{
    public const string ContasAVencer = "contas-a-vencer";
    public const string Vencidas = "vencidas";
    public const string Fatura = "fatura";
    public const string Agenda = "agenda";
    public const string Acertos = "acertos";

    /// <summary>Vencidas há mais que isso são "antigas" (dívidas paradas).</summary>
    public const int DiasRecentes = 90;

    private static readonly TimeZoneInfo SaoPaulo = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public async Task<NotificacoesDTO> ListarAsync(int userId)
    {
        var linha = await preferencias.ObterAsync(userId);
        var pref = PreferenciaNotificacaoService.Ler(linha?.Config);
        var hoje = Hoje();
        if (pref.PausadoAte is { } pausa && pausa >= hoje)
            return new NotificacoesDTO { LidasAte = linha?.LidasAte, PausadoAte = pausa };

        var avisos = new List<AvisoDTO>();
        if (pref.Vencidas.Ativo) avisos.AddRange(await VencidasAsync(userId, hoje, pref.Vencidas.SoUltimos90Dias));
        if (pref.ContasAVencer.Ativo) avisos.AddRange(await ContasAVencerAsync(userId, hoje, pref.ContasAVencer.DiasAntes));
        if (pref.Fatura.Ativo) avisos.AddRange(await FaturasAsync(userId, hoje, pref.Fatura.DiasAntes));
        if (pref.Agenda.Ativo) avisos.AddRange(await AgendaAsync(userId, hoje, pref.Agenda.DiasAntes));
        if (pref.Acertos.Ativo) avisos.AddRange(await AcertosAsync(userId, hoje, pref.Acertos.DiasEmAberto));

        var lidasAte = linha?.LidasAte;
        foreach (var a in avisos)
            a.Novo = lidasAte == null || a.Desde > lidasAte.Value;

        return new NotificacoesDTO
        {
            Avisos = avisos.OrderByDescending(a => a.Desde).ThenBy(a => a.Data).ThenBy(a => a.Titulo).ToList(),
            Novos = avisos.Count(a => a.Novo),
            LidasAte = lidasAte
        };
    }

    public Task MarcarLidasAsync(int userId) =>
        preferencias.MarcarLidasAsync(userId, relogio.GetUtcNow().UtcDateTime);

    // ---------- tipos ----------

    /// <summary>Um aviso só por dia, com a mesma base do selo do menu (GetContaVencida).</summary>
    private async Task<IEnumerable<AvisoDTO>> VencidasAsync(int userId, DateOnly hoje, bool soRecentes)
    {
        var todas = (await transacoes.GetContaVencida(hoje.ToDateTime(TimeOnly.MinValue), userId)).ToList();
        var limite = hoje.AddDays(-DiasRecentes);
        var recentes = todas.Where(t => t.Data >= limite).ToList();
        var antigas = todas.Count - recentes.Count;
        var consideradas = soRecentes ? recentes : todas;
        if (consideradas.Count == 0)
            return [];

        string? detalhe = null;
        if (antigas > 0)
            detalhe = soRecentes
                ? $"Mais {antigas} há mais de {DiasRecentes} dias, fora deste aviso"
                : $"{recentes.Count} {Plural(recentes.Count, "recente", "recentes")} · {antigas} há mais de {DiasRecentes} dias";

        return
        [
            new AvisoDTO
            {
                Id = $"{Vencidas}:{Iso(hoje)}",
                Tipo = Vencidas,
                Desde = InicioDoDia(hoje),
                Titulo = $"{consideradas.Count} {Plural(consideradas.Count, "conta vencida", "contas vencidas")}",
                Detalhe = detalhe,
                Valor = consideradas.Sum(t => t.Valor),
                Data = hoje
            }
        ];
    }

    /// <summary>Despesas em aberto que vencem de hoje até hoje + antecedência (as atrasadas estão nas vencidas).</summary>
    private async Task<IEnumerable<AvisoDTO>> ContasAVencerAsync(int userId, DateOnly hoje, int diasAntes)
    {
        var ate = hoje.AddDays(diasAntes);
        var contas = new List<TransacaoDTO>();
        foreach (var mes in Meses(hoje, ate))
            contas.AddRange(await transacoes.BuscarContasMensais(mes.ToDateTime(TimeOnly.MinValue), userId));

        return contas
            .Where(t => t.Data >= hoje && t.Data <= ate && !Paga(t) && !EhReceita(t))
            .DistinctBy(t => t.Id)
            .Select(t => new AvisoDTO
            {
                Id = $"vencer:{t.Id}:{Iso(t.Data)}",
                Tipo = ContasAVencer,
                Desde = InicioDoDia(t.Data.AddDays(-diasAntes)),
                Titulo = $"{t.Nome} vence {Quando(t.Data, hoje)}",
                Detalhe = t.CategoriaNome,
                Valor = t.Valor,
                Data = t.Data,
                TransacaoId = t.Id
            })
            .ToList();
    }

    /// <summary>Cartões ativos com fatura no mês: avisa o fechamento e o vencimento, cada um N dias antes.</summary>
    private async Task<IEnumerable<AvisoDTO>> FaturasAsync(int userId, DateOnly hoje, int diasAntes)
    {
        var ate = hoje.AddDays(diasAntes);
        var avisos = new Dictionary<string, AvisoDTO>();
        foreach (var mes in Meses(hoje, ate))
        {
            var resumo = await resumoContas.ObterAsync(userId, mes.ToString("yyyy-MM"));
            foreach (var c in resumo.Contas.Where(c => c.EhCredito && c.Ativo && c.Fatura > 0))
            {
                Evento(c.BancoId, c.Nome, c.Fatura, c.DataFechamento, "fecha", "fechamento");
                Evento(c.BancoId, c.Nome, c.Fatura, c.DataVencimento, "vence", "vencimento");
            }
        }
        return avisos.Values.ToList();

        void Evento(int bancoId, string nome, decimal fatura, DateOnly? data, string verbo, string chave)
        {
            if (data is not { } d || d < hoje || d > ate)
                return;
            var id = $"fatura:{bancoId}:{chave}:{Iso(d)}";
            avisos[id] = new AvisoDTO
            {
                Id = id,
                Tipo = Fatura,
                Desde = InicioDoDia(d.AddDays(-diasAntes)),
                Titulo = $"Fatura do {nome} {verbo} {Quando(d, hoje)}",
                Valor = fatura,
                Data = d,
                BancoId = bancoId
            };
        }
    }

    /// <summary>Itens não feitos da agenda de todos os planejamentos, de hoje até hoje + antecedência (0 ou 1).</summary>
    private async Task<IEnumerable<AvisoDTO>> AgendaAsync(int userId, DateOnly hoje, int diasAntes)
    {
        var ocorrencias = await itensLista.ListarAgendaAsync(hoje, hoje.AddDays(diasAntes), userId);
        return ocorrencias
            .Where(o => !o.Feito)
            .Select(o =>
            {
                var quando = Quando(o.Data, hoje) + (o.Hora is { } h ? $" às {h.ToString("HH:mm", PtBr)}" : "");
                return new AvisoDTO
                {
                    Id = $"agenda:{o.ItemId}:{Iso(o.DataOriginal)}",
                    Tipo = Agenda,
                    Desde = InicioDoDia(o.Data.AddDays(-diasAntes)),
                    Titulo = $"{o.Descricao} {quando}",
                    Detalhe = o.ListaNome,
                    Data = o.Data,
                    ListaId = o.ListaId
                };
            })
            .ToList();
    }

    /// <summary>
    /// Um aviso só com quem deve há mais de N dias (a partir da compra mais antiga não quitada). Vale a semana toda,
    /// desde a segunda-feira: aparece como novo no máximo uma vez por semana.
    /// </summary>
    private async Task<IEnumerable<AvisoDTO>> AcertosAsync(int userId, DateOnly hoje, int diasEmAberto)
    {
        var dados = await acertos.ObterAsync(userId, null, null, null);
        var segunda = hoje.AddDays(-(((int)hoje.DayOfWeek + 6) % 7));
        var devedores = dados.Pessoas
            .Where(p => !p.EhTitular && p.Falta > 0)
            .Select(p => (Pessoa: p, Inicio: p.Compras.Where(c => !c.Quitada).Select(c => (DateOnly?)c.Data).Min()))
            .Where(x => x.Inicio is { } i && hoje.DayNumber - i.DayNumber > diasEmAberto)
            .ToList();
        if (devedores.Count == 0)
            return [];

        var total = devedores.Sum(x => x.Pessoa.Falta);
        var maisAntiga = devedores.Min(x => x.Inicio!.Value);
        var umaSo = devedores.Count == 1 ? devedores[0].Pessoa : null;
        return
        [
            new AvisoDTO
            {
                Id = $"acertos:{Iso(segunda)}",
                Tipo = Acertos,
                Desde = InicioDoDia(segunda),
                Titulo = umaSo != null
                    ? $"{umaSo.Nome} te deve {total.ToString("C", PtBr)}"
                    : $"{devedores.Count} pessoas te devem {total.ToString("C", PtBr)}",
                Detalhe = umaSo != null ? $"Em aberto há {hoje.DayNumber - maisAntiga.DayNumber} dias" : $"Há mais de {diasEmAberto} dias",
                Valor = total,
                Data = maisAntiga,
                PessoaId = umaSo?.PessoaId
            }
        ];
    }

    // ---------- apoio ----------

    private DateOnly Hoje() =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(relogio.GetUtcNow(), SaoPaulo).DateTime);

    /// <summary>Meia-noite de São Paulo do dia, em UTC.</summary>
    private static DateTime InicioDoDia(DateOnly dia) =>
        TimeZoneInfo.ConvertTimeToUtc(dia.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified), SaoPaulo);

    /// <summary>Primeiro dia de cada mês entre <paramref name="de"/> e <paramref name="ate"/>.</summary>
    private static IEnumerable<DateOnly> Meses(DateOnly de, DateOnly ate)
    {
        for (var m = new DateOnly(de.Year, de.Month, 1); m <= ate; m = m.AddMonths(1))
            yield return m;
    }

    private static bool Paga(TransacaoDTO t) =>
        t.Situacao == StatusTransacao.PagoNoPrazo || t.Situacao == StatusTransacao.PagoAtrasado;

    private static bool EhReceita(TransacaoDTO t) =>
        string.Equals(t.SubcategoriaNome, "Receita", StringComparison.OrdinalIgnoreCase)
        || string.Equals(t.SubcategoriaNome, "Receitas", StringComparison.OrdinalIgnoreCase);

    private static string Quando(DateOnly data, DateOnly hoje) => (data.DayNumber - hoje.DayNumber) switch
    {
        0 => "hoje",
        1 => "amanhã",
        var n => $"em {n} dias"
    };

    private static string Iso(DateOnly d) => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    private static string Plural(int n, string um, string varios) => n == 1 ? um : varios;
    private static string Primeira(string s) => s.Length == 0 ? s : char.ToUpper(s[0], PtBr) + s[1..];
}
