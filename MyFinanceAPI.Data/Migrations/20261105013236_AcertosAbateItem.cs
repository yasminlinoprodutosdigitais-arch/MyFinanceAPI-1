using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AcertosAbateItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "abate_item_id",
                table: "itens_extrato_bancario",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_itens_extrato_bancario_abate_item_id",
                table: "itens_extrato_bancario",
                column: "abate_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_itens_extrato_bancario_abate_item_id",
                table: "itens_extrato_bancario",
                column: "abate_item_id",
                principalTable: "itens_extrato_bancario",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_itens_extrato_bancario_abate_item_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropIndex(
                name: "ix_itens_extrato_bancario_abate_item_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropColumn(
                name: "abate_item_id",
                table: "itens_extrato_bancario");
        }
    }
}
