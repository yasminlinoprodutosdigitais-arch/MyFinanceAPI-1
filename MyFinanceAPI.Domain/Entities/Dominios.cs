namespace MyFinanceAPI.Domain.Entities;

// Domínios fechados das colunas de status/tipo (DB-P1-03, DB-P1-04, DB-P1-06).
// Os mesmos valores viram CHECK no banco (EntitiesConfiguration): mudar aqui exige migration.

/// <summary>Status de Accounts e Categories (coluna integer).</summary>
public static class StatusAtivo
{
    public const int Inativo = 0;
    public const int Ativo = 1;

    public static readonly int[] Todos = [Inativo, Ativo];
}

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

/// <summary>Status de ItemLista (coluna text).</summary>
public static class StatusItemLista
{
    public const string Pendente = "PENDENTE";
    public const string Aguardando = "AGUARDANDO";
    public const string Ok = "OK";

    public static readonly string[] Todos = [Pendente, Aguardando, Ok];
}

/// <summary>TipoLancamento de ExtratoBancarioItem: gravado pela importação e pelo lançamento manual.</summary>
public static class TipoLancamentoExtrato
{
    public const string Entrada = "Entrada";
    public const string Saida = "Saída";

    public static readonly string[] Todos = [Entrada, Saida];
}

/// <summary>Tipo da Lista (coluna "TipoLista"). Espelha o enum TipoLista do front.</summary>
public enum TipoLista
{
    Checklist = 1,
    Cronograma = 2,
    Orcamento = 3
}
