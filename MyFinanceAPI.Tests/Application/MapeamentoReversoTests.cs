using AutoMapper;
using AutoMapper.Internal;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// DTO → entidade não pode criar entidade relacionada. O ReverseMap desfaz o achatamento (CategoriaNome →
/// Categoria.Nome) e cria uma Categoria vazia; ao gravar, o EF tenta inseri-la e o banco recusa
/// (lançamento manual com 500 depois do [DB] P3, que renomeou Category.Name para Categoria.Nome).
/// </summary>
public class MapeamentoReversoTests
{
    private static readonly IMapper Mapper = MapperFactory.Create();

    public static IEnumerable<object[]> MapeamentosParaEntidade() =>
        Mapper.ConfigurationProvider.Internal().GetAllTypeMaps()
            .Where(m => m.DestinationType.Namespace?.StartsWith(typeof(BaseEntity).Namespace!) == true
                        && m.SourceType.Namespace?.Contains(".DTO") == true)
            .Select(m => new object[] { m.SourceType, m.DestinationType });

    [Theory]
    [MemberData(nameof(MapeamentosParaEntidade))]
    public void Dto_vazio_nao_cria_entidade_relacionada(Type dto, Type entidade)
    {
        var mapeada = Mapper.Map(Activator.CreateInstance(dto), dto, entidade);

        var criadas = entidade.GetProperties()
            .Where(p => p.PropertyType.IsClass
                        && p.PropertyType.Namespace?.StartsWith(typeof(BaseEntity).Namespace!) == true
                        && p.GetValue(mapeada) != null)
            .Select(p => p.Name);

        Assert.Empty(criadas);
    }
}
