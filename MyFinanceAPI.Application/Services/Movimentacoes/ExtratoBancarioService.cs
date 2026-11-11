using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services
{
    public partial class ExtratoBancarioService : IExtratoBancarioService
    {
        private readonly IExtratoBancarioRepository _extratoBancarioRepository;
        private readonly IExtratoBancarioItemRepository _extratoBancarioItemRepository;
        private readonly IMapper _mapper;
        private readonly IBancoService _bancoService;
        private readonly IPessoaMovimentacaoRepository _pessoaMovimentacaoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IRegraClassificacaoRepository _regraRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExtratoBancarioService(
            IExtratoBancarioRepository extratoBancarioRepository,
            IExtratoBancarioItemRepository extratoBancarioItemRepository,
            IPessoaMovimentacaoRepository pessoaMovimentacaoRepository,
            ICategoriaRepository categoriaRepository,
            IRegraClassificacaoRepository regraRepository,
            IMapper mapper, IBancoService bancoService, IUnitOfWork unitOfWork)
        {
            _extratoBancarioRepository = extratoBancarioRepository;
            _extratoBancarioItemRepository = extratoBancarioItemRepository;
            _mapper = mapper;
            _pessoaMovimentacaoRepository = pessoaMovimentacaoRepository;
            _categoriaRepository = categoriaRepository;
            _regraRepository = regraRepository;
            _bancoService = bancoService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>Categoria (Despesa, Essencial) dos encargos importados; criada na primeira vez (card #45).</summary>
        public const string CategoriaEncargos = "Tarifas e encargos";
        private const string SubcategoriaDespesa = "Despesa";
        private const int NaturezaEssencial = 1;

        /// <summary>
        /// Lista fixa do card #45, parte 2 (vira regra configurável no card #76). Palavra inteira: "AMORA" não é "MORA".
        /// </summary>
        private static readonly Regex Encargo = new(
            @"\b(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>A descrição é de encargo do cartão ou da conta (IOF, juros, anuidade…): fica com o "Eu".</summary>
        public static bool EhEncargo(string? descricao) => !string.IsNullOrWhiteSpace(descricao) && Encargo.IsMatch(descricao);

        /// <summary>
        /// Lista todos os extratos do usuário.
        /// </summary>
        public async Task<IEnumerable<ExtratoBancarioDTO>> GetExtratoBancario(int userId, string month)
        {
            var extratos = await _extratoBancarioRepository.GetByUserIdAsync(userId, month);

            // Os bancos do usuário numa consulta só, e o nome vai no DTO. Antes era um .Result por extrato
            // dentro do Select (sync-over-async e N+1), gravando o nome na entidade (BE-P4-05).
            var nomes = (await _bancoService.ListarAsync(userId)).ToDictionary(b => b.Id, b => b.NomeBanco);
            var dtos = _mapper.Map<List<ExtratoBancarioDTO>>(extratos);
            foreach (var dto in dtos)
                dto.BancoNome = nomes.GetValueOrDefault(dto.BancoId) ?? "Banco não encontrado";

            return dtos;
        }

        /// <summary>
        /// Retorna um extrato por Id (validando usuário).
        /// </summary>
        public async Task<ExtratoBancarioDTO?> GetExtratoBancarioById(int id, int? userId)
        {
            if (userId is null || userId == 0)
                throw new UnauthorizedAccessException("Usuário não autorizado.");

            // Extrato de outro usuário volta como "não encontrado" (BE-P0-06).
            var extrato = await _extratoBancarioRepository.GetByIdAsync(id, userId.Value);

            if (extrato == null)
                return null;

            return _mapper.Map<ExtratoBancarioDTO>(extrato);
        }

        /// <summary>
        /// Importa o arquivo de extrato, cria o cabeçalho de extrato e os itens.
        /// Correções de centavos, pessoas novas, cabeçalho e itens valem juntos (BE-P1-05).
        /// </summary>
        public Task<ExtratoImportacaoResultadoDTO> ImportarExtratoAsync(
            Stream arquivoStream,
            string fileName,
            int userId,
            int bancoId,
            OpcoesImportacao? opcoes = null
        ) => _unitOfWork.ExecuteAsync(() => ImportarExtratoNaOperacaoAsync(arquivoStream, fileName, userId, bancoId, opcoes ?? new(), null));

        /// <summary>Card #74: o que a importação faria, sem gravar. Sem banco, tenta reconhecer pelo formato.</summary>
        public async Task<PreviaImportacaoDTO> PreverImportacaoAsync(Stream arquivoStream, int userId, int? bancoId)
        {
            var previa = new PreviaImportacaoDTO();
            await ImportarExtratoNaOperacaoAsync(arquivoStream, "", userId, bancoId, new OpcoesImportacao(), previa);
            return previa;
        }

        /// <summary>Banco do usuário pelo formato do arquivo, preferindo o tipo certo (fatura = cartão); null se nenhum.</summary>
        private async Task<BancoDTO?> DetectarBancoAsync(ArquivoLido arquivo, int userId)
        {
            Func<string, bool>? casa = arquivo.Formato switch
            {
                var f when f.Contains("Nubank") => n => n.StartsWith("NU"),
                var f when f.Contains("PicPay") => n => n.Contains("PICPAY"),
                var f when f.Contains("Caixa") => n => n.Contains("CAIXA"),
                _ => null
            };
            if (casa == null)
                return null;
            return (await _bancoService.ListarAsync(userId))
                .Where(b => b.Ativo && casa(b.NomeBanco.ToUpperInvariant().Replace(" ", "")))
                .OrderByDescending(b => b.TipoCartao?.EhCredito == arquivo.EhFatura)
                .ThenBy(b => b.NomeBanco)
                .FirstOrDefault();
        }

        private static LinhaPreviaImportacaoDTO LinhaDuplicada(LinhaImportada l, string chave) => new()
        {
            Chave = chave, Data = l.Data, Descricao = l.Descricao, Valor = Math.Abs(l.Valor), Duplicado = true,
            TipoLancamento = l.Valor < 0 ? TipoLancamentoExtrato.Saida : TipoLancamentoExtrato.Entrada
        };

        private async Task<ExtratoImportacaoResultadoDTO> ImportarExtratoNaOperacaoAsync(
            Stream arquivoStream,
            string fileName,
            int userId,
            int? bancoId,
            OpcoesImportacao opcoes,
            PreviaImportacaoDTO? previa
        )
        {
            // Card #76, fase 4: o formato vem do conteúdo; desconhecido é recusado sem gravar nada.
            using var memoria = new MemoryStream();
            await arquivoStream.CopyToAsync(memoria);
            var arquivo = LeitorArquivoImportacao.Ler(memoria.ToArray());
            if (previa != null)
            {
                if (arquivo.Vazio)
                    throw new RegraNegocioException("Arquivo vazio ou sem lançamentos.");
                previa.Formato = arquivo.Formato;
                previa.EhFatura = arquivo.EhFatura;
                previa.Aviso = arquivo.Aviso.Trim();
            }

            // Banco inexistente ou de outro usuário: 404, não NullReferenceException (500).
            var banco = bancoId is int id
                ? await _bancoService.ObterPorIdAsync(id, userId) ?? throw new NaoEncontradoException("Banco não encontrado.")
                : previa != null ? await DetectarBancoAsync(arquivo, userId) : throw new NaoEncontradoException("Banco não encontrado.");
            if (banco == null)
                return new ExtratoImportacaoResultadoDTO(); // prévia sem banco reconhecido: a tela pede para escolher
            if (previa != null)
            {
                previa.BancoId = banco.Id;
                previa.BancoDetectado = bancoId == null;
            }

            // Categoria escolhida na prévia precisa ser do usuário.
            var escolhidas = opcoes.Categorias.Values.Where(c => c is > 0).Select(c => c!.Value).Distinct().ToList();
            if (escolhidas.Count > 0 && await _extratoBancarioItemRepository.TemCategoriaForaDoUsuarioAsync(escolhidas, userId))
                throw new RegraNegocioException("Categoria inválida.");
            var culturePtBr = new CultureInfo("pt-BR");
            var cultureDecimal = CultureInfo.InvariantCulture;

            int criados = 0;
            int ignorados = 0;
            int jaImportados = 0;
            int corrigidos = 0;
            int desmarcados = 0;

            // Idempotência: linhas cuja chave já existe (ou repetidas no próprio arquivo) são puladas.
            var chavesExistentes = await _extratoBancarioItemRepository.ObterChavesImportacaoAsync(userId, banco.Id);

            // Reparo do BE-P0-07: importações antigas gravaram o valor sem os centavos, então a chave
            // delas não bate. Uma linha com centavos que casa com um desses itens corrige o item em vez de duplicar.
            var truncados = (await _extratoBancarioItemRepository.ObterCandidatosTruncadosAsync(userId, banco.Id))
                .GroupBy(i => ChaveLegado(i.Identificador!, i.DataMovimentacao, i.TipoLancamento, i.Descricao))
                .ToDictionary(g => g.Key, g => g.ToList());

            var listaItens = new List<ExtratoBancarioItemDTO>();
            // Só buscados (ou criados) quando aparece o primeiro encargo.
            PessoaMovimentacao? titular = null;
            Categoria? categoriaEncargos = null;
            // Card #76: as regras ligadas, lidas uma vez por importação.
            var regras = await _regraRepository.ListarAtivasAsync(userId);
            // Quantos lançamentos cada regra classificou nesta importação ("aplicada N vezes").
            var usoRegras = new Dictionary<int, int>();

            DateOnly? dataInicio = null;
            DateOnly? dataFim = null;
            decimal somaValores = 0;

            if (arquivo.Vazio)
            {
                return new ExtratoImportacaoResultadoDTO(0, 0, 0, 0, "Arquivo vazio ou inválido.");
            }

            if (arquivo.Formato != "Nubank" && banco.TipoCartao is { } tipoConta && tipoConta.EhCredito != arquivo.EhFatura)
                throw new RegraNegocioException(arquivo.EhFatura
                    ? "Este arquivo é uma fatura: escolha uma conta de cartão de crédito."
                    : "Este arquivo é um extrato: escolha uma conta que não seja de cartão de crédito.");

            ignorados = arquivo.Invalidas;
            foreach (var linhaLida in arquivo.Linhas)
            {
                var dataMov = linhaLida.Data;
                var valor = linhaLida.Valor;
                var identificadorStr = linhaLida.Identificador;
                // Card #79: o banco manda "Z&amp;A"; grava "Z&A".
                var descricaoStr = DecodificarHtml(linhaLida.Descricao);

                // As chaves usam o texto como veio no arquivo: reimportar um arquivo antigo com "&amp;" não duplica.
                var chaveImportacao = GerarChaveImportacao(identificadorStr, dataMov, valor, linhaLida.Descricao);
                if (!chavesExistentes.Add(chaveImportacao))
                {
                    jaImportados++;
                    previa?.Linhas.Add(LinhaDuplicada(linhaLida, chaveImportacao));
                    continue;
                }
                // Desmarcada na prévia (card #74).
                if (opcoes.Pular.Contains(chaveImportacao))
                {
                    desmarcados++;
                    continue;
                }

                var valorSemSinal = Math.Abs(valor);
                if (valorSemSinal != Math.Truncate(valorSemSinal)
                    && truncados.TryGetValue(
                        ChaveLegado(identificadorStr, dataMov, valor < 0 ? TipoLancamentoExtrato.Saida : TipoLancamentoExtrato.Entrada, linhaLida.Descricao),
                        out var candidatos))
                {
                    var truncado = candidatos.FirstOrDefault(c => c.Valor == Math.Truncate(valorSemSinal));
                    if (truncado != null && previa != null)
                    {
                        // Na prévia, o item antigo (sem centavos) conta como já importado; ao gravar, ele é corrigido.
                        jaImportados++;
                        previa.Linhas.Add(LinhaDuplicada(linhaLida, chaveImportacao));
                        continue;
                    }
                    if (truncado != null)
                    {
                        candidatos.Remove(truncado);
                        truncado.Valor = valorSemSinal;
                        truncado.ChaveImportacao = chaveImportacao;
                        await _extratoBancarioItemRepository.UpdateAsync(truncado);
                        corrigidos++;
                        continue;
                    }
                }

                try
                {
                    var tipoLancamento = valor < 0 ? TipoLancamentoExtrato.Saida : TipoLancamentoExtrato.Entrada;
                    // Sem cast para int: o (int) descartava os centavos (BE-P0-07).
                    var valorAbsoluto = Math.Abs(valor);
                    
                    string? nomePessoa = null;
                    var encargo = EhEncargo(descricaoStr);
                    if (!encargo && !string.IsNullOrWhiteSpace(linhaLida.Nome))
                    {
                        nomePessoa = DecodificarHtml(linhaLida.Nome).Trim().ToUpper();
                    }
                    else if (!encargo && !string.IsNullOrWhiteSpace(descricaoStr))
                    {
                        var partesDesc = descricaoStr.Split('-', StringSplitOptions.RemoveEmptyEntries);
                        if (partesDesc.Length >= 2)
                            nomePessoa = partesDesc[1].Trim() == null ? descricaoStr.ToUpper() : partesDesc[1].Trim().ToUpper();
                    }

                    // Descrição sem pessoa identificável (ex.: "Pagamento de fatura") → item sem pessoa.
                    int? pessoaId = null;
                    var tipoMovimentacaoId = 0;
                    var categoriaId = 0;
                    var entraNosAcertos = false;

                    if (encargo)
                    {
                        // Encargo fica com o dono: "Eu" + "Tarifas e encargos", sem criar pessoa pelo nome.
                        if (previa != null)
                        {
                            // A prévia não grava: não cria o "Eu" nem a categoria de encargos.
                            nomePessoa = "EU";
                            categoriaEncargos ??= await _categoriaRepository.ObterPorNomeAsync(userId, CategoriaEncargos, SubcategoriaDespesa);
                            categoriaId = categoriaEncargos?.Id ?? 0;
                        }
                        else
                        {
                            titular ??= await _pessoaMovimentacaoRepository.ObterTitularAsync(userId);
                            categoriaEncargos ??= await ObterCategoriaEncargosAsync(userId);
                            pessoaId = titular.Id;
                            nomePessoa = titular.NomePessoa;
                            categoriaId = categoriaEncargos.Id;
                        }
                    }
                    else if (ClassificadorRegras.Encontrar(regras, descricaoStr, nomePessoa, banco.Id) is { PessoaMovimentacaoId: not null } regraComPessoa)
                    {
                        // Regra que liga uma pessoa: usa ela, sem criar outra pelo nome da descrição (card #76).
                        pessoaId = regraComPessoa.PessoaMovimentacaoId;
                        nomePessoa = regraComPessoa.PessoaMovimentacao?.NomePessoa ?? nomePessoa;
                        tipoMovimentacaoId = regraComPessoa.PessoaMovimentacao?.TipoMovimentacaoId ?? 0;
                        categoriaId = regraComPessoa.PessoaMovimentacao?.CategoriaId ?? 0;
                        entraNosAcertos = regraComPessoa.PessoaMovimentacao is { EhTitular: false };
                    }
                    // Card #79: o nome do extrato ("CIRCUITO") nunca vira nem liga pessoa; pessoa só por regra, à mão
                    // ou o "Eu" dos encargos. A categoria vem das regras.

                    // Ordem (card #76/#79): encargo > regra > semelhança com uma regra "igual a".
                    int? regraUsada = null;
                    var porSemelhanca = false;
                    if (!encargo && ClassificadorRegras.Encontrar(regras, descricaoStr, nomePessoa, banco.Id) is { } regra)
                    {
                        regraUsada = regra.Id;
                        categoriaId = regra.CategoriaId ?? categoriaId;
                        tipoMovimentacaoId = regra.TipoMovimentacaoId ?? tipoMovimentacaoId;
                    }
                    else if (!encargo && ClassificadorRegras.EncontrarPorSemelhanca(regras, descricaoStr, nomePessoa, banco.Id) is { } parecida)
                    {
                        regraUsada = parecida.Id;
                        porSemelhanca = parecida.CategoriaId != null || parecida.TipoMovimentacaoId != null;
                        categoriaId = parecida.CategoriaId ?? categoriaId;
                        tipoMovimentacaoId = parecida.TipoMovimentacaoId ?? tipoMovimentacaoId;
                    }
                    // Categoria trocada na prévia vale sobre a sugerida (card #74).
                    if (opcoes.Categorias.TryGetValue(chaveImportacao, out var escolhida))
                    {
                        categoriaId = escolhida ?? 0;
                        regraUsada = null;
                        porSemelhanca = false;
                    }

                    var item = new ExtratoBancarioItemDTO
                    {
                        DataMovimentacao = dataMov,
                        BancoId = banco.Id,
                        TipoCartaoId = banco.TipoCartaoId,
                        CategoriaId = categoriaId != 0 ? categoriaId : null,
                        TipoMovimentacaoId =  tipoMovimentacaoId != 0 ? tipoMovimentacaoId : null,
                        Valor = valorAbsoluto,
                        TipoLancamento = tipoLancamento,
                        Descricao = descricaoStr,
                        PessoaMovimentacaoId = pessoaId,
                        NomePessoaTransacao = nomePessoa,
                        Identificador = identificadorStr,
                        ChaveImportacao = chaveImportacao,
                        NumeroFatura = linhaLida.NumeroFatura,
                        ParcelaAtual = linhaLida.ParcelaAtual,
                        QuantidadeParcelas = linhaLida.QuantidadeParcelas,
                        EhParcelado = linhaLida.QuantidadeParcelas != null ? true : null,
                        Observacao = linhaLida.Observacao,
                        ClassificadoPorSemelhanca = porSemelhanca,
                    };

                    listaItens.Add(item);
                    previa?.Linhas.Add(new LinhaPreviaImportacaoDTO
                    {
                        Chave = chaveImportacao, Data = dataMov, Descricao = descricaoStr, Valor = valorAbsoluto, TipoLancamento = tipoLancamento,
                        CategoriaId = item.CategoriaId, NomePessoa = nomePessoa, EntraNosAcertos = entraNosAcertos
                    });
                    criados++;
                    if (regraUsada is int usada)
                        usoRegras[usada] = usoRegras.GetValueOrDefault(usada) + 1;

                    if (dataInicio == null || dataMov < dataInicio.Value)
                        dataInicio = dataMov;
                    if (dataFim == null || dataMov > dataFim.Value)
                        dataFim = dataMov;

                    somaValores += valor;
                }
                catch
                {
                    ignorados++;
                }
            }

            if (previa != null)
            {
                var mes = ImportacoesService.MesDaFatura(arquivo.Linhas.Select(l => l.NumeroFatura).FirstOrDefault(n => n != null))
                    ?? (arquivo.Linhas.Count > 0 ? new DateOnly(arquivo.Linhas.Min(l => l.Data).Year, arquivo.Linhas.Min(l => l.Data).Month, 1) : null);
                previa.MesReferencia = mes?.ToString("yyyy-MM", CultureInfo.InvariantCulture);
                previa.DataInicio = arquivo.Linhas.Count > 0 ? arquivo.Linhas.Min(l => l.Data) : null;
                previa.DataFim = arquivo.Linhas.Count > 0 ? arquivo.Linhas.Max(l => l.Data) : null;
                previa.Novos = listaItens.Count;
                previa.Duplicados = jaImportados;
                previa.Invalidas = ignorados;
                previa.Entradas = listaItens.Where(i => i.TipoLancamento == TipoLancamentoExtrato.Entrada).Sum(i => i.Valor);
                previa.Saidas = listaItens.Where(i => i.TipoLancamento == TipoLancamentoExtrato.Saida).Sum(i => i.Valor);
                previa.MesJaImportado = mes != null && (await _extratoBancarioRepository.ListarImportacoesAsync(userId, null))
                    .Any(i => i.BancoId == banco.Id && i.DesfeitoEm == null && i.MesReferencia == mes);
                return new ExtratoImportacaoResultadoDTO();
            }

            if (!listaItens.Any())
            {
                var semNovos = jaImportados + corrigidos > 0
                    ? $"Nenhum lançamento novo: {jaImportados} já importado(s)"
                      + (corrigidos > 0 ? $", {corrigidos} corrigido(s) (centavos)" : "")
                      + (desmarcados > 0 ? $", {desmarcados} desmarcado(s)" : "")
                      + (ignorados > 0 ? $", {ignorados} ignorado(s)." : ".")
                    : "Nenhum lançamento válido foi encontrado no arquivo.";
                semNovos += arquivo.Aviso;
                return new ExtratoImportacaoResultadoDTO(
                    0,
                    0,
                    ignorados,
                    0,
                    semNovos
                ) { QuantidadeJaImportados = jaImportados, QuantidadeDesmarcados = desmarcados };
            }

            var extrato = new ExtratoBancario
            {
                DataImportacao = DateTime.UtcNow,
                DataInicioPeriodo = dataInicio,
                DataFimPeriodo = dataFim,
                BancoId = banco.Id,
                TipoCartaoId = banco.TipoCartaoId,
                QuantidadeLancamentos = listaItens.Count,
                ValorTotal = somaValores,
                Situacao = "Concluido",
                QuantidadeDuplicados = jaImportados,
                MesReferencia = ImportacoesService.MesDaFatura(listaItens.Select(i => i.NumeroFatura).FirstOrDefault(n => n != null))
                    ?? (dataInicio is { } inicio ? new DateOnly(inicio.Year, inicio.Month, 1) : null),
                NomeArquivoOrigem = fileName,
                LoteImportacaoId = Guid.NewGuid(),
                UsuarioId = userId
            };

            await _extratoBancarioRepository.CreateAsync(extrato);

            foreach (var item in listaItens)
            {
                item.ExtratoBancarioId = extrato.Id;
                item.ChaveDescricao = GerarChaveDescricao(item.Descricao);
            }

            // O dono vai direto na entidade: o DTO não tem UsuarioId, que só vem do token (BE-P4-17).
            var itens = _mapper.Map<List<ExtratoBancarioItem>>(listaItens);
            itens.ForEach(i => i.UsuarioId = userId);
            await _extratoBancarioItemRepository.CreateRangeAsync(itens);
            await _regraRepository.RegistrarUsoAsync(userId, usoRegras);

            var mensagem = $"Importação concluída. Criados: {criados}, já importados: {jaImportados}, "
                + (corrigidos > 0 ? $"corrigidos (centavos): {corrigidos}, " : "")
                + (desmarcados > 0 ? $"desmarcados: {desmarcados}, " : "")
                + $"ignorados: {ignorados}." + arquivo.Aviso;
            // Fatura aparece no mês do vencimento, que pode não ser o das compras: a mensagem diz onde encontrar.
            var mesDestino = arquivo.Linhas.FirstOrDefault(l => l.NumeroFatura != null)?.NumeroFatura
                ?? dataFim?.ToString("yyyy-MM", CultureInfo.InvariantCulture);
            if (mesDestino != null)
                mensagem += " Veja em " + DateOnly.ParseExact(mesDestino + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("MMMM 'de' yyyy", culturePtBr) + ".";
            return new ExtratoImportacaoResultadoDTO(
                extrato.Id,
                criados,
                ignorados,
                somaValores,
                mensagem
            ) { MesDestino = mesDestino, QuantidadeJaImportados = jaImportados, QuantidadeDesmarcados = desmarcados };
        }

        private async Task<Categoria> ObterCategoriaEncargosAsync(int userId) =>
            await _categoriaRepository.ObterPorNomeAsync(userId, CategoriaEncargos, SubcategoriaDespesa)
            ?? await _categoriaRepository.CriarAsync(new Categoria
            {
                Nome = CategoriaEncargos,
                Subcategoria = SubcategoriaDespesa,
                NaturezaOperacao = NaturezaEssencial,
                Ativo = true,
                UsuarioId = userId
            });

        /// <summary>
        /// Chave de idempotência de uma linha do extrato. A mesma fórmula está no SQL da migration
        /// Integridade (preenchimento dos itens antigos) — mudar uma exige mudar a outra.
        /// O Nubank repete o Identificador no estorno; o valor com sinal e a descrição os distinguem.
        /// </summary>
        public static string GerarChaveImportacao(string identificador, DateOnly data, decimal valorComSinal, string? descricao)
        {
            var linha = string.Join('|',
                identificador.Trim(),
                data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                valorComSinal.ToString("0.00", CultureInfo.InvariantCulture),
                (descricao ?? string.Empty).Trim());
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(linha))).ToLowerInvariant();
        }

        /// <summary>Identifica a mesma linha do arquivo sem depender do valor (que o BE-P0-07 truncou).</summary>
        private static string ChaveLegado(string identificador, DateOnly data, string tipoLancamento, string? descricao) =>
            string.Join('|', identificador.Trim(), data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                tipoLancamento, (descricao ?? string.Empty).Trim());

        /// <summary>Card #79: "Z&amp;AMP;A" e "Z&amp;amp;A" viram "Z&amp;A" (a entidade em maiúsculas também).</summary>
        public static string DecodificarHtml(string? texto) =>
            string.IsNullOrEmpty(texto) ? texto ?? "" : System.Net.WebUtility.HtmlDecode(EntidadeMaiuscula().Replace(texto, m => m.Value.ToLowerInvariant()));

        [GeneratedRegex("&(AMP|LT|GT|QUOT|APOS|NBSP);")]
        private static partial Regex EntidadeMaiuscula();

        public static string GerarChaveDescricao(string? descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                return string.Empty;

            var upper = descricao.ToUpperInvariant();

            // remove números (se quiser)
            upper = Regex.Replace(upper, "[0-9]", "");

            // remove espaços duplicados
            upper = Regex.Replace(upper, @"\s+", " ").Trim();

            // limitar tamanho pra índice ficar leve
            return upper.Length > 100 ? upper.Substring(0, 100) : upper;
        }


        /// <summary>
        /// Remove um extrato + itens (se aplicável).
        /// </summary>
        public Task Remove(int id, int userId) =>
            _unitOfWork.ExecuteAsync(() => RemoveNaOperacao(id, userId));

        private async Task RemoveNaOperacao(int id, int userId)
        {
            var extrato = await _extratoBancarioRepository.GetByIdAsync(id, userId);
            if (extrato == null)
                throw new KeyNotFoundException("Extrato bancário não encontrado.");

            // Remove itens primeiro (FK)
            await _extratoBancarioItemRepository.RemoveByExtratoIdAsync(id);

            // Remove o cabeçalho
            await _extratoBancarioRepository.DeleteAsync(extrato.Id, userId);
        }
    }
}
