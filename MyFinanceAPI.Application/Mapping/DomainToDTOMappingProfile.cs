using AutoMapper;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.DTO;

namespace MyFinanceAPI.Application.Mapping;
public class DomainToDTOMappingProfile : Profile
{
    public DomainToDTOMappingProfile()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
        CreateMap<Account, AccountDTO>()
            .ReverseMap()
            // 💡 SOLUÇÃO: Ignora a propriedade de navegação 'Category' ao mapear DTO -> Entidade
            .ForMember(dest => dest.Category, opt => opt.Ignore());
        CreateMap<AccountGrouping, AccountGroupingDTO>().ReverseMap();
        CreateMap<Transaction, TransactionDTO>().ReverseMap();
        CreateMap<Banco, BancoDTO>().ReverseMap();
        CreateMap<Lista, ListaDTO>().ReverseMap();
        CreateMap<ItemLista, ItemListaDTO>().ReverseMap();
        CreateMap<MovimentacaoDiaria, MovimentacaoDiariaDTO>().ReverseMap();
        CreateMap<TipoCartao, TipoCartaoDTO>().ReverseMap();
        CreateMap<TipoMovimentacao, TipoMovimentacaoDTO>().ReverseMap();
    }
}
