using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class RegrasClassificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "regras_classificacao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    texto = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    campo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "descricao_ou_pessoa"),
                    operador = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "contem"),
                    banco_id = table.Column<int>(type: "integer", nullable: true),
                    categoria_id = table.Column<int>(type: "integer", nullable: true),
                    tipo_movimentacao_id = table.Column<int>(type: "integer", nullable: true),
                    pessoa_movimentacao_id = table.Column<int>(type: "integer", nullable: true),
                    ativa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ordem = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    vezes_aplicada = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ultima_aplicacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    usuario_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regras_classificacao", x => x.id);
                    table.ForeignKey(
                        name: "fk_regras_classificacao_asp_net_users_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_regras_classificacao_bancos_banco_id",
                        column: x => x.banco_id,
                        principalTable: "bancos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_regras_classificacao_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_regras_classificacao_pessoa_movimentacao_id",
                        column: x => x.pessoa_movimentacao_id,
                        principalTable: "pessoas_movimentacao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_regras_classificacao_tipos_movimentacao_tipo_movimentacao_id",
                        column: x => x.tipo_movimentacao_id,
                        principalTable: "tipos_movimentacao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_regras_classificacao_banco_id",
                table: "regras_classificacao",
                column: "banco_id");

            migrationBuilder.CreateIndex(
                name: "ix_regras_classificacao_categoria_id",
                table: "regras_classificacao",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_regras_classificacao_pessoa_movimentacao_id",
                table: "regras_classificacao",
                column: "pessoa_movimentacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_regras_classificacao_tipo_movimentacao_id",
                table: "regras_classificacao",
                column: "tipo_movimentacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_regras_classificacao_usuario_id_campo_operador_texto_banco_",
                table: "regras_classificacao",
                columns: new[] { "usuario_id", "campo", "operador", "texto", "banco_id" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "ix_regras_classificacao_usuario_id_ordem",
                table: "regras_classificacao",
                columns: new[] { "usuario_id", "ordem" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "regras_classificacao");
        }
    }
}
