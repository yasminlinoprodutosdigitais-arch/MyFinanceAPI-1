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
using MyFinanceAPI.Domain.Interfaces.Repositories;

namespace MyFinanceAPI.Application.Services
{
    public class ExtratoBancarioService : IExtratoBancarioService
    {
        private readonly IExtratoBancarioRepository _extratoBancarioRepository;
        private readonly IExtratoBancarioItemRepository _extratoBancarioItemRepository;
        private readonly IMapper _mapper;
        private readonly IBancoService _bancoService;
        private readonly IPessoaMovimentacaoRepository _pessoaMovimentacaoRepository;

        public ExtratoBancarioService(
            IExtratoBancarioRepository extratoBancarioRepository,
            IExtratoBancarioItemRepository extratoBancarioItemRepository,
            IPessoaMovimentacaoRepository pessoaMovimentacaoRepository,
            IMapper mapper, IBancoService bancoService)
        {
            _extratoBancarioRepository = extratoBancarioRepository;
            _extratoBancarioItemRepository = extratoBancarioItemRepository;
            _mapper = mapper;
            _pessoaMovimentacaoRepository = pessoaMovimentacaoRepository;
            _bancoService = bancoService;
        }

        /// <summary>
        /// Lista todos os extratos do usuário.
        /// </summary>
        public async Task<IEnumerable<ExtratoBancarioDTO>> GetExtratoBancario(int userId, string month)
        {
            // Aqui você pode ter um campo UserId no ExtratoBancario (em BaseEntity)
            var extratos = await _extratoBancarioRepository.GetByUserIdAsync(userId, month);
            
            extratos = extratos.Select(e =>
            {
                var banco = _bancoService.GetBancoById(e.BancoId, userId).Result;
                e.BancoNome = banco?.NomeBanco ?? "Banco não encontrado";
                return e;
            });
            return _mapper.Map<IEnumerable<ExtratoBancarioDTO>>(extratos);
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
        /// </summary>

        public async Task<ExtratoImportacaoResultadoDTO> ImportarExtratoAsync(
            Stream arquivoStream,
            string fileName,
            int userId,
            int bancoId
        )
        {
            var banco = await _bancoService.GetBancoById(bancoId, userId);
            var culturePtBr = new CultureInfo("pt-BR");
            var cultureDecimal = CultureInfo.InvariantCulture;

            int criados = 0;
            int ignorados = 0;
            int jaImportados = 0;
            int corrigidos = 0;

            // Idempotência: linhas cuja chave já existe (ou repetidas no próprio arquivo) são puladas.
            var chavesExistentes = await _extratoBancarioItemRepository.ObterChavesImportacaoAsync(userId, banco.Id);

            // Reparo do BE-P0-07: importações antigas gravaram o valor sem os centavos, então a chave
            // delas não bate. Uma linha com centavos que casa com um desses itens corrige o item em vez de duplicar.
            var truncados = (await _extratoBancarioItemRepository.ObterCandidatosTruncadosAsync(userId, banco.Id))
                .GroupBy(i => ChaveLegado(i.Identificador!, i.DataMovimentacao, i.TipoLancamento, i.Descricao))
                .ToDictionary(g => g.Key, g => g.ToList());

            var listaItens = new List<ExtratoBancarioItemDTO>();

            DateOnly? dataInicio = null;
            DateOnly? dataFim = null;
            decimal somaValores = 0;

            using var reader = new StreamReader(
                arquivoStream,
                System.Text.Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                leaveOpen: false
            );

            var header = await reader.ReadLineAsync();
            if (header == null)
            {
                return new ExtratoImportacaoResultadoDTO(
                    0,
                    0,
                    0,
                    0,
                    "Arquivo vazio ou inválido."
                );
            }

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',', 4);
                if (parts.Length < 4)
                {
                    ignorados++;
                    continue;
                }

                var dataStr = parts[0].Trim();
                var valorStr = parts[1].Trim();
                var identificadorStr = parts[2].Trim();
                var descricaoStr = parts[3].Trim();

                if (!DateOnly.TryParseExact(dataStr, "dd/MM/yyyy", culturePtBr, DateTimeStyles.None, out var dataMov))
                {
                    ignorados++;
                    continue;
                }

                if (!decimal.TryParse(valorStr, NumberStyles.Number, cultureDecimal, out var valor))
                {
                    ignorados++;
                    continue;
                }

                var chaveImportacao = GerarChaveImportacao(identificadorStr, dataMov, valor, descricaoStr);
                if (!chavesExistentes.Add(chaveImportacao))
                {
                    jaImportados++;
                    continue;
                }

                var valorSemSinal = Math.Abs(valor);
                if (valorSemSinal != Math.Truncate(valorSemSinal)
                    && truncados.TryGetValue(
                        ChaveLegado(identificadorStr, dataMov, valor < 0 ? "Saída" : "Entrada", descricaoStr),
                        out var candidatos))
                {
                    var truncado = candidatos.FirstOrDefault(c => c.Valor == Math.Truncate(valorSemSinal));
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
                    var tipoLancamento = valor < 0 ? "Saída" : "Entrada";
                    // Sem cast para int: o (int) descartava os centavos (BE-P0-07).
                    var valorAbsoluto = Math.Abs(valor);
                    
                    string? nomePessoa = null;
                    if (!string.IsNullOrWhiteSpace(descricaoStr))
                    {
                        var partesDesc = descricaoStr.Split('-', StringSplitOptions.RemoveEmptyEntries);
                        if (partesDesc.Length >= 2)
                            nomePessoa = partesDesc[1].Trim() == null ? descricaoStr.ToUpper() : partesDesc[1].Trim().ToUpper();
                    }

                    // Descrição sem pessoa identificável (ex.: "Pagamento de fatura") → item sem pessoa.
                    int? pessoaId = null;
                    var tipoMovimentacaoId = 0;
                    var categoriaId = 0;

                    if (!string.IsNullOrWhiteSpace(nomePessoa))
                    {
                        var pessoaCadastrada = await _pessoaMovimentacaoRepository.VerificaPossuiPessoa(nomePessoa, userId);
                        if (pessoaCadastrada.Any())
                        {
                            pessoaId = pessoaCadastrada.First().Id;
                            tipoMovimentacaoId = pessoaCadastrada.First().TipoMovimentacaoId ?? 0;
                            categoriaId = pessoaCadastrada.First().CategoriaId ?? 0;
                        }
                        else
                        {
                            var novaPessoa = await _pessoaMovimentacaoRepository.Create(
                                new PessoaMovimentacao { NomePessoa = nomePessoa }, userId);
                            pessoaId = novaPessoa.Id;
                            tipoMovimentacaoId = novaPessoa.TipoMovimentacaoId ?? 0;
                            categoriaId = novaPessoa.CategoriaId ?? 0;
                        }
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
                        UserId = userId
                    };

                    listaItens.Add(item);
                    criados++;

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

            if (!listaItens.Any())
            {
                var semNovos = jaImportados + corrigidos > 0
                    ? $"Nenhum lançamento novo: {jaImportados} já importado(s)"
                      + (corrigidos > 0 ? $", {corrigidos} corrigido(s) (centavos)" : "")
                      + (ignorados > 0 ? $", {ignorados} ignorado(s)." : ".")
                    : "Nenhum lançamento válido foi encontrado no arquivo.";
                return new ExtratoImportacaoResultadoDTO(
                    0,
                    0,
                    ignorados,
                    0,
                    semNovos
                );
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
                NomeArquivoOrigem = fileName,
                LoteImportacaoId = Guid.NewGuid(),
                UserId = userId
            };

            await _extratoBancarioRepository.CreateAsync(extrato);

            foreach (var item in listaItens)
            {
                item.ExtratoBancarioId = extrato.Id;
                item.UserId = userId;
                item.ChaveDescricao = GerarChaveDescricao(item.Descricao);
            }

            var itens = _mapper.Map<IEnumerable<ExtratoBancarioItem>>(listaItens);
            await _extratoBancarioItemRepository.CreateRangeAsync(itens);

            var mensagem = $"Importação concluída. Criados: {criados}, já importados: {jaImportados}, "
                + (corrigidos > 0 ? $"corrigidos (centavos): {corrigidos}, " : "")
                + $"ignorados: {ignorados}.";
            return new ExtratoImportacaoResultadoDTO(
                extrato.Id,
                criados,
                ignorados,
                somaValores,
                mensagem
            );
        }

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

        public static string GerarChaveDescricao(string descricao)
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
        public async Task Remove(int id, int userId)
        {
            var extrato = await _extratoBancarioRepository.GetByIdAsync(id, userId);
            if (extrato == null)
                throw new KeyNotFoundException("Extrato bancário não encontrado.");

            // Remove itens primeiro (FK)
            await _extratoBancarioItemRepository.RemoveByExtratoIdAsync(id);

            // Remove o cabeçalho
            await _extratoBancarioRepository.RemoveAsync(extrato.Id);
        }
    }
}
