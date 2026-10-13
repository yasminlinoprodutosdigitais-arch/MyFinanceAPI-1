using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class DataCriacaoTransacional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "ExtratoBancarioItem",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            // DB-P1-12: para as linhas existentes, o melhor dado de criação é o DataAlteracao atual — até
            // aqui só Banco/Category o atualizavam na edição, então nestas tabelas ele ainda é a data do INSERT.
            migrationBuilder.Sql(@"UPDATE ""Transactions"" SET ""DataCriacao"" = ""DataAlteracao"";");
            migrationBuilder.Sql(@"UPDATE ""ExtratoBancarioItem"" SET ""DataCriacao"" = ""DataAlteracao"";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "ExtratoBancarioItem");
        }
    }
}
