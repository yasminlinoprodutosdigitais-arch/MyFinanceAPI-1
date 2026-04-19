using AutoMapper;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Application.DTO.Extrato;

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
        CreateMap<ExtratoBancario, ExtratoBancarioDTO>().ReverseMap();
        CreateMap<ExtratoBancarioItem, ExtratoBancarioItemDTO>()
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<TipoCartao, TipoCartaoDTO>().ReverseMap();
        CreateMap<TipoMovimentacao, TipoMovimentacaoDTO>().ReverseMap();
        CreateMap<VinculoTipoMovimentacao, VinculoTipoMovimentacaoDTO>().ReverseMap();
        CreateMap<PessoaMovimentacao, PessoaMovimentacaoDTO>().ReverseMap();
    }
}
