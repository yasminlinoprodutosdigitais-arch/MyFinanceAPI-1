using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ItensTipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "usa_tipo",
                table: "listas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "tipo",
                table: "itens_lista",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "usa_tipo",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "tipo",
                table: "itens_lista");
        }
    }
}
