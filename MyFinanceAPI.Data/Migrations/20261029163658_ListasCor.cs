using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ListasCor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cor",
                table: "listas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "listas_cor_check",
                table: "listas",
                sql: "cor IN ('azul', 'roxo', 'verde-agua', 'verde', 'laranja', 'indigo', 'rosa', 'vermelho', 'amarelo', 'cinza')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "listas_cor_check",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "cor",
                table: "listas");
        }
    }
}
