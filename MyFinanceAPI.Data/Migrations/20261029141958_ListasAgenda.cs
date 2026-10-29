using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <summary>
    /// Card #67, parte 3 (29/10/2026): agenda. horario_tarefa vira hora (sem hora = dia todo), o item ganha a regra
    /// de repetição e itens_lista_ocorrencias guarda só as ocorrências que diferem da regra. Nada a converter.
    /// </summary>
    public partial class ListasAgenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "horario_tarefa",
                table: "itens_lista",
                newName: "hora");

            migrationBuilder.AddColumn<int[]>(
                name: "dias_semana",
                table: "itens_lista",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "repeticao",
                table: "itens_lista",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "NENHUMA");

            migrationBuilder.AddColumn<DateOnly>(
                name: "repetir_ate",
                table: "itens_lista",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "itens_lista_ocorrencias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    feito = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    feito_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    nova_data = table.Column<DateOnly>(type: "date", nullable: true),
                    nova_hora = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    pulada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    usuario_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_itens_lista_ocorrencias", x => x.id);
                    table.ForeignKey(
                        name: "fk_itens_lista_ocorrencias_asp_net_users_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_itens_lista_ocorrencias_itens_lista_item_id",
                        column: x => x.item_id,
                        principalTable: "itens_lista",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_dias_semana_check",
                table: "itens_lista",
                sql: "dias_semana <@ ARRAY[1, 2, 3, 4, 5, 6, 7]");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_repeticao_check",
                table: "itens_lista",
                sql: "repeticao IN ('NENHUMA', 'DIARIA', 'SEMANAL', 'MENSAL')");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_repeticao_com_data_check",
                table: "itens_lista",
                sql: "repeticao = 'NENHUMA' OR data IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_repetir_ate_check",
                table: "itens_lista",
                sql: "repetir_ate >= data");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_semanal_com_dias_check",
                table: "itens_lista",
                sql: "repeticao <> 'SEMANAL' OR cardinality(dias_semana) > 0");

            migrationBuilder.CreateIndex(
                name: "ix_itens_lista_ocorrencias_item_id_data",
                table: "itens_lista_ocorrencias",
                columns: new[] { "item_id", "data" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_itens_lista_ocorrencias_usuario_id",
                table: "itens_lista_ocorrencias",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "itens_lista_ocorrencias");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_dias_semana_check",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_repeticao_check",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_repeticao_com_data_check",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_repetir_ate_check",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_semanal_com_dias_check",
                table: "itens_lista");

            migrationBuilder.DropColumn(
                name: "dias_semana",
                table: "itens_lista");

            migrationBuilder.DropColumn(
                name: "repeticao",
                table: "itens_lista");

            migrationBuilder.DropColumn(
                name: "repetir_ate",
                table: "itens_lista");

            migrationBuilder.RenameColumn(
                name: "hora",
                table: "itens_lista",
                newName: "horario_tarefa");
        }
    }
}
