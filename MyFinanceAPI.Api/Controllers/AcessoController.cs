using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyFinanceAPI.Api.Autorizacao;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers;

/// <summary>
/// Acesso sem login (card #50): criar conta, confirmar o e-mail e recuperar a senha. Pedidos com e-mail
/// respondem sempre 202, tenha ou não conta; link vencido ou usado responde 400.
/// Limite de pedidos por IP (<see cref="LimitesAcesso"/>): passou, 429.
/// </summary>
[AllowAnonymous]
[EnableRateLimiting(LimitesAcesso.Politica)]
public class AcessoController(IAcessoService acesso) : ApiControllerBase
{
    private const string LinkInvalido = "Este link expirou ou já foi usado.";

    [HttpPost("/cadastro")]
    public async Task<IActionResult> Cadastrar([FromBody] CadastroDto dados)
    {
        await acesso.CadastrarAsync(dados);
        return Accepted();
    }

    [HttpPost("/cadastro/confirmar")]
    public async Task<IActionResult> ConfirmarEmail([FromBody] LinkDto dados) =>
        await acesso.ConfirmarEmailAsync(dados.Token) ? NoContent() : ErroRequisicao(LinkInvalido);

    [HttpPost("/cadastro/reenviar")]
    public async Task<IActionResult> ReenviarConfirmacao([FromBody] EmailDto dados)
    {
        await acesso.ReenviarConfirmacaoAsync(dados.Email);
        return Accepted();
    }

    [HttpPost("/senha/esqueci")]
    public async Task<IActionResult> EsqueciSenha([FromBody] EmailDto dados)
    {
        await acesso.EsqueciSenhaAsync(dados.Email);
        return Accepted();
    }

    [HttpPost("/senha/redefinir")]
    public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDto dados) =>
        await acesso.RedefinirSenhaAsync(dados) ? NoContent() : ErroRequisicao(LinkInvalido);
}
