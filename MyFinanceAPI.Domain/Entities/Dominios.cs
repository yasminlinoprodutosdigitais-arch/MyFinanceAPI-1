namespace MyFinanceAPI.Domain.Entities;

// Domínios fechados das colunas de status/tipo (DB-P1-03, DB-P1-04, DB-P1-06).
// Os mesmos valores viram CHECK no banco (EntitiesConfiguration): mudar aqui exige migration.

/// <summary>Status de Transactions (coluna text).</summary>
public static class StatusTransacao
{
    public const string Pendente = "PENDENTE";
    public const string Aguardando = "AGUARDANDO";
    public const string PagoNoPrazo = "PAGO NO PRAZO";
    public const string PagoAtrasado = "PAGO ATRASADO";

    public static readonly string[] Todos = [Pendente, Aguardando, PagoNoPrazo, PagoAtrasado];

    public static bool EhPago(string? status) => status is PagoNoPrazo or PagoAtrasado;
    public static bool EhEmAberto(string? status) => status is Pendente or Aguardando;
}

/// <summary>Como o valor do item de lista soma no total (coluna listas.modo_valor).</summary>
public static class ModoValorLista
{
    /// <summary>O valor é o preço de uma unidade: o total do item é quantidade × valor.</summary>
    public const string Unitario = "UNITARIO";
    /// <summary>O valor já é o total do item.</summary>
    public const string Total = "TOTAL";

    public static readonly string[] Todos = [Unitario, Total];
}

/// <summary>
/// Repetição de um item com data (coluna itens_lista.repeticao, card #67 parte 3). DIARIA: todo dia; SEMANAL: nos
/// dias_semana; MENSAL: no mesmo dia do mês (no último dia, se o mês for mais curto).
/// </summary>
public static class RepeticaoItem
{
    public const string Nenhuma = "NENHUMA";
    public const string Diaria = "DIARIA";
    public const string Semanal = "SEMANAL";
    public const string Mensal = "MENSAL";

    public static readonly string[] Todos = [Nenhuma, Diaria, Semanal, Mensal];
}

/// <summary>Modelo escolhido ao criar a lista (coluna listas.modelo). Só pré-marca os campos; não muda regra.</summary>
public static class ModeloLista
{
    public const string Compras = "COMPRAS";
    public const string Orcamento = "ORCAMENTO";
    public const string Tarefas = "TAREFAS";
    public const string Agenda = "AGENDA";
    public const string Branco = "BRANCO";

    public static readonly string[] Todos = [Compras, Orcamento, Tarefas, Agenda, Branco];
}

/// <summary>
/// Chaves da paleta do front (shared/ui/aparencia.ts) para a cor de categorias (#23) e listas (#63).
/// Guarda-se a chave, nunca o hex: a cor em si fica nos tokens do front.
/// </summary>
public static class PaletaCores
{
    public static readonly string[] Todas =
        ["azul", "roxo", "verde-agua", "verde", "laranja", "indigo", "rosa", "vermelho", "amarelo", "cinza"];

    private static readonly HashSet<string> Chaves = [.. Todas];

    /// <summary>Vazio vale (sem cor).</summary>
    public static bool Valida(string? cor) => string.IsNullOrWhiteSpace(cor) || Chaves.Contains(cor.Trim());
}

/// <summary>TipoLancamento de ExtratoBancarioItem: gravado pela importação e pelo lançamento manual.</summary>
public static class TipoLancamentoExtrato
{
    public const string Entrada = "Entrada";
    public const string Saida = "Saída";

    public static readonly string[] Todos = [Entrada, Saida];
}
