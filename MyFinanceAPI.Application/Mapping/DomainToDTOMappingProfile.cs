using AutoMapper;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Application.Mapping;

public class DomainToDTOMappingProfile : Profile
{
    public DomainToDTOMappingProfile()
    {
        // Mapeamento de CategoryDTO para Category
        CreateMap<CategoryDTO, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignora o mapeamento do Id
    }
}

