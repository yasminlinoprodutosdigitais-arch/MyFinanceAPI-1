using AutoMapper;
using MyFinanceAPI.Application.Mapping;

namespace MyFinanceAPI.Tests.Application;

/// <summary>AutoMapper real do projeto: os testes também exercitam os mapeamentos usados pelos services.</summary>
internal static class MapperFactory
{
    public static IMapper Create() =>
        new MapperConfiguration(cfg => cfg.AddProfile<DomainToDTOMappingProfile>()).CreateMapper();
}
