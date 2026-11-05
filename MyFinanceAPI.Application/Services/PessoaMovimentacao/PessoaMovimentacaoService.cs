using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class PessoaMovimentacaoService(IPessoaMovimentacaoRepository pessoaMovimentacaoRepository, IExtratoBancarioItemService extratoBancarioItemService, IMapper mapper)
    : ServicoCadastro<PessoaMovimentacao, PessoaMovimentacaoDTO>(pessoaMovimentacaoRepository, mapper), IPessoaMovimentacaoService
{
    /// <summary>Nome da pessoa "Eu" gravado (normalizado).</summary>
    private const string NomeTitular = "EU";

    /// <summary>A lista sempre traz o "Eu" (card #45): na primeira vez, ele é criado aqui.</summary>
    public override async Task<IEnumerable<PessoaMovimentacaoDTO>> ListarAsync(int userId)
    {
        await pessoaMovimentacaoRepository.ObterTitularAsync(userId);
        return await base.ListarAsync(userId);
    }

    /// <summary>Titular só nasce pelo ObterTitularAsync; o que vier do front é ignorado.</summary>
    protected override PessoaMovimentacao ParaEntidade(PessoaMovimentacaoDTO dto)
    {
        var pessoa = base.ParaEntidade(dto);
        pessoa.EhTitular = false;
        return pessoa;
    }

    protected override async Task ValidarGravacaoAsync(PessoaMovimentacaoDTO dto, int userId, int ignorarId)
    {
        if (ignorarId == 0)
            return;

        var atual = await pessoaMovimentacaoRepository.ObterPorIdAsync(ignorarId, userId);
        if (atual is { EhTitular: true } && !string.Equals(dto.NomePessoa?.Trim(), NomeTitular, StringComparison.OrdinalIgnoreCase))
            throw new RegraNegocioException("A pessoa \"Eu\" não pode ser renomeada.");
    }

    protected override async Task ValidarExclusaoAsync(int id, int userId)
    {
        var pessoa = await pessoaMovimentacaoRepository.ObterPorIdAsync(id, userId);
        if (pessoa is { EhTitular: true })
            throw new RegraNegocioException("A pessoa \"Eu\" não pode ser excluída.");

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

            // GetByMonthAsync já traz só os itens do usuário.
            var extratos = await extratoBancarioItemService.GetByMonthAsync(userId, year, month);
            foreach (var extrato in extratos.Where(x => x.PessoaMovimentacaoId == dto.Id).ToList())
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
