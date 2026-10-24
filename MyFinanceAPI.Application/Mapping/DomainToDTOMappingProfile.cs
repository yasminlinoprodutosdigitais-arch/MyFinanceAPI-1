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
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Conta, ContaDTO>()
            .ReverseMap()
            // 💡 SOLUÇÃO: Ignora a propriedade de navegação 'Category' ao mapear DTO -> Entidade
            .ForMember(dest => dest.Categoria, opt => opt.Ignore());
        CreateMap<Transacao, TransacaoDTO>().ReverseMap();
        CreateMap<Banco, BancoDTO>().ReverseMap();
        // O JSON mantém "tipoMovimentacao"; a entidade/coluna é TipoLista (DB-P1-06).
        CreateMap<Lista, ListaDTO>()
            .ForMember(d => d.TipoMovimentacao, o => o.MapFrom(s => (int)s.TipoLista))
            .ReverseMap()
            .ForMember(d => d.TipoLista, o => o.MapFrom(s => (TipoLista)s.TipoMovimentacao));
        CreateMap<ItemLista, ItemListaDTO>().ReverseMap();
        CreateMap<MovimentacaoDiaria, MovimentacaoDiariaDTO>().ReverseMap();
        CreateMap<ExtratoBancario, ExtratoBancarioDTO>().ReverseMap();
        CreateMap<ExtratoBancarioItem, ExtratoBancarioItemDTO>()
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            // A categoria vem por CategoriaId. Sem o Ignore, o ReverseMap desfaz o achatamento de CategoriaNome
            // (Categoria.Nome desde o [DB] P3) e cria uma Categoria vazia, que o EF tenta inserir ao gravar o item.
            .ForMember(dest => dest.Categoria, opt => opt.Ignore());
        CreateMap<TipoCartao, TipoCartaoDTO>().ReverseMap();
        CreateMap<TipoMovimentacao, TipoMovimentacaoDTO>().ReverseMap();
        CreateMap<PessoaMovimentacao, PessoaMovimentacaoDTO>().ReverseMap();
    }
}
