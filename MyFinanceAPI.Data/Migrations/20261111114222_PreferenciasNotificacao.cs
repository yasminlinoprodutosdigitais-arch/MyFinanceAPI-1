using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class PreferenciasNotificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "preferencias_notificacao",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    lidas_ate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    config = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_preferencias_notificacao", x => x.usuario_id);
                    table.ForeignKey(
                        name: "fk_preferencias_notificacao_asp_net_users_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "preferencias_notificacao");
        }
    }
}
