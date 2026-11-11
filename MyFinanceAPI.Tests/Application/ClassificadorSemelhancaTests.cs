using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Application;

/// <summary>Card #79: regra pelo nome do extrato e a classificação por semelhança (nome parecido com uma regra "igual a").</summary>
public class ClassificadorSemelhancaTests
{
    private static RegraClassificacao Regra(int id, string texto, string campo = CamposRegra.NomeExtrato, string operador = OperadoresRegra.Igual,
        int ordem = 0, bool ativa = true, int? banco = null) =>
        new() { Id = id, Texto = texto, Campo = campo, Operador = operador, Ordem = ordem, Ativa = ativa, BancoId = banco, CategoriaId = 1 };

    [Fact]
    public void Regra_nome_do_extrato_igual_compara_a_chave_do_lancamento()
    {
        var regra = Regra(1, "IFOOD");
        Assert.Same(regra, ClassificadorRegras.Encontrar([regra], "IFOOD *RESTAURANTE 123", null, null));
        Assert.Same(regra, ClassificadorRegras.Encontrar([regra], "Compra - Ifood", null, null));
        Assert.Null(ClassificadorRegras.Encontrar([regra], "IFOOD MERCADO", null, null));
    }

    [Fact]
    public void Regra_nome_do_extrato_contem_procura_na_chave()
    {
        var regra = Regra(1, "CIRCUITO", operador: OperadoresRegra.Contem);
        Assert.Same(regra, ClassificadorRegras.Encontrar([regra], "Pix", "SUPERMERCADO CIRCUITO", null));
    }

    [Fact]
    public void Semelhanca_acha_regra_igual_com_nome_parecido()
    {
        var bar = Regra(1, "BAR DOIS IRMAOS");
        Assert.Same(bar, ClassificadorRegras.EncontrarPorSemelhanca([bar], "Compra - BARDOISIRMAOS", null, null));
    }

    [Fact]
    public void Semelhanca_vale_para_regra_antiga_de_pessoa_igual()
    {
        var farmacia = Regra(1, "FARMACIA LED FARMA LT", campo: CamposRegra.Pessoa);
        Assert.Same(farmacia, ClassificadorRegras.EncontrarPorSemelhanca([farmacia], "Pix", "FARMACIA LED FARMA LTDA", null));
    }

    [Fact]
    public void Semelhanca_ignora_contem_desligada_e_outra_conta()
    {
        RegraClassificacao[] regras =
        [
            Regra(1, "TELEFONICA", operador: OperadoresRegra.Contem),
            Regra(2, "TELEFONICA", ativa: false),
            Regra(3, "TELEFONICA", banco: 9)
        ];
        Assert.Null(ClassificadorRegras.EncontrarPorSemelhanca(regras, "TELEFONICA BRAS", null, 5));
    }

    [Fact]
    public void Semelhanca_empate_vale_a_de_cima()
    {
        var baixo = Regra(1, "GOOGLE CLOUD", ordem: 2);
        var cima = Regra(2, "GOOGLE CLOUD BR", ordem: 0);
        Assert.Same(cima, ClassificadorRegras.EncontrarPorSemelhanca([baixo, cima], "GOOGLE CLOUD BRASIL", null, null));
    }

    [Fact]
    public void Semelhanca_sem_nome_nao_acha()
    {
        Assert.Null(ClassificadorRegras.EncontrarPorSemelhanca([Regra(1, "BAR")], null, "Eu", null));
    }
}
