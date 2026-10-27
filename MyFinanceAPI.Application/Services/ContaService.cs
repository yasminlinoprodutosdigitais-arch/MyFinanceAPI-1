using System;
using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class ContaService : IContaService
{
    private IContaRepository _accountRepository;
    private IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;


    public ContaService(IContaRepository AccountRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _accountRepository = AccountRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Parcelas da conta parcelada dentro do CHECK do banco (contas_parcelas_check): total ≥ 1 e parcela atual
    /// de 0 (nenhuma paga) até o total. Barrar aqui troca o erro genérico de constraint por uma mensagem clara (card #22).
    /// </summary>
    private static void ValidarParcelas(ContaDTO conta)
    {
        if (!conta.EhParcelado)
            return;

        if (conta.QuantidadeParcelas is not int total || total < 1)
            throw new RegraNegocioException("Conta parcelada precisa de ao menos 1 parcela no total.");
        if (conta.ParcelaAtual is not int atual || atual < 0 || atual > total)
            throw new RegraNegocioException($"A parcela atual deve estar entre 0 e {total}.");
    }

    // Conta e vencimentos valem juntos (BE-P1-05).
    public Task<ContaDTO> Add(ContaDTO accountDTO, int userId) => _unitOfWork.ExecuteAsync(async () =>
    {
        ValidarParcelas(accountDTO);
        var account = _mapper.Map<Conta>(accountDTO);
        account.UsuarioId = userId;

        var conta = await _accountRepository.Create(account);

        var vencimentos = accountDTO.DataOperacao.Select(d => new ContaVencimento
        {
            ContaId = conta.Id,
            Dia = d,
            UsuarioId = userId
        }).ToList();

        await _accountRepository.CreateContaVencimento(vencimentos);
        return _mapper.Map<ContaDTO>(conta);
    });

    public Task Update(ContaDTO accountDTO, int userId) => _unitOfWork.ExecuteAsync(async () =>
    {
        ValidarParcelas(accountDTO);
        var account = _mapper.Map<Conta>(accountDTO);

        await _accountRepository.Update(account, userId);

        await _accountRepository.RemoveContaVencimento(account.Id, userId);

        if (accountDTO.DataOperacao != null)
        {
            // 🔥 adicionar novos
            var vencimentos = accountDTO.DataOperacao.Select(d => new ContaVencimento
            {
                ContaId = account.Id,
                Dia = d,
                UsuarioId = userId
            }).ToList();

            await _accountRepository.CreateContaVencimento(vencimentos);
        }
    });

    public async Task UpdateParcela(ContaDTO accountDTO, int userId)
    {
        var account = _mapper.Map<Conta>(accountDTO);
        await _accountRepository.Update(account, userId);
    }
    public async Task Remove(int id, int userId)
    {
        if (await _accountRepository.EmUsoAsync(id, userId))
            throw new RegraNegocioException(
                "Não é possível excluir essa conta, pois ela já tem lançamentos. Inative-a para ela parar de gerar lançamentos.");

        await _accountRepository.Remove(id, userId);
    }

    public async Task<IEnumerable<ContaDTO>> GetAccounts(int userId)
    {
        var accounts = await _accountRepository.GetAccounts(userId);
        return _mapper.Map<IEnumerable<ContaDTO>>(accounts);
    }

    public async Task<IEnumerable<ContaDTO>> GetContasAtivas(int userId)
    {
        var accounts = await _accountRepository.GetContasAtivas(userId);
        return _mapper.Map<IEnumerable<ContaDTO>>(accounts);
    }

    public async Task<ContaDTO> GetAccountById(int id, int userId)
    {
        var account = await _accountRepository.GetAccountById(id, userId);
        return _mapper.Map<ContaDTO>(account);
    }

}
