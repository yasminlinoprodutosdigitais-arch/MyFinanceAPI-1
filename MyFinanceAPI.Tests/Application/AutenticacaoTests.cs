using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using MyFinanceAPI.Api.Controllers.Authentication;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Application.Services;
using MyFinanceAPI.Application.Utils;
using MyFinanceAPI.Domain.Entities;
using MyFinanceAPI.Domain.Interfaces;

namespace MyFinanceAPI.Tests.Application;

/// <summary>
/// BE-P3-10: o repositório busca só pelo login (a senha é conferida no serviço, contra o hash);
/// o token é gerado para o usuário já validado, sem buscá-lo de novo; o cadastro não engole erro.
/// </summary>
public class AutenticacaoTests
{
    private readonly Mock<IUsuarioRepository> _repo = new();

    private static Usuario Ana() =>
        new("ana", "senha-certa", "Ana", "Admin", true, DateTime.UtcNow, DateTime.UtcNow) { Id = 12 };

    // ---------- Login ----------

    [Fact]
    public async Task Login_com_senha_certa_devolve_o_usuario_buscado_so_pelo_login()
    {
        var ana = Ana();
        _repo.Setup(r => r.BuscarUsuario("ana")).ReturnsAsync(ana);

        var usuario = await new UsuarioService(_repo.Object).BuscarUsuario("ana", "senha-certa");

        Assert.Same(ana, usuario);
    }

    [Fact]
    public async Task Login_com_senha_errada_devolve_null()
    {
        _repo.Setup(r => r.BuscarUsuario("ana")).ReturnsAsync(Ana());

        Assert.Null(await new UsuarioService(_repo.Object).BuscarUsuario("ana", "senha-errada"));
    }

    [Fact]
    public async Task Login_inexistente_devolve_null()
    {
        Assert.Null(await new UsuarioService(_repo.Object).BuscarUsuario("ninguem", "x"));
    }

    // ---------- Token ----------

    [Fact]
    public void Token_leva_id_login_e_role_do_usuario_validado()
    {
        var configuracao = Options.Create(new TokenSettings { SecretKey = new string('k', 64) });

        var token = new TokenService(configuracao).CreateToken(Ana());

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken((string)token.Token);
        var claims = jwt.Claims.ToDictionary(c => c.Type, c => c.Value);
        Assert.Equal("12", claims["nameid"]);
        Assert.Equal("ana", claims["unique_name"]);
        Assert.Equal("Admin", claims["role"]);
        Assert.True(token.DataExpiracao > DateTime.Now);
    }

    // ---------- Cadastro ----------

    [Fact]
    public void Cadastro_que_falha_no_banco_propaga_o_erro()
    {
        // Antes um catch devolvia null e a falha virava 422 sem log; agora vai ao ErroHandler (500 com log).
        _repo.Setup(r => r.Cadastrar(It.IsAny<Usuario>())).Throws(new InvalidOperationException("falha"));

        Assert.Throws<InvalidOperationException>(() =>
            new UsuarioService(_repo.Object).CadastrarUsuario(new CadastrarUsuarioDto("bia", "s3nh@", "Bia", "Admin")));
    }

    [Fact]
    public void Cadastro_grava_a_senha_como_hash()
    {
        Usuario? gravado = null;
        _repo.Setup(r => r.Cadastrar(It.IsAny<Usuario>())).Callback<Usuario>(u => gravado = u);

        var criado = new UsuarioService(_repo.Object).CadastrarUsuario(new CadastrarUsuarioDto("bia", "s3nh@", "Bia", "Admin"));

        Assert.Same(gravado, criado);
        Assert.Equal("bia", criado.UserName);
        Assert.NotEqual("s3nh@", criado.PasswordHash);
    }

    // ---------- Controller ----------

    private static AuthenticationController Controller(Mock<IUsuarioService> usuarios, Mock<ITokenService> tokens) =>
        new(tokens.Object, usuarios.Object) { ControllerContext = new() { HttpContext = new DefaultHttpContext() } };

    [Fact]
    public async Task Autenticar_gera_o_token_para_o_usuario_validado()
    {
        var ana = Ana();
        var usuarios = new Mock<IUsuarioService>();
        usuarios.Setup(s => s.BuscarUsuario("ana", "senha-certa")).ReturnsAsync(ana);
        var tokens = new Mock<ITokenService>();
        tokens.Setup(t => t.CreateToken(ana)).Returns(new TokenDto("jwt", DateTime.Now, DateTime.Now.AddHours(3)));

        var resposta = await Controller(usuarios, tokens).Autenticar(new UsuarioDto("ana", "senha-certa"));

        Assert.IsType<OkObjectResult>(resposta);
        tokens.Verify(t => t.CreateToken(ana), Times.Once);
    }

    [Fact]
    public void Cadastrar_responde_201_com_o_usuario_sem_senha()
    {
        var usuarios = new Mock<IUsuarioService>();
        usuarios.Setup(s => s.CadastrarUsuario(It.IsAny<CadastrarUsuarioDto>())).Returns(Ana());

        var resposta = Controller(usuarios, new Mock<ITokenService>())
            .Cadastrar(new CadastrarUsuarioDto("ana", "senha-certa", "Ana", "Admin"));

        var objeto = Assert.IsAssignableFrom<ObjectResult>(resposta);
        Assert.Equal(StatusCodes.Status201Created, objeto.StatusCode);
        Assert.Equal(new UsuarioCadastradoDto(12, "ana", "Ana", "Admin"), objeto.Value);
    }
}
