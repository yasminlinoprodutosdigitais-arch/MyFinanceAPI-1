using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class BancoService(IBancoRepository bancoRepository, IMapper mapper)
    : ServicoCadastro<Banco, BancoDTO>(bancoRepository, mapper), IBancoService
{
    // UX_Banco_UserId_NomeBanco é exata; aqui a regra também ignora maiúsculas e espaços.
    protected override async Task ValidarGravacaoAsync(BancoDTO dto, int userId, int ignorarId)
    {
        if (dto.DiaVencimento is < 1 or > 31)
            throw new RegraNegocioException("O dia do vencimento tem que ser entre 1 e 31.");
        if (dto.DiaFechamento is < 1 or > 31)
            throw new RegraNegocioException("O dia do fechamento tem que ser entre 1 e 31.");
        await ValidarPessoasAsync(dto.PessoasIds, userId);
        if (await bancoRepository.ExisteNomeAsync(userId, dto.NomeBanco ?? string.Empty, ignorarId))
            throw new RegraNegocioException($"Já existe um banco chamado {dto.NomeBanco?.Trim()}.");
    }

    protected override async Task ValidarExclusaoAsync(int id, int userId)
    {
        if (await bancoRepository.EmUsoAsync(id, userId))
            throw new RegraNegocioException("Não é possível excluir este banco, pois ele possui lançamentos ou extratos. Inative o banco em vez de excluí-lo.");
    }

    private async Task ValidarPessoasAsync(IReadOnlyCollection<int>? pessoasIds, int userId)
    {
        if (pessoasIds is { Count: > 0 } && !await bancoRepository.PessoasValidasAsync(userId, pessoasIds))
            throw new RegraNegocioException("Escolha pessoas do seu cadastro (o \"Eu\" não entra).");
    }

    public async Task DefinirCompartilhamentoAsync(int id, int userId, MyFinanceAPI.Application.DTO.Movimentacoes.CompartilhamentoDTO dto)
    {
        await ValidarPessoasAsync(dto.PessoasIds, userId);
        await bancoRepository.DefinirCompartilhamentoAsync(id, userId, dto.Compartilhado, dto.PessoasIds, dto.ConviteDispensado);
    }

    public Task<bool> UpdateSaldo(int bancoId, decimal saldoAtual, int userId) =>
        bancoRepository.UpdateSaldo(bancoId, saldoAtual, userId);
}
