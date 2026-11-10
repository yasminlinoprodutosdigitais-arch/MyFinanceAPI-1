using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContasCompartilhadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "compartilhado",
                table: "bancos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "convite_compartilhar_dispensado",
                table: "bancos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "dia_fechamento",
                table: "bancos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "bancos_pessoas",
                columns: table => new
                {
                    banco_id = table.Column<int>(type: "integer", nullable: false),
                    pessoa_movimentacao_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bancos_pessoas", x => new { x.banco_id, x.pessoa_movimentacao_id });
                    table.ForeignKey(
                        name: "fk_bancos_pessoas_bancos_banco_id",
                        column: x => x.banco_id,
                        principalTable: "bancos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bancos_pessoas_pessoas_movimentacao_pessoa_movimentacao_id",
                        column: x => x.pessoa_movimentacao_id,
                        principalTable: "pessoas_movimentacao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bancos_pessoas_pessoa_movimentacao_id",
                table: "bancos_pessoas",
                column: "pessoa_movimentacao_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bancos_pessoas");

            migrationBuilder.DropColumn(
                name: "compartilhado",
                table: "bancos");

            migrationBuilder.DropColumn(
                name: "convite_compartilhar_dispensado",
                table: "bancos");

            migrationBuilder.DropColumn(
                name: "dia_fechamento",
                table: "bancos");
        }
    }
}
