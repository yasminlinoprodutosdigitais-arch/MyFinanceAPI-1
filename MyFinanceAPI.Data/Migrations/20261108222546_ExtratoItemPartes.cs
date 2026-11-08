using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtratoItemPartes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "extrato_item_partes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    extrato_bancario_item_id = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    categoria_id = table.Column<int>(type: "integer", nullable: true),
                    tipo_movimentacao_id = table.Column<int>(type: "integer", nullable: true),
                    pessoa_movimentacao_id = table.Column<int>(type: "integer", nullable: true),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    usuario_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_extrato_item_partes", x => x.id);
                    table.CheckConstraint("extrato_item_partes_valor_check", "valor > 0");
                    table.ForeignKey(
                        name: "fk_extrato_item_partes_asp_net_users_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_extrato_item_partes_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_extrato_item_partes_extrato_bancario_item_id",
                        column: x => x.extrato_bancario_item_id,
                        principalTable: "itens_extrato_bancario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_extrato_item_partes_pessoa_movimentacao_id",
                        column: x => x.pessoa_movimentacao_id,
                        principalTable: "pessoas_movimentacao",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_extrato_item_partes_tipos_movimentacao_tipo_movimentacao_id",
                        column: x => x.tipo_movimentacao_id,
                        principalTable: "tipos_movimentacao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_extrato_item_partes_categoria_id",
                table: "extrato_item_partes",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_extrato_item_partes_extrato_bancario_item_id_ordem",
                table: "extrato_item_partes",
                columns: new[] { "extrato_bancario_item_id", "ordem" });

            migrationBuilder.CreateIndex(
                name: "ix_extrato_item_partes_pessoa_movimentacao_id",
                table: "extrato_item_partes",
                column: "pessoa_movimentacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_extrato_item_partes_tipo_movimentacao_id",
                table: "extrato_item_partes",
                column: "tipo_movimentacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_extrato_item_partes_usuario_id",
                table: "extrato_item_partes",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "extrato_item_partes");
        }
    }
}
