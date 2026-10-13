using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class TipoCartaoEhCredito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EhCredito",
                table: "TipoCartao",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // DB-P1-07: os tipos existentes viram crédito pelo nome (no dev, os 2 "Cartão de crédito",
            // ids 1 e 13 — os IDs que estavam fixos no ExtratoBancarioItemRepository).
            migrationBuilder.Sql(
                @"UPDATE ""TipoCartao"" SET ""EhCredito"" = true WHERE ""NomeTipoCartao"" ILIKE '%crédito%' OR ""NomeTipoCartao"" ILIKE '%credito%';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EhCredito",
                table: "TipoCartao");
        }
    }
}
