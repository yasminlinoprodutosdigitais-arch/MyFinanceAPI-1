using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// Card #76, fase 1 (05/11/2026): regras de classificação como frase. "Se [a descrição | a pessoa | as duas]
/// [contém | começa com | é igual a] TEXTO em [conta | todas], então categoria, tipo e pessoa". Sem diferenciar
/// maiúscula, acento e espaços; mais de uma bate: vale a de cima (Ordem).
/// </summary>
public class RegraClassificacaoServiceTests
{
    private const int UserId = 7;

    private static RegraClassificacao Regra(int id, string texto, int ordem = 0, bool ativa = true, string campo = CamposRegra.DescricaoOuPessoa,
        string operador = OperadoresRegra.Contem, int? banco = null) =>
        new()
        {
            Id = id, UsuarioId = UserId, Texto = RegraClassificacao.Normalizar(texto)!, CategoriaId = 1, Ativa = ativa, Ordem = ordem,
            Campo = campo, Operador = operador, BancoId = banco
        };

    // ---------- normalização e casamento ----------

    [Theory]
    [InlineData("  iFood  *Restaurante ", "IFOOD *RESTAURANTE")]
    [InlineData("Padaria São João", "PADARIA SAO JOAO")]
    [InlineData("   ", null)]
    [InlineData(null, null)]
    public void Normaliza_sem_acento_maiuscula_nem_espacos_repetidos(string? texto, string? esperado)
    {
        Assert.Equal(esperado, RegraClassificacao.Normalizar(texto));
    }

    [Fact]
    public void Casa_pela_descricao_ou_pelo_nome_da_pessoa_e_vale_a_de_cima()
    {
        // "IFOOD" está acima de "IFOOD MERCADO": vale a de cima, mesmo sendo menos específica.
        var regras = new[] { Regra(2, "ifood mercado", ordem: 1), Regra(1, "IFOOD", ordem: 0), Regra(3, "Uber", ordem: 2, ativa: false), Regra(4, "Padaria", ordem: 3) };

        Assert.Equal(1, ClassificadorRegras.Encontrar(regras, "Compra iFood Mercado *123", null, null)?.Id);
        Assert.Equal(4, ClassificadorRegras.Encontrar(regras, null, "PADARIA DO ZÉ", null)?.Id);
        Assert.Null(ClassificadorRegras.Encontrar(regras, "UBER *TRIP", null, null));            // desligada
        Assert.Null(ClassificadorRegras.Encontrar(regras, "Mercado X", "MERCADO X", null));
        Assert.Null(ClassificadorRegras.Encontrar(regras, null, null, null));

        // Mesma ordem: a de id menor.
        Assert.Equal(5, ClassificadorRegras.Encontrar([Regra(6, "UBER", 0), Regra(5, "UBER TRIP", 0)], "UBER TRIP", null, null)?.Id);
    }

    [Fact]
    public void Campo_operador_e_conta_restringem_a_regra()
    {
        var soDescricao = Regra(1, "POSTO", campo: CamposRegra.Descricao, operador: OperadoresRegra.ComecaCom);
        Assert.True(Bate(soDescricao, "Posto Shell", null));
        Assert.False(Bate(soDescricao, "Auto Posto Shell", null));     // não começa com
        Assert.False(Bate(soDescricao, null, "POSTO SHELL"));          // só olha a descrição

        var soPessoa = Regra(2, "Carlos", campo: CamposRegra.Pessoa, operador: OperadoresRegra.Igual);
        Assert.True(Bate(soPessoa, "Pix", "carlos"));
        Assert.False(Bate(soPessoa, "Pix", "Carlos Silva"));           // igual, não contém
        Assert.False(Bate(soPessoa, "Carlos", null));

        var naConta = Regra(3, "IOF", banco: 5);
        Assert.True(ClassificadorRegras.Bate(naConta, "IOF COMPRA", null, 5));
        Assert.False(ClassificadorRegras.Bate(naConta, "IOF COMPRA", null, 6));
        Assert.False(ClassificadorRegras.Bate(naConta, "IOF COMPRA", null, null));
    }

    private static bool Bate(RegraClassificacao r, string? descricao, string? pessoa) =>
        ClassificadorRegras.Bate(r, RegraClassificacao.Normalizar(descricao), RegraClassificacao.Normalizar(pessoa), null);

    // ---------- cadastro ----------

    private readonly Mock<IRegraClassificacaoRepository> _repo = new();

    private RegraClassificacaoService Servico()
    {
        _repo.Setup(r => r.CriarAsync(It.IsAny<RegraClassificacao>())).ReturnsAsync((RegraClassificacao r) => r);
        return new RegraClassificacaoService(_repo.Object, MapperFactory.Create());
    }

    [Fact]
    public async Task Cria_a_regra_com_o_texto_normalizado_ativa_e_os_padroes_da_frase()
    {
        var criada = await Servico().AdicionarAsync(new RegraClassificacaoDTO { Texto = " iFood ", CategoriaId = 3, Campo = "", Operador = "", BancoId = 0 }, UserId);

        Assert.Equal("IFOOD", criada.Texto);
        Assert.True(criada.Ativa);
        Assert.Equal(CamposRegra.DescricaoOuPessoa, criada.Campo);
        Assert.Equal(OperadoresRegra.Contem, criada.Operador);
        Assert.Null(criada.BancoId);
        _repo.Verify(r => r.CriarAsync(It.Is<RegraClassificacao>(x => x.UsuarioId == UserId && x.Texto == "IFOOD")), Times.Once);
    }

    [Theory]
    [InlineData(" ", 3, null, null, "O texto da regra é obrigatório")]
    [InlineData("A", 3, null, null, "pelo menos 2")]
    [InlineData("IFOOD", null, null, null, "categoria, um tipo ou uma pessoa")]
    [InlineData("IFOOD", 3, "valor", null, "onde procurar")]
    [InlineData("IFOOD", 3, null, "termina_com", "como comparar")]
    public async Task Recusa_regra_incompleta_ou_com_campo_e_operador_desconhecidos(string texto, int? categoria, string? campo, string? operador, string mensagem)
    {
        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() => Servico().AdicionarAsync(
            new RegraClassificacaoDTO { Texto = texto, CategoriaId = categoria, Campo = campo!, Operador = operador! }, UserId));

        Assert.Contains(mensagem, ex.Message);
    }

    [Fact]
    public async Task Recusa_frase_repetida_mas_aceita_o_mesmo_texto_noutra_conta()
    {
        _repo.Setup(r => r.ExisteFraseAsync(UserId, CamposRegra.DescricaoOuPessoa, OperadoresRegra.Contem, "IFOOD", null, 0)).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            Servico().AdicionarAsync(new RegraClassificacaoDTO { Texto = "ifood", CategoriaId = 3 }, UserId));
        Assert.Contains("Já existe uma regra igual", ex.Message);

        await Servico().AdicionarAsync(new RegraClassificacaoDTO { Texto = "ifood", CategoriaId = 3, BancoId = 5 }, UserId);
        _repo.Verify(r => r.CriarAsync(It.Is<RegraClassificacao>(x => x.BancoId == 5)), Times.Once);
    }

    [Fact]
    public async Task Reordenar_repassa_a_ordem_ao_repositorio()
    {
        await Servico().ReordenarAsync(UserId, [3, 1, 2]);

        _repo.Verify(r => r.ReordenarAsync(UserId, It.Is<IReadOnlyList<int>>(l => l.SequenceEqual(new[] { 3, 1, 2 }))), Times.Once);
    }

    // ---------- modelo ----------

    [Fact]
    public void Tabela_com_a_frase_unica_por_usuario_e_nulls_iguais()
    {
        using var contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseNpgsql("Host=nenhum").Options);
        var tabela = contexto.GetService<IDesignTimeModel>().Model.GetEntityTypes().Single(e => e.GetTableName() == "regras_classificacao");

        Assert.Subset(tabela.GetProperties().Select(p => p.GetColumnName()).ToHashSet(),
            new HashSet<string> { "texto", "campo", "operador", "banco_id", "ordem", "vezes_aplicada", "ultima_aplicacao" });
        var unico = tabela.GetIndexes().Single(i => i.IsUnique);
        Assert.Equal(["UsuarioId", "Campo", "Operador", "Texto", "BancoId"], unico.Properties.Select(p => p.Name));
        Assert.False(unico.GetAreNullsDistinct());
    }
}
