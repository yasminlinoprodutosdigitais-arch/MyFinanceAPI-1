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
        // Resumo é calculado na listagem; arquivada_em, feito_em, série e origens são gravados pelo serviço, não pela entrada.
        CreateMap<Lista, ListaDTO>()
            .ForMember(d => d.Resumo, o => o.Ignore())
            .ReverseMap()
            .ForMember(d => d.ArquivadaEm, o => o.Ignore())
            .ForMember(d => d.SerieId, o => o.Ignore())
            .ForMember(d => d.ListaOrigemId, o => o.Ignore());
        CreateMap<ItemLista, ItemListaDTO>()
            .ReverseMap()
            .ForMember(d => d.FeitoEm, o => o.Ignore())
            .ForMember(d => d.ItemOrigemId, o => o.Ignore())
            .ForMember(d => d.Lista, o => o.Ignore());
        CreateMap<MovimentacaoDiaria, MovimentacaoDiariaDTO>().ReverseMap();
        CreateMap<ExtratoBancario, ExtratoBancarioDTO>().ReverseMap();
        CreateMap<ExtratoBancarioItem, ExtratoBancarioItemDTO>()
            .ForMember(d => d.TemNota, o => o.MapFrom(s => s.NotaFiscal != null))
            .ReverseMap()
            .ForMember(dest => dest.NotaFiscal, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            // A categoria vem por CategoriaId. Sem o Ignore, o ReverseMap desfaz o achatamento de CategoriaNome
            // (Categoria.Nome desde o [DB] P3) e cria uma Categoria vazia, que o EF tenta inserir ao gravar o item.
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Partes, opt => opt.Ignore());
        CreateMap<ExtratoItemParte, ExtratoItemParteDTO>()
            .ForMember(d => d.CategoriaNome, o => o.MapFrom(s => s.Categoria != null ? s.Categoria.Nome : null))
            .ForMember(d => d.PessoaNome, o => o.MapFrom(s => s.PessoaMovimentacao != null ? s.PessoaMovimentacao.NomePessoa : null));
        CreateMap<TipoCartao, TipoCartaoDTO>().ReverseMap();
        CreateMap<TipoMovimentacao, TipoMovimentacaoDTO>().ReverseMap();
        CreateMap<PessoaMovimentacao, PessoaMovimentacaoDTO>().ReverseMap();
        CreateMap<RegraClassificacao, RegraClassificacaoDTO>()
            .ForMember(d => d.CategoriaNome, o => o.MapFrom(s => s.Categoria != null ? s.Categoria.Nome : null))
            .ForMember(d => d.TipoMovimentacaoNome, o => o.MapFrom(s => s.TipoMovimentacao != null ? s.TipoMovimentacao.NomeTipoMovimentacao : null))
            .ForMember(d => d.PessoaNome, o => o.MapFrom(s => s.PessoaMovimentacao != null ? s.PessoaMovimentacao.NomePessoa : null))
            .ForMember(d => d.BancoNome, o => o.MapFrom(s => s.Banco != null ? s.Banco.NomeBanco : null))
            .ReverseMap()
            .ForMember(e => e.Categoria, o => o.Ignore())
            .ForMember(e => e.TipoMovimentacao, o => o.Ignore())
            .ForMember(e => e.PessoaMovimentacao, o => o.Ignore())
            .ForMember(e => e.Banco, o => o.Ignore());
    }
}
