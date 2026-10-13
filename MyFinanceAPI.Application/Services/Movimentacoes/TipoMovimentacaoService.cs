using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class TipoMovimentacaoService(ITipoMovimentacaoRepository tipoMovimentacaoRepository, IMapper mapper)
    : ServicoCadastro<TipoMovimentacao, TipoMovimentacaoDTO>(tipoMovimentacaoRepository, mapper), ITipoMovimentacaoService
{
    protected override async Task ValidarExclusaoAsync(int id, int userId)
    {
        if (await tipoMovimentacaoRepository.EmUsoAsync(id, userId))
            throw new RegraNegocioException("Não é possível excluir este tipo de movimentação, pois ele está em uso por lançamentos ou pessoas.");
    }
}
