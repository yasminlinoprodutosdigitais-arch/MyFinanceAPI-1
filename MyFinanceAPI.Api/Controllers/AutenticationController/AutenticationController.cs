using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers.Authentication
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AuthenticationController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly IUsuarioService _usuarioService;

        public AuthenticationController(ITokenService tokenService, IUsuarioService usuarioService)
        {
            _tokenService = tokenService;
            _usuarioService = usuarioService;
        }

        [HttpPost("/Autenticar")]
        public async Task<IActionResult> Autenticar([FromBody] UsuarioDto command)
        {
            // Verificando se o usuário existe
            UsuarioDto usuario = await _usuarioService.BuscarUsuario(command.Login, command.Senha);
            if (usuario == null)
                return NotFound("Usuário não encontrado. Verifique o login e senha.");

            // Criando o token para o usuário
            TokenDto tokenDto = await _tokenService.CreateToken(usuario);
            if (tokenDto == null)
                return UnprocessableEntity("Erro ao gerar token.");

            return Ok(tokenDto);
        }


        [HttpPost("/RefreshToken")]
        public IActionResult RefreshToken([FromBody] RefreshTokenDto request)
        {
            TokenDto tokenDto = _tokenService.RefreshToken(request.Token, request.TokenRefresh);
            if (tokenDto == null)
                return UnprocessableEntity("Erro ao atualizar token.");

            return Ok(tokenDto);
        }

        [HttpPost("/CadastrarUsuario")]
        public IActionResult Cadastrar([FromBody] CadastrarUsuarioDto request)
        {
            // if (_usuarioService.VerificaSeUsuarioExiste(request.Login, request.NomeUsuario))
            //     return BadRequest("Usuário já cadastrado.");

            // Chamando o serviço para cadastrar o usuário
            UsuarioDto usuario = _usuarioService.CadastrarUsuario(request);
            if (usuario == null)
                return UnprocessableEntity("Erro ao cadastrar usuário.");

            return Ok("Usuário cadastrado com sucesso.");
        }
    }
}
