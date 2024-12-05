using AutoMapper;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Mapping;
public class DomainToDTOMappingProfile : Profile
{
    public DomainToDTOMappingProfile()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
        CreateMap<Account, AccountDTO>().ReverseMap();
        CreateMap<AccountGrouping, AccountGroupingDTO>().ReverseMap();
        CreateMap<MonthlyUpdate, MonthlyUpdateDTO>().ReverseMap();
    }
}
