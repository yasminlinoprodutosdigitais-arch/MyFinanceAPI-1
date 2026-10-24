using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Api.Controllers;

namespace MyFinanceAPI.Tests.Api;

/// <summary>
/// BE-P4-16: os GET de lista aceitam ?pagina=&amp;tamanhoPagina= (opcionais) e devolvem o total em X-Total-Count.
/// Sem os parâmetros, a resposta é a lista inteira, como antes: o front (o data-grid pagina no navegador) não muda.
/// </summary>
public class PaginacaoTests
{
    private sealed class ListaController : ApiControllerBase
    {
        public ActionResult<IEnumerable<int>> Listar(IEnumerable<int> itens, Paginacao paginacao) => Lista(itens, paginacao);
    }

    private static (int[] Corpo, string? Total) Chamar(Paginacao paginacao, int quantidade = 45)
    {
        var controller = new ListaController { ControllerContext = new() { HttpContext = new DefaultHttpContext() } };
        var resultado = controller.Listar(Enumerable.Range(1, quantidade), paginacao).Result;
        var corpo = ((IEnumerable<int>)Assert.IsType<OkObjectResult>(resultado).Value!).ToArray();
        var total = controller.Response.Headers.TryGetValue("X-Total-Count", out var valor) ? valor.ToString() : null;
        return (corpo, total);
    }

    private static string[] Erros(Paginacao paginacao)
    {
        var erros = new List<ValidationResult>();
        Validator.TryValidateObject(paginacao, new ValidationContext(paginacao), erros, validateAllProperties: true);
        return erros.Select(e => e.ErrorMessage!).ToArray();
    }

    [Fact]
    public void Sem_parametros_devolve_tudo_sem_cabecalho()
    {
        var (corpo, total) = Chamar(new Paginacao());

        Assert.Equal(45, corpo.Length);
        Assert.Null(total);
    }

    [Fact]
    public void Com_pagina_e_tamanho_devolve_a_fatia_e_o_total()
    {
        var (corpo, total) = Chamar(new Paginacao { Pagina = 2, TamanhoPagina = 10 });

        Assert.Equal(Enumerable.Range(11, 10), corpo);
        Assert.Equal("45", total);
    }

    [Fact]
    public void So_a_pagina_usa_o_tamanho_padrao_de_20()
    {
        var (corpo, total) = Chamar(new Paginacao { Pagina = 3 });

        Assert.Equal(Enumerable.Range(41, 5), corpo);
        Assert.Equal("45", total);
    }

    [Fact]
    public void Pagina_depois_do_fim_volta_vazia_com_o_total()
    {
        var (corpo, total) = Chamar(new Paginacao { Pagina = 9, TamanhoPagina = 10 });

        Assert.Empty(corpo);
        Assert.Equal("45", total);
    }

    [Fact]
    public void Pagina_zero_e_tamanho_acima_do_teto_sao_recusados()
    {
        Assert.Equal(["Pagina deve estar entre 1 e 2147483647."], Erros(new Paginacao { Pagina = 0 }));
        Assert.Equal(["TamanhoPagina deve estar entre 1 e 100."], Erros(new Paginacao { TamanhoPagina = 101 }));
        Assert.Empty(Erros(new Paginacao { Pagina = 1, TamanhoPagina = 100 }));
    }

    /// <summary>Todo GET que devolve lista recebe a paginação, para o contrato ser o mesmo em toda a API.</summary>
    [Fact]
    public void Todo_GET_de_lista_aceita_paginacao()
    {
        var semPaginacao = typeof(ApiControllerBase).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ApiControllerBase)) && !t.IsAbstract)
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Where(m => m.GetCustomAttribute<HttpGetAttribute>() != null && DevolveLista(m.ReturnType))
            .Where(m => m.GetParameters().All(p => p.ParameterType != typeof(Paginacao)))
            .Select(m => $"{m.DeclaringType!.Name}.{m.Name}");

        Assert.Empty(semPaginacao);
    }

    private static bool DevolveLista(Type retorno)
    {
        while (retorno.IsGenericType && retorno.GetGenericTypeDefinition() is var g
               && (g == typeof(Task<>) || g == typeof(ActionResult<>)))
            retorno = retorno.GetGenericArguments()[0];
        return retorno != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(retorno);
    }
}
