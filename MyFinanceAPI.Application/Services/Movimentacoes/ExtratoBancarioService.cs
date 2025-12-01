using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces.Repositories;

namespace MyFinanceAPI.Application.Services
{
    public class ExtratoBancarioService : IExtratoBancarioService
    {
        private readonly IExtratoBancarioRepository _extratoBancarioRepository;
        private readonly IExtratoBancarioItemRepository _extratoBancarioItemRepository;
        private readonly IMapper _mapper;
        private readonly IBancoService _bancoService;

        public ExtratoBancarioService(
            IExtratoBancarioRepository extratoBancarioRepository,
            IExtratoBancarioItemRepository extratoBancarioItemRepository,
            IMapper mapper, IBancoService bancoService)
        {
            _extratoBancarioRepository = extratoBancarioRepository;
            _extratoBancarioItemRepository = extratoBancarioItemRepository;
            _mapper = mapper;
            _bancoService = bancoService;
        }

        /// <summary>
        /// Lista todos os extratos do usuário.
        /// </summary>
        public async Task<IEnumerable<ExtratoBancarioDTO>> GetExtratoBancario(int userId)
        {
            // Aqui você pode ter um campo UserId no ExtratoBancario (em BaseEntity)
            var extratos = await _extratoBancarioRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ExtratoBancarioDTO>>(extratos);
        }

        /// <summary>
        /// Retorna um extrato por Id (validando usuário).
        /// </summary>
        public async Task<ExtratoBancarioDTO?> GetExtratoBancarioById(int id, int? userId)
        {
            if (userId is null || userId == 0)
                throw new UnauthorizedAccessException("Usuário não autorizado.");

            var extrato = await _extratoBancarioRepository.GetByIdAsync(id);

            if (extrato == null)
                return null;

            // Se tiver UserId na entidade, valide:
            // if (extrato.UserId != userId.Value) throw new UnauthorizedAccessException();

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

                try
                {
                    var tipoLancamento = valor < 0 ? "Saída" : "Entrada";

                    string? nomePessoa = null;
                    if (!string.IsNullOrWhiteSpace(descricaoStr))
                    {
                        var partesDesc = descricaoStr.Split('-', StringSplitOptions.RemoveEmptyEntries);
                        if (partesDesc.Length >= 2)
                            nomePessoa = partesDesc[1].Trim();
                    }

                    var item = new ExtratoBancarioItemDTO
                    {
                        DataMovimentacao = dataMov,
                        BancoId = banco.Id,
                        TipoCartaoId = banco.TipoCartaoId,
                        TipoMovimentacaoId = null,
                        Valor = valor,
                        TipoLancamento = tipoLancamento,
                        Descricao = descricaoStr,
                        NomePessoaTransacao = nomePessoa,
                        Identificador = identificadorStr,
                        // UserId se tiver em BaseEntity
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
                return new ExtratoImportacaoResultadoDTO(
                    0,
                    0,
                    ignorados,
                    0,
                    "Nenhum lançamento válido foi encontrado no arquivo."
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
            }

            var itens = _mapper.Map<IEnumerable<ExtratoBancarioItem>>(listaItens);
            await _extratoBancarioItemRepository.CreateRangeAsync(itens);

            var mensagem = $"Importação concluída. Criados: {criados}, ignorados: {ignorados}.";
            return new ExtratoImportacaoResultadoDTO(
                extrato.Id,
                criados,
                ignorados,
                somaValores,
                mensagem
            );
        }

        /// <summary>
        /// Remove um extrato + itens (se aplicável).
        /// </summary>
        public async Task Remove(int id, int userId)
        {
            var extrato = await _extratoBancarioRepository.GetByIdAsync(id, userId);
            if (extrato == null)
                throw new KeyNotFoundException("Extrato bancário não encontrado.");

            // Se tiver UserId na entidade, valide:
            // if (extrato.UserId != userId) throw new UnauthorizedAccessException();

            // Remove itens primeiro (FK)
            await _extratoBancarioItemRepository.RemoveByExtratoIdAsync(id);

            // Remove o cabeçalho
            await _extratoBancarioRepository.RemoveAsync(extrato.Id);
        }
    }
}
