using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class PessoaMovimentacaoService(IPessoaMovimentacaoRepository pessoaMovimentacaoRepository, IExtratoBancarioItemService extratoBancarioItemService, IMapper mapper)
    : ServicoCadastro<PessoaMovimentacao, PessoaMovimentacaoDTO>(pessoaMovimentacaoRepository, mapper), IPessoaMovimentacaoService
{
    protected override async Task ValidarExclusaoAsync(int id, int userId)
    {
        if (await pessoaMovimentacaoRepository.EmUsoAsync(id, userId))
            throw new RegraNegocioException("Não é possível excluir esta pessoa, pois ela está vinculada a itens de extrato.");
    }

    /// <summary>Com MesAtualizacao ("AAAA-MM"), leva o novo nome, categoria e tipo aos itens do extrato daquele mês.</summary>
    public override async Task AtualizarAsync(PessoaMovimentacaoDTO dto, int userId)
    {
        if (!string.IsNullOrEmpty(dto.MesAtualizacao))
        {
            var dataSeparada = dto.MesAtualizacao.Split('-');
            var year = int.Parse(dataSeparada[0]);
            var month = int.Parse(dataSeparada[1]);

            var extratos = await extratoBancarioItemService.GetByMonthAsync(userId, year, month);
            foreach (var extrato in extratos.Where(x => x.UsuarioId == userId && x.PessoaMovimentacaoId == dto.Id).ToList())
            {
                extrato.NomePessoaTransacao = dto.NomePessoa;
                extrato.CategoriaId = dto.CategoriaId;
                extrato.TipoMovimentacaoId = dto.TipoMovimentacaoId;
                await extratoBancarioItemService.UpdateAsync(extrato, userId);
            }
        }

        await base.AtualizarAsync(dto, userId);
    }
}
