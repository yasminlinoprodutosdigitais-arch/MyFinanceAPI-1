using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class NotasFiscais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notas_fiscais",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    extrato_bancario_item_id = table.Column<int>(type: "integer", nullable: false),
                    estabelecimento = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    link = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    usuario_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notas_fiscais", x => x.id);
                    table.ForeignKey(
                        name: "fk_notas_fiscais_asp_net_users_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notas_fiscais_extrato_bancario_item_id",
                        column: x => x.extrato_bancario_item_id,
                        principalTable: "itens_extrato_bancario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notas_fiscais_itens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nota_fiscal_id = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantidade = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    categoria_id = table.Column<int>(type: "integer", nullable: true),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    usuario_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notas_fiscais_itens", x => x.id);
                    table.ForeignKey(
                        name: "fk_notas_fiscais_itens_asp_net_users_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notas_fiscais_itens_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_notas_fiscais_itens_notas_fiscais_nota_fiscal_id",
                        column: x => x.nota_fiscal_id,
                        principalTable: "notas_fiscais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notas_fiscais_extrato_bancario_item_id",
                table: "notas_fiscais",
                column: "extrato_bancario_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notas_fiscais_usuario_id",
                table: "notas_fiscais",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_notas_fiscais_itens_categoria_id",
                table: "notas_fiscais_itens",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_notas_fiscais_itens_nota_fiscal_id_ordem",
                table: "notas_fiscais_itens",
                columns: new[] { "nota_fiscal_id", "ordem" });

            migrationBuilder.CreateIndex(
                name: "ix_notas_fiscais_itens_usuario_id",
                table: "notas_fiscais_itens",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notas_fiscais_itens");

            migrationBuilder.DropTable(
                name: "notas_fiscais");
        }
    }
}
