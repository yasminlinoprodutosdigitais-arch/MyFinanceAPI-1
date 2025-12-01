using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Interfaces
{
    public interface IExtratoBancarioService
    {
        Task<IEnumerable<ExtratoBancarioDTO>> GetExtratoBancario(int userId);

        Task<ExtratoBancarioDTO?> GetExtratoBancarioById(int id, int? userId);

        Task<ExtratoImportacaoResultadoDTO> ImportarExtratoAsync(
            Stream arquivoStream,
            string fileName,
            int userId,
            int banco
        );

        Task Remove(int id, int userId);
    }
}
