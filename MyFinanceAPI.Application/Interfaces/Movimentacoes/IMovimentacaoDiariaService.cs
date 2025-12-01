using System;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces;

public interface IMovimentacaoDiariaService
{
    Task Add(MovimentacaoDiariaDTO MovimentacaoDiariaDTO, int userId);
    Task<IEnumerable<MovimentacaoDiariaDTO>> GetMovimentacaoDiaria(int userId);
    Task<IEnumerable<MovimentacaoDiariaDTO>> GetMovimentacaoByDate( DateTime date, int userId);
    Task<MovimentacaoDiariaDTO> GetMovimentacaoDiariaById(int id, int userId);
    // Task<ExtratoImportacaoResultadoDTO> ImportarExtratoAsync(Stream arquivoStream, string fileName, int userId, BancoDTO banco);
    Task<bool> UpdateAsync(MovimentacaoDiariaDTO dto, int userId);
    Task Remove(int id, int userId);
}
