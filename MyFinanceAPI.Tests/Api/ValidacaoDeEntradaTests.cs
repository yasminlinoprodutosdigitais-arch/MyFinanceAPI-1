using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyFinanceAPI.Api.Erros;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Api;

/// <summary>
/// BE-P4-17: os DTOs de entrada recusam, com 400 e <c>{ message }</c> em português, o mesmo que o banco
/// recusaria pelas CHECKs e pelos NOT NULL. Nada que o banco aceita passa a ser recusado: o banco fica como rede.
/// </summary>
public class ValidacaoDeEntradaTests
{
    private static string? Erro(object dto)
    {
        var erros = new List<ValidationResult>();
        if (Validator.TryValidateObject(dto, new ValidationContext(dto), erros, validateAllProperties: true)) return null;

        var estado = new ModelStateDictionary();
        foreach (var erro in erros) estado.AddModelError(erro.MemberNames.FirstOrDefault() ?? "", erro.ErrorMessage!);
        var resposta = Assert.IsType<BadRequestObjectResult>(ModeloInvalido.Resposta(new ActionContext(new DefaultHttpContext(), new(), new(), estado)));
        return Assert.IsType<MensagemErro>(resposta.Value).Message;
    }

    private static TransacaoDTO Transacao(string situacao = StatusTransacao.Pendente) =>
        new() { Nome = "Luz", Situacao = situacao, Data = new DateOnly(2026, 10, 10) };

    [Fact]
    public void Transacao_com_situacao_fora_do_dominio_e_recusada()
    {
        Assert.Equal("Dados inválidos. Situacao aceita só: PENDENTE, AGUARDANDO, PAGO NO PRAZO, PAGO ATRASADO.",
            Erro(Transacao("PAGO")));
        Assert.Null(Erro(Transacao(StatusTransacao.PagoAtrasado)));
    }

    [Fact]
    public void Transacao_sem_nome_e_recusada_em_portugues()
    {
        Assert.Equal("Dados inválidos. Nome é obrigatório.", Erro(new TransacaoDTO { Nome = "", Situacao = StatusTransacao.Pendente }));
    }

    [Fact]
    public void Conta_sem_dias_de_operacao_e_recusada_em_portugues()
    {
        // Sem o atributo, o [Required] implícito do ASP.NET (referência não anulável) respondia em inglês.
        Assert.Equal("Dados inválidos. DataOperacao é obrigatório.",
            Erro(new ContaDTO { Nome = "Aluguel", Ativo = StatusAtivo.Ativo, DataOperacao = null! }));
    }

    [Fact]
    public void Dias_de_vencimento_fora_de_1_a_31_sao_recusados()
    {
        var transacao = Transacao();
        transacao.DiasVencimento = [10, 32];
        Assert.Equal("Dados inválidos. DiasVencimento: cada dia deve estar entre 1 e 31.", Erro(transacao));

        var conta = new ContaDTO { Nome = "Aluguel", Ativo = StatusAtivo.Ativo, DataOperacao = [0] };
        Assert.Equal("Dados inválidos. DataOperacao: cada dia deve estar entre 1 e 31.", Erro(conta));

        conta.DataOperacao = [1, 31];
        Assert.Null(Erro(conta));
    }

    [Fact]
    public void Ativo_aceita_so_0_e_1()
    {
        Assert.Equal("Dados inválidos. Ativo aceita só: 0, 1.",
            Erro(new ContaDTO { Nome = "Aluguel", Ativo = 2, DataOperacao = [5] }));
        Assert.Equal("Dados inválidos. Ativo aceita só: 0, 1.",
            Erro(new CategoriaDTO { Nome = "Casa", Subcategoria = "Luz", Ativo = 3 }));
    }

    [Fact]
    public void Natureza_da_categoria_vai_de_0_a_5()
    {
        Assert.Equal("Dados inválidos. NaturezaOperacao deve estar entre 0 e 5.",
            Erro(new CategoriaDTO { Nome = "Casa", Subcategoria = "Luz", Ativo = 1, NaturezaOperacao = 6 }));
        Assert.Null(Erro(new CategoriaDTO { Nome = "Casa", Subcategoria = "Luz", Ativo = 1, NaturezaOperacao = 5 }));
    }

    [Fact]
    public void Item_de_lista_valida_situacao_e_quantidade()
    {
        Assert.Equal("Dados inválidos. Situacao aceita só: PENDENTE, AGUARDANDO, OK.",
            Erro(new ItemListaDTO { Descricao = "Pão", Situacao = "FEITO" }));
        Assert.Equal("Dados inválidos. Quantidade não pode ser menor que 0.",
            Erro(new ItemListaDTO { Descricao = "Pão", Quantidade = -1 }));
        Assert.Null(Erro(new ItemListaDTO { Descricao = "Pão", Quantidade = 0, Situacao = null }));
    }

    [Fact]
    public void Tipo_de_lista_fora_do_enum_e_recusado()
    {
        Assert.Equal("Dados inválidos. TipoMovimentacao aceita só: " + string.Join(", ", Enum.GetValues<TipoLista>().Select(v => (int)v)) + ".",
            Erro(new ListaDTO { Nome = "Feira", TipoMovimentacao = 99 }));
        Assert.Null(Erro(new ListaDTO { Nome = "Feira", TipoMovimentacao = (int)Enum.GetValues<TipoLista>()[0] }));
    }

    [Fact]
    public void Lancamento_do_extrato_aceita_so_entrada_e_saida()
    {
        Assert.Equal("Dados inválidos. TipoLancamento aceita só: Entrada, Saída.",
            Erro(new ExtratoBancarioItemDTO { TipoLancamento = "Saida" }));
        Assert.Null(Erro(new ExtratoBancarioItemDTO { TipoLancamento = TipoLancamentoExtrato.Saida }));
    }

    [Fact]
    public void Meta_do_tipo_de_movimentacao_nao_pode_ser_negativa()
    {
        Assert.Equal("Dados inválidos. ValorMeta não pode ser menor que 0.",
            Erro(new TipoMovimentacaoDTO { NomeTipoMovimentacao = "Mercado", Descricao = "Mensal", ValorMeta = -1 }));
    }

    [Fact]
    public void Item_do_extrato_nao_recebe_usuario_pelo_corpo()
    {
        Assert.Null(typeof(ExtratoBancarioItemDTO).GetProperty("UsuarioId"));
    }
}
