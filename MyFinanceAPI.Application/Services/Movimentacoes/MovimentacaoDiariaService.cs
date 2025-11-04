using System;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class MovimentacaoDiariaService : IMovimentacaoDiariaService
{
    private readonly IMovimentacaoDiariaRepository _movimentacaoDiariaRepository;
    private readonly IBancoRepository _bancoRepository;
    private readonly IMapper _mapper;

    public MovimentacaoDiariaService(IMovimentacaoDiariaRepository MovimentacaoDiariaRepository, IBancoRepository bancoRepository, IMapper mapper)
    {
        _movimentacaoDiariaRepository = MovimentacaoDiariaRepository;
        _bancoRepository = bancoRepository;
        _mapper = mapper;
    }
    public async Task Add(MovimentacaoDiariaDTO MovimentacaoDiariaDTO, int userId)
    {
        var MovimentacaoDiaria = _mapper.Map<MovimentacaoDiaria>(MovimentacaoDiariaDTO);
        MovimentacaoDiaria.UserId = userId;
        await _movimentacaoDiariaRepository.Create(MovimentacaoDiaria);
        if (MovimentacaoDiariaDTO.BancoId != null && MovimentacaoDiariaDTO.TipoCartaoId != null)
        {
            await this.AtualizaSaldoBanco(MovimentacaoDiariaDTO, userId);
        }
    }

    private async Task<bool> AtualizaSaldoBanco(MovimentacaoDiariaDTO MovimentacaoDiariaDTO, int userId, bool ehAtualizacao = false, decimal valorAntigo = 0)
    {
        var banco = await _bancoRepository.GetBancoById(MovimentacaoDiariaDTO.BancoId, userId);

        decimal saldoBase = banco.SaldoInicial;
        decimal valorNovo = MovimentacaoDiariaDTO.Valor;
        decimal valorAjuste = 0;

        if (ehAtualizacao)
        {
            var valorLancadoAnteriormente = await _movimentacaoDiariaRepository.GetMovimentacaoDiariaById(MovimentacaoDiariaDTO.Id, userId);

            if (MovimentacaoDiariaDTO.TipoLancamento != valorLancadoAnteriormente.TipoLancamento)
            {
                if (valorLancadoAnteriormente.TipoLancamento == "Desconto")
                {
                    saldoBase += valorAntigo;
                }
                else
                {
                    saldoBase -= valorAntigo;
                }
                valorAjuste = valorNovo;
            }
            else
            {
                valorAjuste = valorNovo - valorAntigo;

                if (valorAjuste == 0)
                {
                    return false;
                }
            }
        }
        else
        {
            valorAjuste = valorNovo;
        }

        if (MovimentacaoDiariaDTO.TipoLancamento == "Desconto")
        {
            saldoBase -= valorAjuste;
        }
        else
        {
            saldoBase += valorAjuste;
        }

        decimal novoSaldo = saldoBase;
        return await _bancoRepository.UpdateSaldo(MovimentacaoDiariaDTO.BancoId, novoSaldo, userId);
    }

    public async Task<IEnumerable<MovimentacaoDiariaDTO>> GetMovimentacaoDiaria(int userId)
    {
        var movimentacoDiaria = await _movimentacaoDiariaRepository.GetMovimentacaoDiaria(userId);
        return _mapper.Map<IEnumerable<MovimentacaoDiariaDTO>>(movimentacoDiaria);
    }

    public async Task<MovimentacaoDiariaDTO> GetMovimentacaoDiariaById(int id, int userId)
    {
        var MovimentacaoDiaria = await _movimentacaoDiariaRepository.GetMovimentacaoDiariaById(id, userId);
        return _mapper.Map<MovimentacaoDiariaDTO>(MovimentacaoDiaria);
    }


    public async Task<IEnumerable<MovimentacaoDiariaDTO>> GetMovimentacaoByDate(DateTime date, int userId)
    {
        var Transaction = await _movimentacaoDiariaRepository.GetMovimentacaoByDate(date, userId);
        return _mapper.Map<IEnumerable<MovimentacaoDiariaDTO>>(Transaction);
    }


    public async Task Remove(int id, int userId)
    {
        MovimentacaoDiariaDTO dto = _mapper.Map<MovimentacaoDiariaDTO>(await _movimentacaoDiariaRepository.GetMovimentacaoDiariaById(id, userId));
        dto.Valor = 0;
        var valorLancadoAnteriormente = await _movimentacaoDiariaRepository.GetValorLancadoAnteriormente(dto.Id, userId);

        await this.AtualizaSaldoBanco(dto, userId, true, valorLancadoAnteriormente);

        await _movimentacaoDiariaRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(MovimentacaoDiariaDTO dto, int userId)
    {
        var valorLancadoAnteriormente = await _movimentacaoDiariaRepository.GetValorLancadoAnteriormente(dto.Id, userId);

        var account = _mapper.Map<MovimentacaoDiaria>(dto);
        await _movimentacaoDiariaRepository.UpdateAsync(account, userId);
        if (dto.BancoId != null && dto.TipoCartaoId != null)
        {
            await this.AtualizaSaldoBanco(dto, userId, true, valorLancadoAnteriormente);
        }
        return true;
    }

}
