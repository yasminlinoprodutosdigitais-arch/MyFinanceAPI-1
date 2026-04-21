using System;
using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Application.Services;

public class PessoaMovimentacaoService : IPessoaMovimentacaoService
{
    private readonly IPessoaMovimentacaoRepository _PessoaMovimentacaoRepository;
    private readonly IExtratoBancarioItemService _extratoBancarioItemService;
    private readonly IMapper _mapper;

    public PessoaMovimentacaoService(IPessoaMovimentacaoRepository PessoaMovimentacaoRepository, IExtratoBancarioItemService extratoBancarioItemService, IMapper mapper)
    {
        _PessoaMovimentacaoRepository = PessoaMovimentacaoRepository;
        _extratoBancarioItemService = extratoBancarioItemService;
        _mapper = mapper;
    }
    public async Task Add(PessoaMovimentacaoDTO PessoaMovimentacaoDTO, int userId)
    {
        var PessoaMovimentacao = _mapper.Map<PessoaMovimentacao>(PessoaMovimentacaoDTO);
        PessoaMovimentacao.UserId = userId;
        await _PessoaMovimentacaoRepository.Create(PessoaMovimentacao, userId);
    }

    public async Task<IEnumerable<PessoaMovimentacaoDTO>> GetPessoaMovimentacao(int userId)
    {
        var movimentacoDiaria = await _PessoaMovimentacaoRepository.GetPessoaMovimentacao(userId);
        return _mapper.Map<IEnumerable<PessoaMovimentacaoDTO>>(movimentacoDiaria);
    }

    public async Task<PessoaMovimentacaoDTO> GetPessoaMovimentacaoById(int id, int userId)
    {
        var PessoaMovimentacao = await _PessoaMovimentacaoRepository.GetPessoaMovimentacaoById(id, userId);
        return _mapper.Map<PessoaMovimentacaoDTO>(PessoaMovimentacao);
    }

    public async Task Remove(int id, int userId)
    {
        await _PessoaMovimentacaoRepository.Remove(id, userId);
    }

    public async Task<bool> UpdateAsync(PessoaMovimentacaoDTO dto, int userId)
    {
        var PessoaMovimentacao = _mapper.Map<PessoaMovimentacao>(dto);
        if (dto.MesAtualizacao != null && dto.MesAtualizacao != "")
        {
            var dataSeparada = dto.MesAtualizacao.ToString().Split('-');
            var year = int.Parse(dataSeparada[0]);
            var month = int.Parse(dataSeparada[1]);

            var extratos = await _extratoBancarioItemService.GetByMonthAsync(userId, year, month);
            extratos = extratos.Where((x) => x.UserId == userId && x.PessoaMovimentacaoId == dto.Id).ToArray();
            foreach (var extrato in extratos)
            {
                extrato.NomePessoaTransacao = dto.NomePessoa;
                extrato.CategoriaId = dto.CategoriaId;
                extrato.TipoMovimentacaoId = dto.TipoMovimentacaoId;
                await _extratoBancarioItemService.UpdateAsync(extrato, userId);
            }
        }
        await _PessoaMovimentacaoRepository.UpdateAsync(PessoaMovimentacao, userId);
        return true;
    }

}
