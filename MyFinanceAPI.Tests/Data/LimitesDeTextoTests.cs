using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using MyFinanceAPI.Api.Erros;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.DTO.Movimentacoes;
using MyFinanceAPI.Data.Context;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Data;

/// <summary>
/// DB-P3-04: o tamanho de cada coluna de texto é o da tabela da spec do card, e o DTO recusa
/// (400 com <c>{ message }</c>) o mesmo texto que a coluna recusaria — o banco fica só como rede.
/// </summary>
public class LimitesDeTextoTests
{
    private static IModel Modelo()
    {
        using var contexto = new ContextDB(new DbContextOptionsBuilder<ContextDB>().UseNpgsql("Host=nenhum").Options);
        return contexto.GetService<IDesignTimeModel>().Model;
    }

    private static int? LimiteDaColuna(string tabela, string coluna)
    {
        var entidade = Modelo().GetEntityTypes().Single(e => e.GetTableName() == tabela);
        var objeto = StoreObjectIdentifier.Table(tabela, entidade.GetSchema());
        return entidade.GetProperties().Single(p => p.GetColumnName(objeto) == coluna).GetMaxLength();
    }

    [Theory]
    [InlineData("contas", "nome", 120)]
    [InlineData("categorias", "nome", 120)]
    [InlineData("listas", "nome", 120)]
    [InlineData("transacoes", "nome", 120)]
    [InlineData("categorias", "subcategoria", 120)]
    [InlineData("bancos", "nome_banco", 120)]
    [InlineData("pessoas_movimentacao", "nome_pessoa", 120)]
    [InlineData("movimentacoes_diarias", "nome_pessoa_transacao", 120)]
    [InlineData("itens_extrato_bancario", "nome_pessoa_transacao", 120)]
    [InlineData("asp_net_users", "nome_usuario", 120)]
    [InlineData("bancos", "agencia", 10)]
    [InlineData("bancos", "numero_conta", 20)]
    [InlineData("asp_net_users", "phone_number", 20)]
    [InlineData("movimentacoes_diarias", "tipo_lancamento", 20)]
    [InlineData("itens_extrato_bancario", "tipo_lancamento", 20)]
    [InlineData("transacoes", "situacao", 20)]
    [InlineData("itens_lista", "situacao", 20)]
    [InlineData("extratos_bancarios", "situacao", 20)]
    [InlineData("asp_net_users", "perfil", 30)]
    [InlineData("tipos_cartao", "nome_tipo_cartao", 60)]
    [InlineData("tipos_movimentacao", "nome_tipo_movimentacao", 60)]
    [InlineData("movimentacoes_diarias", "identificador", 100)]
    [InlineData("itens_extrato_bancario", "identificador", 100)]
    [InlineData("tipos_cartao", "descricao", 200)]
    [InlineData("tipos_movimentacao", "descricao", 200)]
    [InlineData("movimentacoes_diarias", "descricao", 200)]
    [InlineData("itens_extrato_bancario", "descricao", 200)]
    [InlineData("extratos_bancarios", "nome_arquivo", 260)]
    [InlineData("extratos_bancarios", "nome_arquivo_origem", 260)]
    [InlineData("transacoes", "observacao", 500)]
    [InlineData("itens_lista", "observacao", 500)]
    [InlineData("itens_extrato_bancario", "observacao", 500)]
    public void Coluna_tem_o_limite_da_spec(string tabela, string coluna, int limite)
    {
        Assert.Equal(limite, LimiteDaColuna(tabela, coluna));
    }

    public static TheoryData<Type, Type> DtosEEntidades => new()
    {
        { typeof(ContaDTO), typeof(Conta) },
        { typeof(CategoriaDTO), typeof(Categoria) },
        { typeof(TransacaoDTO), typeof(Transacao) },
        { typeof(BancoDTO), typeof(Banco) },
        { typeof(TipoCartaoDTO), typeof(TipoCartao) },
        { typeof(ListaDTO), typeof(Lista) },
        { typeof(ItemListaDTO), typeof(ItemLista) },
        { typeof(ExtratoBancarioDTO), typeof(ExtratoBancario) },
        { typeof(ExtratoBancarioItemDTO), typeof(ExtratoBancarioItem) },
        { typeof(MovimentacaoDiariaDTO), typeof(MovimentacaoDiaria) },
        { typeof(TipoMovimentacaoDTO), typeof(TipoMovimentacao) },
        { typeof(PessoaMovimentacaoDTO), typeof(PessoaMovimentacao) },
    };

    /// <summary>Toda propriedade de texto do DTO com coluna limitada de mesmo nome repete o limite dela.</summary>
    [Theory]
    [MemberData(nameof(DtosEEntidades))]
    public void Dto_tem_o_mesmo_limite_da_coluna(Type dto, Type entidade)
    {
        var tipo = Modelo().FindEntityType(entidade)!;
        var divergentes =
            from p in dto.GetProperties().Where(p => p.PropertyType == typeof(string))
            let coluna = tipo.FindProperty(p.Name)?.GetMaxLength()
            where coluna is not null
            let doDto = p.GetCustomAttribute<MaxLengthAttribute>()?.Length
            where doDto != coluna
            select $"{dto.Name}.{p.Name}: DTO {doDto?.ToString() ?? "sem limite"}, coluna {coluna}";

        Assert.Empty(divergentes);
    }

    /// <summary>Record posicional: no ASP.NET a validação fica no parâmetro do construtor.</summary>
    [Theory]
    [InlineData(nameof(CadastrarUsuarioDto.NomeUsuario))]
    [InlineData(nameof(CadastrarUsuarioDto.Perfil))]
    public void Cadastro_de_usuario_tem_o_mesmo_limite_da_coluna(string nome)
    {
        var parametro = typeof(CadastrarUsuarioDto).GetConstructors().Single().GetParameters().Single(p => p.Name == nome);
        var coluna = Modelo().FindEntityType(typeof(Usuario))!.FindProperty(nome)!.GetMaxLength();

        Assert.Equal(coluna, parametro.GetCustomAttribute<MaxLengthAttribute>()?.Length);
    }

    private static IActionResult? Validar(object dto)
    {
        var erros = new List<ValidationResult>();
        if (Validator.TryValidateObject(dto, new ValidationContext(dto), erros, validateAllProperties: true)) return null;

        var estado = new ModelStateDictionary();
        foreach (var erro in erros) estado.AddModelError(erro.MemberNames.FirstOrDefault() ?? "", erro.ErrorMessage!);
        return ModeloInvalido.Resposta(new ActionContext(new DefaultHttpContext(), new(), new(), estado));
    }

    [Fact]
    public void Conta_com_nome_de_121_caracteres_e_400_com_message()
    {
        var resposta = Validar(new ContaDTO { Nome = new string('a', 121), DataOperacao = [5] });

        var objeto = Assert.IsAssignableFrom<ObjectResult>(resposta);
        Assert.Equal(StatusCodes.Status400BadRequest, objeto.StatusCode);
        Assert.Equal("Dados inválidos. Nome aceita no máximo 120 caracteres.", Assert.IsType<MensagemErro>(objeto.Value).Message);
    }

    [Fact]
    public void Conta_com_nome_de_120_caracteres_passa()
    {
        Assert.Null(Validar(new ContaDTO { Nome = new string('a', 120), DataOperacao = [5] }));
    }
}
