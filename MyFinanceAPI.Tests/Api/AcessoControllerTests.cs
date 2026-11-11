using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using MyFinanceAPI.Api.Controllers;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Tests.Api;

/// <summary>
/// Card #50: rotas anônimas de cadastro e recuperação de senha. Os pedidos com e-mail respondem sempre 202
/// (não revelam quem tem conta); link vencido ou usado responde 400 com a mensagem.
/// </summary>
public class AcessoControllerTests
{
    private readonly Mock<IAcessoService> _acesso = new();

    private AcessoController Controller() =>
        new(_acesso.Object) { ControllerContext = new() { HttpContext = new DefaultHttpContext() } };

    private static string? Mensagem(IActionResult resultado) =>
        (Assert.IsAssignableFrom<ObjectResult>(resultado).Value as MyFinanceAPI.Api.Erros.MensagemErro)?.Message;

    [Fact]
    public void Controller_e_anonimo()
    {
        Assert.NotNull(typeof(AcessoController).GetCustomAttribute<AllowAnonymousAttribute>());
    }

    [Fact]
    public async Task Cadastro_responde_202()
    {
        var dados = new CadastroDto("Ana", "ana.lima", "ana@exemplo.com", "Senha123");

        var resposta = await Controller().Cadastrar(dados);

        Assert.Equal(StatusCodes.Status202Accepted, Assert.IsAssignableFrom<IStatusCodeActionResult>(resposta).StatusCode);
        _acesso.Verify(a => a.CadastrarAsync(dados));
    }

    [Fact]
    public async Task Confirmar_valido_responde_204_e_vencido_400()
    {
        _acesso.Setup(a => a.ConfirmarEmailAsync("bom")).ReturnsAsync(true);

        Assert.IsType<NoContentResult>(await Controller().ConfirmarEmail(new LinkDto("bom")));
        var vencido = await Controller().ConfirmarEmail(new LinkDto("ruim"));
        Assert.Equal(StatusCodes.Status400BadRequest, Assert.IsAssignableFrom<ObjectResult>(vencido).StatusCode);
        Assert.Equal("Este link expirou ou já foi usado.", Mensagem(vencido));
    }

    [Fact]
    public async Task Reenviar_e_esqueci_respondem_202()
    {
        var reenviar = await Controller().ReenviarConfirmacao(new EmailDto("ana@exemplo.com"));
        var esqueci = await Controller().EsqueciSenha(new EmailDto("ana@exemplo.com"));

        Assert.Equal(StatusCodes.Status202Accepted, Assert.IsAssignableFrom<IStatusCodeActionResult>(reenviar).StatusCode);
        Assert.Equal(StatusCodes.Status202Accepted, Assert.IsAssignableFrom<IStatusCodeActionResult>(esqueci).StatusCode);
        _acesso.Verify(a => a.ReenviarConfirmacaoAsync("ana@exemplo.com"));
        _acesso.Verify(a => a.EsqueciSenhaAsync("ana@exemplo.com"));
    }

    [Fact]
    public async Task Redefinir_valido_responde_204_e_vencido_400()
    {
        var bom = new RedefinirSenhaDto("bom", "NovaSenha9");
        _acesso.Setup(a => a.RedefinirSenhaAsync(bom)).ReturnsAsync(true);

        Assert.IsType<NoContentResult>(await Controller().RedefinirSenha(bom));
        var vencido = await Controller().RedefinirSenha(new RedefinirSenhaDto("ruim", "NovaSenha9"));
        Assert.Equal("Este link expirou ou já foi usado.", Mensagem(vencido));
    }
}
