using System.Text.Json;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// BE-P3-09 e DB-P3 (subprojeto 3): a categoria da conta viaja como "categoriaId" — antes "categoryid"
/// e depois "categoryId". O front foi ajustado junto.
/// </summary>
public class ContaContratoTests
{
    private readonly AutoMapper.IMapper _mapper = MapperFactory.Create();

    [Fact]
    public void Categoria_da_conta_vai_e_volta_entre_entidade_e_DTO()
    {
        var dto = _mapper.Map<ContaDTO>(new Conta { Id = 1, Nome = "Internet", CategoriaId = 4 });
        Assert.Equal(4, dto.CategoriaId);

        Assert.Equal(4, _mapper.Map<Conta>(dto).CategoriaId);
    }

    [Fact]
    public void JSON_da_conta_usa_categoriaId()
    {
        var web = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var json = JsonSerializer.Serialize(new ContaDTO { Id = 1, CategoriaId = 4 }, web);
        Assert.Contains("\"categoriaId\":4", json);
        Assert.DoesNotContain("\"categoryId\"", json);

        Assert.Equal(4, JsonSerializer.Deserialize<ContaDTO>("{\"categoriaId\":4}", web)!.CategoriaId);
    }
}
