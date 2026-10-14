using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class IndicesConsulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "idx_extratobancarioitem_datamov",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropIndex(
                name: "IX_ExtratoBancarioItem_UserId",
                table: "ExtratoBancarioItem");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_Date",
                table: "Transactions",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ExtratoBancarioItem_UserId_DataMovimentacao",
                table: "ExtratoBancarioItem",
                columns: new[] { "UserId", "DataMovimentacao" });

            migrationBuilder.CreateIndex(
                name: "IX_ExtratoBancarioItem_UserId_NumeroFatura",
                table: "ExtratoBancarioItem",
                columns: new[] { "UserId", "NumeroFatura" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId_Date",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_ExtratoBancarioItem_UserId_DataMovimentacao",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropIndex(
                name: "IX_ExtratoBancarioItem_UserId_NumeroFatura",
                table: "ExtratoBancarioItem");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_extratobancarioitem_datamov",
                table: "ExtratoBancarioItem",
                column: "DataMovimentacao");

            migrationBuilder.CreateIndex(
                name: "IX_ExtratoBancarioItem_UserId",
                table: "ExtratoBancarioItem",
                column: "UserId");
        }
    }
}
