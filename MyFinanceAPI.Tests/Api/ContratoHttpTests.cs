using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyFinanceAPI.Api.Controllers;
using MyFinanceAPI.Api.Controllers.Authentication;
using MyFinanceAPI.Api.Erros;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.Interfaces;
using MyFinanceAPI.Domain.Entities;

namespace MyFinanceAPI.Tests.Api;

/// <summary>
/// Contrato HTTP único (BE-P3-05): Create → 201 + criado + Location do GetById; Update → 200 + DTO;
/// Delete → 204; não achou → 404 { message }; lista vazia → 200 [].
/// </summary>
public class ContratoHttpTests
{
    private const int UserId = 7;

    private static T Com<T>(T controller) where T : ControllerBase
    {
        var usuario = new Mock<IUserContextService>();
        usuario.Setup(u => u.GetUserIdFromClaims()).Returns(UserId);
        var servicos = new ServiceCollection().AddSingleton(usuario.Object).BuildServiceProvider();
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { RequestServices = servicos } };
        return controller;
    }

    /// <summary>201 cujo nome de rota existe num GET do controller (BE-P3-04) e cujo Location leva o id.</summary>
    private static T Criado<TController, T>(IActionResult resultado, int idEsperado)
    {
        var criado = Assert.IsType<CreatedAtRouteResult>(resultado);
        Assert.Equal(StatusCodes.Status201Created, criado.StatusCode);
        Assert.Contains(criado.RouteName!, RotasTests.NomesDeGet(typeof(TController)));
        Assert.Equal(idEsperado, criado.RouteValues!["id"]);
        return Assert.IsAssignableFrom<T>(criado.Value);
    }

    private static string Mensagem(IActionResult resultado, int status)
    {
        var objeto = Assert.IsAssignableFrom<ObjectResult>(resultado);
        Assert.Equal(status, objeto.StatusCode);
        return Assert.IsType<MensagemErro>(objeto.Value).Message;
    }

    // ---------- Cadastro simples (Banco representa os da base ServicoCadastro) ----------

    private readonly Mock<IBancoService> _bancos = new();
    private BancosController Bancos() => Com(new BancosController(_bancos.Object));

    [Fact]
    public async Task Create_responde_201_com_o_criado_e_o_Location_do_GetById()
    {
        _bancos.Setup(s => s.AdicionarAsync(It.IsAny<BancoDTO>(), UserId))
            .ReturnsAsync(new BancoDTO { Id = 42, NomeBanco = "Nubank" });

        var resposta = await Bancos().CreateBanco(new BancoDTO { NomeBanco = "Nubank" });

        var corpo = Criado<BancosController, BancoDTO>(resposta.Result!, idEsperado: 42);
        Assert.Equal("Nubank", corpo.NomeBanco);
    }

    [Fact]
    public async Task Update_responde_200_com_o_DTO()
    {
        var dto = new BancoDTO { Id = 3, NomeBanco = "Inter" };

        var resposta = await Bancos().UpdateBanco(dto);

        Assert.Same(dto, Assert.IsType<OkObjectResult>(resposta.Result).Value);
        _bancos.Verify(s => s.AtualizarAsync(dto, UserId), Times.Once);
    }

    [Fact]
    public async Task Delete_responde_204_sem_corpo()
    {
        _bancos.Setup(s => s.ObterPorIdAsync(3, UserId)).ReturnsAsync(new BancoDTO { Id = 3 });

        var resposta = await Bancos().DeleteBanco(3);

        Assert.IsType<NoContentResult>(resposta);
        _bancos.Verify(s => s.RemoverAsync(3, UserId), Times.Once);
    }

    [Fact]
    public async Task Delete_de_inexistente_responde_404_com_message_e_nao_remove()
    {
        var resposta = await Bancos().DeleteBanco(3);

        Assert.Equal("Banco não encontrado.", Mensagem(resposta, 404));
        _bancos.Verify(s => s.RemoverAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetById_de_inexistente_responde_404_com_message()
    {
        var resposta = await Bancos().GetBancoById(3);

        Assert.Equal("Banco não encontrado.", Mensagem(resposta.Result!, 404));
    }

    [Fact]
    public async Task Lista_vazia_responde_200_com_lista_vazia()
    {
        var categorias = new Mock<ICategoriaService>();
        categorias.Setup(s => s.ListarAsync(UserId)).ReturnsAsync([]);

        var resposta = await Com(new CategoriasController(categorias.Object)).GetCategories(new Paginacao());

        var corpo = Assert.IsType<OkObjectResult>(resposta.Result).Value;
        Assert.Empty(Assert.IsAssignableFrom<IEnumerable<CategoriaDTO>>(corpo));
    }

    // ---------- Os que tinham Create fora do contrato ----------

    [Fact]
    public async Task Create_de_conta_aponta_para_o_GET_de_conta()
    {
        // Antes: CreatedAtRoute("GetCategory", ...) com o id do payload (0) — BE-P3-04.
        var contas = new Mock<IContaService>();
        contas.Setup(s => s.Add(It.IsAny<ContaDTO>(), UserId)).ReturnsAsync(new ContaDTO { Id = 15, Nome = "Internet" });

        var resposta = await Com(new ContasController(contas.Object)).CreateAccount(new ContaDTO { Nome = "Internet" });

        Criado<ContasController, ContaDTO>(resposta.Result!, idEsperado: 15);
        Assert.Equal(ContasController.RotaPorId, ((CreatedAtRouteResult)resposta.Result!).RouteName);
    }

    [Fact]
    public async Task Create_de_transacao_devolve_todas_as_criadas_com_o_Location_da_primeira()
    {
        // Antes: "Getupdate" para a rota "GetUpdate" — 500 ao criar (BE-P3-04).
        var transacoes = new Mock<ITransacaoService>();
        transacoes.Setup(s => s.Add(It.IsAny<TransacaoDTO>(), UserId))
            .ReturnsAsync([new TransacaoDTO { Id = 900 }, new TransacaoDTO { Id = 901 }]);

        var resposta = await Com(new TransacoesController(transacoes.Object)).CreateTransaction(new TransacaoDTO());

        var corpo = Criado<TransacoesController, IReadOnlyList<TransacaoDTO>>(resposta.Result!, idEsperado: 900);
        Assert.Equal([900, 901], corpo.Select(t => t.Id));
    }

    [Fact]
    public async Task Delete_de_transacao_responde_204()
    {
        var transacoes = new Mock<ITransacaoService>();

        var resposta = await Com(new TransacoesController(transacoes.Object)).DeleteAccountHistory(5);

        Assert.IsType<NoContentResult>(resposta);
        transacoes.Verify(s => s.Delete(5, UserId), Times.Once);
    }

    [Fact]
    public async Task Create_de_lista_responde_201_em_vez_de_texto()
    {
        var listas = new Mock<IListaService>();
        listas.Setup(s => s.AdicionarAsync(It.IsAny<ListaDTO>(), UserId)).ReturnsAsync(new ListaDTO { Id = 8, Nome = "Mercado" });

        var resposta = await Com(new ListasController(listas.Object)).CreateLista(new ListaDTO { Nome = "Mercado" });

        Criado<ListasController, ListaDTO>(resposta.Result!, idEsperado: 8);
    }

    [Fact]
    public async Task Create_de_item_da_lista_aponta_para_os_itens_da_lista_dele()
    {
        var itens = new Mock<IItemListaService>();
        itens.Setup(s => s.AdicionarAsync(It.IsAny<ItemListaDTO>(), UserId))
            .ReturnsAsync(new ItemListaDTO { Id = 30, ListaId = 8, Descricao = "Arroz" });

        var resposta = await Com(new ItemListaController(itens.Object)).CreateItemLista(new ItemListaDTO { ListaId = 8, Descricao = "Arroz" });

        var corpo = Criado<ItemListaController, ItemListaDTO>(resposta.Result!, idEsperado: 8);
        Assert.Equal(30, corpo.Id);
    }

    [Fact]
    public async Task Update_de_item_de_extrato_responde_200_com_o_DTO_em_vez_de_mensagem()
    {
        var itens = new Mock<IExtratoBancarioItemService>();
        var dto = new ExtratoBancarioItemDTO { Id = 4 };

        var resposta = await Com(new ExtratoBancarioItemController(itens.Object)).UpdateExtratoItem(dto);

        Assert.Same(dto, Assert.IsType<OkObjectResult>(resposta.Result).Value);
    }

    [Fact]
    public async Task Parametro_invalido_responde_400_com_message()
    {
        var itens = new Mock<IExtratoBancarioItemService>();

        var resposta = await Com(new ExtratoBancarioItemController(itens.Object)).GetByMonth("2025/11", null, new Paginacao());

        Assert.StartsWith("Formato de mês inválido", Mensagem(resposta.Result!, 400));
    }

    [Fact]
    public async Task Login_nao_encontrado_responde_404_com_message()
    {
        var usuarios = new Mock<IUsuarioService>();
        var controller = Com(new AuthenticationController(Mock.Of<ITokenService>(), usuarios.Object));

        var resposta = await controller.Autenticar(new UsuarioDto("ana", "errada"));

        Assert.Equal("Usuário não encontrado. Verifique o login e senha.", Mensagem(resposta, 404));
    }

    // ---------- Corpo inválido ([ApiController]) ----------

    [Fact]
    public void Modelo_invalido_responde_400_com_message_em_vez_de_ProblemDetails()
    {
        var estado = new ModelStateDictionary();
        estado.AddModelError("", "A non-empty request body is required.");
        estado.AddModelError("dto", "A non-empty request body is required.");
        var contexto = new ActionContext(new DefaultHttpContext(), new(), new(), estado);

        var resposta = ModeloInvalido.Resposta(contexto);

        Assert.Equal("Dados inválidos. A non-empty request body is required.", Mensagem(resposta, 400));
    }

    [Fact]
    public void Modelo_invalido_sem_detalhe_responde_so_o_prefixo()
    {
        var contexto = new ActionContext(new DefaultHttpContext(), new(), new(), new ModelStateDictionary());

        Assert.Equal(ModeloInvalido.Prefixo, Mensagem(ModeloInvalido.Resposta(contexto), 400));
    }
}
