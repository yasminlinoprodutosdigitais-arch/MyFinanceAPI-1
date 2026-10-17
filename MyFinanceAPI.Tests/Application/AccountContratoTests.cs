using System.Text.Json;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// BE-P3-09: AccountDTO.Categoryid virou CategoryId — no JSON, "categoryid" passou a "categoryId",
/// como no TransactionDTO. O front foi ajustado junto.
/// </summary>
public class AccountContratoTests
{
    private readonly AutoMapper.IMapper _mapper = MapperFactory.Create();

    [Fact]
    public void Categoria_da_conta_vai_e_volta_entre_entidade_e_DTO()
    {
        var dto = _mapper.Map<AccountDTO>(new Account { Id = 1, Name = "Internet", CategoryId = 4 });
        Assert.Equal(4, dto.CategoryId);

        Assert.Equal(4, _mapper.Map<Account>(dto).CategoryId);
    }

    [Fact]
    public void JSON_da_conta_usa_categoryId()
    {
        var web = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var json = JsonSerializer.Serialize(new AccountDTO { Id = 1, CategoryId = 4 }, web);
        Assert.Contains("\"categoryId\":4", json);
        Assert.DoesNotContain("\"categoryid\"", json);

        Assert.Equal(4, JsonSerializer.Deserialize<AccountDTO>("{\"categoryId\":4}", web)!.CategoryId);
    }
}
