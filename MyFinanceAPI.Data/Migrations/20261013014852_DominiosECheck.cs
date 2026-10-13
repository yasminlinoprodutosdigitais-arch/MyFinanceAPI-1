using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class DominiosECheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TipoMovimentacao",
                table: "Lista",
                newName: "TipoLista");

            // Normalização dos dados ANTES dos CHECKs (DB-P1-03/04). Idempotente; no dev de 12/10/2026
            // só "CONCLUIDO" (46 itens de checklist) e grafias de ItemLista precisavam de ajuste.
            migrationBuilder.Sql(@"UPDATE ""Accounts"" SET ""Status"" = 0 WHERE ""Status"" = 2;");                  // backend gravava 2 = inativa; o front, 0
            migrationBuilder.Sql(@"UPDATE ""Transactions"" SET ""Status"" = UPPER(TRIM(""Status"")) WHERE ""Status"" <> UPPER(TRIM(""Status""));");
            migrationBuilder.Sql(@"UPDATE ""ItemLista"" SET ""Status"" = UPPER(TRIM(""Status"")) WHERE ""Status"" <> UPPER(TRIM(""Status""));"); // Pendente/Ok do modal do Orçamento
            migrationBuilder.Sql(@"UPDATE ""ItemLista"" SET ""Status"" = 'OK' WHERE ""Status"" = 'CONCLUIDO';");           // decisão de 12/10: CONCLUIDO = feito
            migrationBuilder.Sql(@"UPDATE ""ExtratoBancarioItem"" SET ""TipoLancamento"" = 'Saída' WHERE UPPER(TRIM(""TipoLancamento"")) IN ('SAIDA', 'SAÍDA') AND ""TipoLancamento"" <> 'Saída';");     // padrão 'SAIDA' do Resumo
            migrationBuilder.Sql(@"UPDATE ""ExtratoBancarioItem"" SET ""TipoLancamento"" = 'Entrada' WHERE UPPER(TRIM(""TipoLancamento"")) = 'ENTRADA' AND ""TipoLancamento"" <> 'Entrada';");

            migrationBuilder.AddCheckConstraint(
                name: "transactions_parcelas_check",
                table: "Transactions",
                sql: "\"QuantidadeParcelas\" >= 1 AND \"ParcelaAtual\" BETWEEN 1 AND \"QuantidadeParcelas\"");

            migrationBuilder.AddCheckConstraint(
                name: "transactions_status_check",
                table: "Transactions",
                sql: "\"Status\" IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO')");

            migrationBuilder.AddCheckConstraint(
                name: "lista_tipolista_check",
                table: "Lista",
                sql: "\"TipoLista\" IN (1, 2, 3)");

            migrationBuilder.AddCheckConstraint(
                name: "itemlista_quantidade_check",
                table: "ItemLista",
                sql: "\"Quantidade\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "itemlista_status_check",
                table: "ItemLista",
                sql: "\"Status\" IN ('PENDENTE', 'AGUARDANDO', 'OK')");

            migrationBuilder.AddCheckConstraint(
                name: "extratobancarioitem_parcelas_check",
                table: "ExtratoBancarioItem",
                sql: "\"QuantidadeParcelas\" >= 1 AND \"ParcelaAtual\" BETWEEN 1 AND \"QuantidadeParcelas\"");

            migrationBuilder.AddCheckConstraint(
                name: "extratobancarioitem_tipolancamento_check",
                table: "ExtratoBancarioItem",
                sql: "\"TipoLancamento\" IN ('Entrada', 'Saída')");

            migrationBuilder.AddCheckConstraint(
                name: "contavencimento_dia_check",
                table: "ContaVencimento",
                sql: "\"Dia\" BETWEEN 1 AND 31");

            migrationBuilder.AddCheckConstraint(
                name: "categories_naturezaoperacao_check",
                table: "Categories",
                sql: "\"NaturezaOperacao\" BETWEEN 0 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "categories_status_check",
                table: "Categories",
                sql: "\"Status\" IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "accounts_parcelas_check",
                table: "Accounts",
                sql: "NOT \"EhParcelado\" OR (\"QuantidadeParcelas\" >= 1 AND \"ParcelaAtual\" BETWEEN 0 AND \"QuantidadeParcelas\")");

            migrationBuilder.AddCheckConstraint(
                name: "accounts_status_check",
                table: "Accounts",
                sql: "\"Status\" IN (0, 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "transactions_parcelas_check",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "transactions_status_check",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "lista_tipolista_check",
                table: "Lista");

            migrationBuilder.DropCheckConstraint(
                name: "itemlista_quantidade_check",
                table: "ItemLista");

            migrationBuilder.DropCheckConstraint(
                name: "itemlista_status_check",
                table: "ItemLista");

            migrationBuilder.DropCheckConstraint(
                name: "extratobancarioitem_parcelas_check",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropCheckConstraint(
                name: "extratobancarioitem_tipolancamento_check",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropCheckConstraint(
                name: "contavencimento_dia_check",
                table: "ContaVencimento");

            migrationBuilder.DropCheckConstraint(
                name: "categories_naturezaoperacao_check",
                table: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "categories_status_check",
                table: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "accounts_parcelas_check",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "accounts_status_check",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "TipoLista",
                table: "Lista",
                newName: "TipoMovimentacao");
        }
    }
}
