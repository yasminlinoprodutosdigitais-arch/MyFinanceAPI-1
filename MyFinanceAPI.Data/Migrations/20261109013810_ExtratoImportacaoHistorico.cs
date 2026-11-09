using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtratoImportacaoHistorico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "desfeito_em",
                table: "extratos_bancarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "mes_referencia",
                table: "extratos_bancarios",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "motivo_desfazer",
                table: "extratos_bancarios",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "quantidade_duplicados",
                table: "extratos_bancarios",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "desfeito_em",
                table: "extratos_bancarios");

            migrationBuilder.DropColumn(
                name: "mes_referencia",
                table: "extratos_bancarios");

            migrationBuilder.DropColumn(
                name: "motivo_desfazer",
                table: "extratos_bancarios");

            migrationBuilder.DropColumn(
                name: "quantidade_duplicados",
                table: "extratos_bancarios");
        }
    }
}
