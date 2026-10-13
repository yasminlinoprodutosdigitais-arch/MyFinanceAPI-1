using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Api.Erros;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
{
    /// <summary>
    /// Base dos controllers autenticados (BE-P2-01). O usuário vem de <see cref="UsuarioId"/>, e os
    /// erros são tratados pelo <see cref="ErroHandler"/> — os controllers não têm checagem de usuário
    /// nem try/catch.
    /// </summary>
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        /// <summary>Usuário do token. Sem usuário: 401 (UsuarioNaoAutenticadoException).</summary>
        protected int UsuarioId
        {
            get
            {
                var userId = HttpContext.RequestServices.GetRequiredService<IUserContextService>().GetUserIdFromClaims();
                return userId != 0 ? userId : throw new UsuarioNaoAutenticadoException();
            }
        }
    }
}
