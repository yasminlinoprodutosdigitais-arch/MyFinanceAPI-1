using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <summary>
    /// Card #67, parte 2 (28/10/2026): série e origem para comparar listas. O padrão gen_random_uuid() roda
    /// linha a linha, então cada lista existente começa na própria série. Apagar a original só solta a cópia.
    /// </summary>
    public partial class ListasSerie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_listas_usuario_id",
                table: "listas");

            migrationBuilder.AddColumn<int>(
                name: "lista_origem_id",
                table: "listas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "serie_id",
                table: "listas",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<int>(
                name: "item_origem_id",
                table: "itens_lista",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_listas_lista_origem_id",
                table: "listas",
                column: "lista_origem_id");

            migrationBuilder.CreateIndex(
                name: "ix_listas_usuario_id_serie_id",
                table: "listas",
                columns: new[] { "usuario_id", "serie_id" });

            migrationBuilder.CreateIndex(
                name: "ix_itens_lista_item_origem_id",
                table: "itens_lista",
                column: "item_origem_id");

            migrationBuilder.AddForeignKey(
                name: "fk_itens_lista_itens_lista_item_origem_id",
                table: "itens_lista",
                column: "item_origem_id",
                principalTable: "itens_lista",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_listas_listas_lista_origem_id",
                table: "listas",
                column: "lista_origem_id",
                principalTable: "listas",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_itens_lista_itens_lista_item_origem_id",
                table: "itens_lista");

            migrationBuilder.DropForeignKey(
                name: "fk_listas_listas_lista_origem_id",
                table: "listas");

            migrationBuilder.DropIndex(
                name: "ix_listas_lista_origem_id",
                table: "listas");

            migrationBuilder.DropIndex(
                name: "ix_listas_usuario_id_serie_id",
                table: "listas");

            migrationBuilder.DropIndex(
                name: "ix_itens_lista_item_origem_id",
                table: "itens_lista");

            migrationBuilder.DropColumn(
                name: "lista_origem_id",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "serie_id",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "item_origem_id",
                table: "itens_lista");

            migrationBuilder.CreateIndex(
                name: "ix_listas_usuario_id",
                table: "listas",
                column: "usuario_id");
        }
    }
}
