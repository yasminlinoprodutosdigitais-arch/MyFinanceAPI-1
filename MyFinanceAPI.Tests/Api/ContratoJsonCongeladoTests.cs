using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using MyFinanceAPI.Application.DTO;
using MyFinanceAPI.Application.DTO.Extrato;
using MyFinanceAPI.Application.DTO.Movimentacoes;

namespace MyFinanceAPI.Tests.Api;

/// <summary>
/// DB-P3: nenhuma chave do JSON que o front lê ou envia muda sem querer. Este teste percorre cada DTO
/// (e as entidades que viajam dentro dele) com as opções Web do ASP.NET e compara as chaves com
/// ContratoJson.congelado.txt — desde o subprojeto 3, o contrato em português (nome, valor, data,
/// situacao, contaId, usuarioId…), derivado do contrato em inglês só pelos renomes pretendidos.
/// </summary>
public class ContratoJsonCongeladoTests
{
    // O rótulo é estável; o tipo muda com o renome (o compilador obriga a atualizar aqui).
    private static readonly (string Rotulo, Type Tipo)[] Raizes =
    [
        ("conta", typeof(ContaDTO)),
        ("transacao", typeof(TransacaoDTO)),
        ("categoria", typeof(CategoriaDTO)),
        ("banco", typeof(BancoDTO)),
        ("tipoCartao", typeof(TipoCartaoDTO)),
        ("lista", typeof(ListaDTO)),
        ("resumoLista", typeof(ResumoListaDTO)),
        ("itemLista", typeof(ItemListaDTO)),
        ("marcarItens", typeof(MarcarItensDTO)),
        ("resultadoLote", typeof(ResultadoLoteDTO)),
        ("duplicarLista", typeof(DuplicarListaDTO)),
        ("serieLista", typeof(SerieListaDTO)),
        ("ocorrencia", typeof(OcorrenciaDTO)),
        ("alterarOcorrencia", typeof(AlterarOcorrenciaDTO)),
        ("dividirItem", typeof(DividirItemDTO)),
        ("comparacaoListas", typeof(ComparacaoListasDTO)),
        ("listaComparada", typeof(ListaComparadaDTO)),
        ("efeitosComparacao", typeof(EfeitosComparacaoDTO)),
        ("itemComparado", typeof(ItemComparadoDTO)),
        ("ladoItemComparado", typeof(LadoItemComparadoDTO)),
        ("destaquesComparacao", typeof(DestaquesComparacaoDTO)),
        ("destaqueComparacao", typeof(DestaqueComparacaoDTO)),
        ("extratoBancario", typeof(ExtratoBancarioDTO)),
        ("extratoBancarioItem", typeof(ExtratoBancarioItemDTO)),
        ("extratoImportacaoResultado", typeof(ExtratoImportacaoResultadoDTO)),
        ("acertos", typeof(AcertosDTO)),
        ("totaisAcertos", typeof(TotaisAcertosDTO)),
        ("acertoPessoa", typeof(AcertoPessoaDTO)),
        ("acertoCompra", typeof(AcertoCompraDTO)),
        ("acertoPagamento", typeof(AcertoPagamentoDTO)),
        ("acertoParcelaFutura", typeof(AcertoParcelaFuturaDTO)),
        ("movimentacaoDiaria", typeof(MovimentacaoDiariaDTO)),
        ("tipoMovimentacao", typeof(TipoMovimentacaoDTO)),
        ("pessoaMovimentacao", typeof(PessoaMovimentacaoDTO)),
        ("pessoaEstabelecimento", typeof(PessoaEstabelecimentoDTO)),
        ("pessoasRemovidas", typeof(PessoasRemovidasDTO)),
        ("resumoCategorias", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.ResumoCategoriasDTO)),
        ("resumoCategoriaLinha", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.ResumoCategoriaLinhaDTO)),
        ("evolucaoExtratos", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.EvolucaoExtratosDTO)),
        ("evolucaoExtratosMes", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.EvolucaoExtratosMesDTO)),
        ("opcoesImportacao", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.OpcoesImportacao)),
        ("previaImportacao", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.PreviaImportacaoDTO)),
        ("linhaPreviaImportacao", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.LinhaPreviaImportacaoDTO)),
        ("buscaResultado", typeof(BuscaResultadoDTO)),
        ("resumoContas", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.ResumoContasDTO)),
        ("resumoConta", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.ResumoContaDTO)),
        ("resumoContaPessoa", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.ResumoContaPessoaDTO)),
        ("compartilhamento", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.CompartilhamentoDTO)),
        ("notaFiscal", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.NotaFiscalDTO)),
        ("notaFiscalItem", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.NotaFiscalItemDTO)),
        ("lerNotaFiscal", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.LerNotaFiscalDTO)),
        ("notaSalva", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.NotaSalvaDTO)),
        ("importacao", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.ImportacaoDTO)),
        ("coberturaImportacoes", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.CoberturaImportacoesDTO)),
        ("vinculosImportacao", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.VinculosImportacaoDTO)),
        ("desfazerImportacao", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.DesfazerImportacaoDTO)),
        ("coberturaConta", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.CoberturaContaDTO)),
        ("coberturaCelula", typeof(MyFinanceAPI.Application.DTO.Movimentacoes.CoberturaCelulaDTO)),
        ("regraClassificacao", typeof(RegraClassificacaoDTO)),
        ("extratoItemParte", typeof(MyFinanceAPI.Application.DTO.Extrato.ExtratoItemParteDTO)),
        ("sugestaoCategoria", typeof(SugestaoCategoriaDTO)),
        ("previaRegra", typeof(PreviaRegraDTO)),
        ("previaRegraResultado", typeof(PreviaRegraResultadoDTO)),
        ("exemploRegra", typeof(ExemploRegraDTO)),
        ("aplicacaoRegra", typeof(AplicacaoRegraDTO)),
        ("sugestaoRegra", typeof(SugestaoRegraDTO)),
        ("cadastrarUsuario", typeof(CadastrarUsuarioDto)),
        ("usuarioCadastrado", typeof(UsuarioCadastradoDto)),
        ("token", typeof(TokenDto)),
        ("usuario", typeof(UsuarioDto)),
    ];

    private static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    [Fact]
    public void Todo_DTO_esta_na_lista_de_raizes()
    {
        var dtos = typeof(ContaDTO).Assembly.GetTypes()
            .Where(t => t.IsPublic && t.Namespace?.StartsWith("MyFinanceAPI.Application.DTO") == true);

        Assert.Empty(dtos.Except(Raizes.Select(r => r.Tipo)));
    }

    [Fact]
    public void Chaves_do_JSON_continuam_as_mesmas()
    {
        var atual = Descrever();
        var congelado = Caminho("ContratoJson.congelado.txt");

        if (File.Exists(congelado) && File.ReadAllText(congelado).ReplaceLineEndings() == atual)
            return;

        File.WriteAllText(Caminho("ContratoJson.atual.txt"), atual);
        Assert.Fail("O contrato JSON mudou: compare ContratoJson.atual.txt com ContratoJson.congelado.txt.");
    }

    private static string Descrever()
    {
        var texto = new StringBuilder();
        foreach (var (rotulo, tipo) in Raizes)
            Descrever(texto, rotulo, tipo, []);
        return texto.ToString().ReplaceLineEndings();
    }

    private static void Descrever(StringBuilder texto, string caminho, Type tipo, HashSet<Type> noCaminho)
    {
        tipo = Elemento(tipo);
        if (tipo.Namespace?.StartsWith("MyFinanceAPI") != true || !noCaminho.Add(tipo))
            return;

        foreach (var propriedade in Web.GetTypeInfo(tipo).Properties.OrderBy(p => p.Name, StringComparer.Ordinal))
        {
            // [JsonIgnore] aparece nos metadados, mas nunca no JSON.
            if (propriedade.AttributeProvider?.GetCustomAttributes(typeof(JsonIgnoreAttribute), true)
                    .Cast<JsonIgnoreAttribute>().Any(a => a.Condition == JsonIgnoreCondition.Always) == true)
                continue;

            var filho = $"{caminho}.{propriedade.Name}";
            texto.AppendLine(filho);
            Descrever(texto, filho, propriedade.PropertyType, noCaminho);
        }

        noCaminho.Remove(tipo);
    }

    private static Type Elemento(Type tipo)
    {
        tipo = Nullable.GetUnderlyingType(tipo) ?? tipo;
        if (tipo == typeof(string))
            return tipo;

        var colecao = tipo.GetInterfaces().Append(tipo)
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        return colecao?.GetGenericArguments()[0] ?? tipo;
    }

    private static string Caminho(string arquivo, [CallerFilePath] string origem = "") =>
        Path.Combine(Path.GetDirectoryName(origem)!, arquivo);
}
