using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// DB-P1-06: a coluna virou "TipoLista" (enum), mas o JSON da API continua "tipoMovimentacao"
/// para não quebrar o front.
/// </summary>
public class ListaMappingTests
{
    private readonly AutoMapper.IMapper _mapper = MapperFactory.Create();

    [Theory]
    [InlineData(1, TipoLista.Checklist)]
    [InlineData(2, TipoLista.Cronograma)]
    [InlineData(3, TipoLista.Orcamento)]
    public void DTO_tipoMovimentacao_vira_TipoLista_da_entidade(int tipoNoDto, TipoLista esperado)
    {
        var lista = _mapper.Map<Lista>(new ListaDTO { Id = 7, Nome = "Mercado", TipoMovimentacao = tipoNoDto, Ativo = true });

        Assert.Equal(esperado, lista.TipoLista);
        Assert.Equal("Mercado", lista.Nome);
    }

    [Fact]
    public void Entidade_TipoLista_volta_como_tipoMovimentacao_no_DTO()
    {
        var dto = _mapper.Map<ListaDTO>(new Lista { Id = 7, Nome = "Viagem", TipoLista = TipoLista.Orcamento, Ativo = true });

        Assert.Equal(3, dto.TipoMovimentacao);
    }
}
