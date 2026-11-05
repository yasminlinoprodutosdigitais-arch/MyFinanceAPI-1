using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class PessoaEuAcertos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "eh_titular",
                table: "pessoas_movimentacao",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "entra_nos_acertos",
                table: "pessoas_movimentacao",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ux_pessoas_movimentacao_titular",
                table: "pessoas_movimentacao",
                column: "usuario_id",
                unique: true,
                filter: "eh_titular");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_pessoas_movimentacao_titular",
                table: "pessoas_movimentacao");

            migrationBuilder.DropColumn(
                name: "eh_titular",
                table: "pessoas_movimentacao");

            migrationBuilder.DropColumn(
                name: "entra_nos_acertos",
                table: "pessoas_movimentacao");
        }
    }
}
