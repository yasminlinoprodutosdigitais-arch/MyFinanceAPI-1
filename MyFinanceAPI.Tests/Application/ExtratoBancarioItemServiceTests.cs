using Moq;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Criação de item de extrato (lançamento manual): a categoria e o tipo escolhidos no formulário
/// prevalecem sobre o vínculo da pessoa; sem escolha, o item herda o vínculo (decisão de 14/10/2026).
/// Antes a escolha era descartada quando havia pessoa.
/// </summary>
public class ExtratoBancarioItemServiceTests
{
    private const int UserId = 7;
    private const int Mercado = 25;
    private const int Outros = 10;
    private const int Lanche = 27;

    private readonly Mock<IExtratoBancarioItemRepository> _itemRepo = new();
    private readonly Mock<IPessoaMovimentacaoRepository> _pessoaRepo = new();
    private readonly List<PessoaMovimentacao> _pessoasCadastradas = new();
    private readonly List<PessoaMovimentacao> _pessoasCriadas = new();
    private ExtratoBancarioItem? _itemCriado;

    public ExtratoBancarioItemServiceTests()
    {
        _itemRepo.Setup(r => r.CreateAsync(It.IsAny<ExtratoBancarioItem>()))
            .Callback<ExtratoBancarioItem>(i => _itemCriado = i)
            .ReturnsAsync((ExtratoBancarioItem i) => i);
        _pessoaRepo.Setup(r => r.VerificaPossuiPessoa(It.IsAny<string?>(), UserId))
            .Returns((string? nome, int _) => Task.FromResult<IEnumerable<PessoaMovimentacao>>(
                _pessoasCadastradas.Where(p => p.NomePessoa == nome).ToList()));
        _pessoaRepo.Setup(r => r.ObterOuCriarAsync(It.IsAny<PessoaMovimentacao>(), UserId))
            .ReturnsAsync((PessoaMovimentacao p, int _) =>
            {
                p.Id = 900;
                _pessoasCriadas.Add(p);
                return p;
            });
    }

    private Task Criar(ExtratoBancarioItemDTO dto) =>
        new ExtratoBancarioItemService(_itemRepo.Object, _pessoaRepo.Object, MapperFactory.Create()).AddAsync(dto, UserId);

    private static ExtratoBancarioItemDTO Lancamento(string? pessoa, int? categoriaId) => new()
    {
        DataMovimentacao = new DateOnly(2026, 12, 1),
        Valor = 12.34m,
        TipoLancamento = "Saída",
        BancoId = 5,
        TipoCartaoId = 2,
        NomePessoaTransacao = pessoa,
        CategoriaId = categoriaId
    };

    private void PessoaComVinculo(string nome, int? categoriaId, int? tipoId) =>
        _pessoasCadastradas.Add(new PessoaMovimentacao { Id = 88, NomePessoa = nome, CategoriaId = categoriaId, TipoMovimentacaoId = tipoId });

    [Fact]
    public async Task Pessoa_existente_com_categoria_escolhida_usa_a_escolhida_e_mantem_o_tipo_do_vinculo()
    {
        PessoaComVinculo("DALLE PIZZA CIC", Mercado, Lanche);

        await Criar(Lancamento("DALLE PIZZA CIC", Outros));

        Assert.Equal(88, _itemCriado!.PessoaMovimentacaoId);
        Assert.Equal(Outros, _itemCriado.CategoriaId);
        Assert.Equal(Lanche, _itemCriado.TipoMovimentacaoId);
    }

    [Fact]
    public async Task Pessoa_existente_sem_categoria_escolhida_herda_o_vinculo()
    {
        PessoaComVinculo("DALLE PIZZA CIC", Mercado, Lanche);

        await Criar(Lancamento("DALLE PIZZA CIC", null));

        Assert.Equal(Mercado, _itemCriado!.CategoriaId);
        Assert.Equal(Lanche, _itemCriado.TipoMovimentacaoId);
    }

    [Fact]
    public async Task Pessoa_existente_tipo_escolhido_tambem_prevalece()
    {
        PessoaComVinculo("DALLE PIZZA CIC", Mercado, Lanche);
        var dto = Lancamento("DALLE PIZZA CIC", null);
        dto.TipoMovimentacaoId = 3;

        await Criar(dto);

        Assert.Equal(3, _itemCriado!.TipoMovimentacaoId);
        Assert.Equal(Mercado, _itemCriado.CategoriaId);
    }

    [Fact]
    public async Task Pessoa_nova_com_categoria_escolhida_grava_no_item_e_no_vinculo_criado()
    {
        await Criar(Lancamento("PADARIA NOVA", Outros));

        Assert.Equal(900, _itemCriado!.PessoaMovimentacaoId);
        Assert.Equal(Outros, _itemCriado.CategoriaId);
        Assert.Equal(Outros, Assert.Single(_pessoasCriadas).CategoriaId);
    }

    [Fact]
    public async Task Pessoa_nova_sem_categoria_fica_sem_categoria()
    {
        await Criar(Lancamento("PADARIA NOVA", null));

        Assert.Null(_itemCriado!.CategoriaId);
        Assert.Null(_itemCriado.TipoMovimentacaoId);
    }

    [Fact]
    public async Task Pessoa_enviada_como_objeto_com_categoria_escolhida_usa_a_escolhida()
    {
        var dto = Lancamento(null, Outros);
        dto.PessoaMovimentacao = new PessoaMovimentacaoDTO { Id = 88, NomePessoa = "DALLE PIZZA CIC", CategoriaId = Mercado, TipoMovimentacaoId = Lanche };

        await Criar(dto);

        Assert.Equal(88, _itemCriado!.PessoaMovimentacaoId);
        Assert.Equal(Outros, _itemCriado.CategoriaId);
        Assert.Equal(Lanche, _itemCriado.TipoMovimentacaoId);
    }

    [Fact]
    public async Task Sem_pessoa_mantem_a_categoria_escolhida()
    {
        await Criar(Lancamento(null, Outros));

        Assert.Null(_itemCriado!.PessoaMovimentacaoId);
        Assert.Equal(Outros, _itemCriado.CategoriaId);
    }

    [Fact]
    public async Task O_vinculo_existente_nao_e_alterado()
    {
        PessoaComVinculo("DALLE PIZZA CIC", Mercado, Lanche);

        await Criar(Lancamento("DALLE PIZZA CIC", Outros));

        Assert.Equal(Mercado, _pessoasCadastradas[0].CategoriaId);
        _pessoaRepo.Verify(r => r.ObterOuCriarAsync(It.IsAny<PessoaMovimentacao>(), It.IsAny<int>()), Times.Never);
    }
}
