using Microsoft.AspNetCore.Mvc;
using MyFinanceAPI.Api.Erros;
using MyFinanceAPI.Application.Interfaces;

namespace MyFinanceAPI.Api.Controllers
{
    /// <summary>
    /// Base dos controllers (BE-P2-01). O usuário vem de <see cref="UsuarioId"/>, e os erros são
    /// tratados pelo <see cref="ErroHandler"/> — os controllers não têm checagem de usuário nem try/catch.
    /// Contrato (BE-P3-05): Create → 201 + criado + Location do GetById (<see cref="Criado"/>);
    /// Update → 200 + DTO; Delete → 204; erro → <c>{ message }</c>; lista vazia → 200 [].
    /// As rotas são absolutas em cada ação; não há [Route] de classe (BE-P3-03).
    /// Os GET por id ficam mesmo quando o front não os chama: são o destino do Location dos 201 (BE-P3-02).
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

        /// <summary>201 com <paramref name="corpo"/> e o Location da rota GET de nome <paramref name="rotaPorId"/>.</summary>
        protected CreatedAtRouteResult Criado(string rotaPorId, int id, object corpo) =>
            CreatedAtRoute(rotaPorId, new { id }, corpo);

        /// <summary>400 com <c>{ message }</c>.</summary>
        protected BadRequestObjectResult ErroRequisicao(string mensagem) => BadRequest(new MensagemErro(mensagem));

        /// <summary>404 com <c>{ message }</c>.</summary>
        protected NotFoundObjectResult NaoEncontrado(string mensagem) => NotFound(new MensagemErro(mensagem));

        /// <summary>
        /// 200 com a lista, paginada se a requisição pediu (BE-P4-16): a fatia no corpo e o total em X-Total-Count.
        /// A paginação é sobre o que o service devolveu (já filtrado por usuário e, onde há, por mês), na ordem dele.
        /// </summary>
        protected ActionResult<IEnumerable<T>> Lista<T>(IEnumerable<T> itens, Paginacao paginacao)
        {
            if (!paginacao.Ativa)
                return Ok(itens);

            var todos = itens as IReadOnlyCollection<T> ?? itens.ToList();
            var tamanho = paginacao.TamanhoPagina ?? Paginacao.TamanhoPadrao;
            var pagina = paginacao.Pagina ?? 1;

            Response.Headers[CabecalhoTotal] = todos.Count.ToString();
            return Ok(todos.Skip((int)Math.Min((long)(pagina - 1) * tamanho, int.MaxValue)).Take(tamanho).ToList());
        }

        public const string CabecalhoTotal = "X-Total-Count";
    }
}
