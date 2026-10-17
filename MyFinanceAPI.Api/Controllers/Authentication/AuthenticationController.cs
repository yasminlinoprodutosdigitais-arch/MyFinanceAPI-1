using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Api.Controllers.Authentication
{
    [Produces("application/json")]
    public class AuthenticationController : ApiControllerBase
    {
        private static readonly string[] RolesPermitidas = { "Admin", "Empresa" };

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
            // Login e senha conferidos aqui; o token é gerado para esse mesmo usuário (BE-P3-10).
            var usuario = await _usuarioService.BuscarUsuario(command.Login, command.Senha);
            if (usuario == null)
                return NaoEncontrado("Usuário não encontrado. Verifique o login e senha.");

            var tokenDto = _tokenService.CreateToken(usuario);

            return Ok(new
            {
                Token = tokenDto,
                Usuario = new Usuario
                {
                    NomeUsuario = usuario.NomeUsuario,
                    Email = usuario.Email,
                    Role = usuario.Role,

                }
            });
        }


        // Exige usuário autenticado (BE-P0-02): antes era anônimo e aceitava qualquer Role do body,
        // o que dava acesso total à API a quem se cadastrasse.
        // Sem tela no front: é a forma de criar usuário (via Swagger, logado), por isso fica (BE-P3-02).
        [Authorize(Policy = "Admin")]
        [HttpPost("/CadastrarUsuario")]
        public IActionResult Cadastrar([FromBody] CadastrarUsuarioDto request)
        {
            if (!RolesPermitidas.Contains(request.Role))
                return ErroRequisicao($"Role inválida. Valores aceitos: {string.Join(", ", RolesPermitidas)}.");

            if (_usuarioService.VerificaSeUsuarioExiste(request.Login, request.NomeUsuario))
                return ErroRequisicao("Usuário já cadastrado.");

            // Falha ao gravar vai ao ErroHandler (500 com log). Não há GET de usuário, então o 201
            // vai sem Location; o corpo é o criado, sem senha nem hash.
            var usuario = _usuarioService.CadastrarUsuario(request);

            return StatusCode(StatusCodes.Status201Created,
                new UsuarioCadastradoDto(usuario.Id, usuario.UserName!, usuario.NomeUsuario, usuario.Role));
        }
    }
}
