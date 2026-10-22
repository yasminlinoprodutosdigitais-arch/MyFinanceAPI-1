using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class NomesDeForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bancos_usuario_usuario_id",
                table: "bancos");

            migrationBuilder.DropForeignKey(
                name: "fk_categorias_usuario_usuario_id",
                table: "categorias");

            migrationBuilder.DropForeignKey(
                name: "fk_contas_usuario_usuario_id",
                table: "contas");

            migrationBuilder.DropForeignKey(
                name: "fk_contas_vencimento_usuario_usuario_id",
                table: "contas_vencimento");

            migrationBuilder.DropForeignKey(
                name: "fk_extratos_bancarios_usuario_usuario_id",
                table: "extratos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_extratos_bancarios_extrato_bancario_",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_pessoas_movimentacao_pessoa_moviment",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_tipos_movimentacao_tipo_movimentacao",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_usuario_usuario_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_lista_usuario_usuario_id",
                table: "itens_lista");

            migrationBuilder.DropForeignKey(
                name: "fk_listas_usuario_usuario_id",
                table: "listas");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_diarias_tipos_movimentacao_tipo_movimentacao_",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_diarias_usuario_usuario_id",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "fk_pessoas_movimentacao_usuario_usuario_id",
                table: "pessoas_movimentacao");

            migrationBuilder.DropForeignKey(
                name: "fk_tipos_cartao_usuario_usuario_id",
                table: "tipos_cartao");

            migrationBuilder.DropForeignKey(
                name: "fk_tipos_movimentacao_usuario_usuario_id",
                table: "tipos_movimentacao");

            migrationBuilder.DropForeignKey(
                name: "fk_transacoes_usuario_usuario_id",
                table: "transacoes");

            migrationBuilder.AddForeignKey(
                name: "fk_bancos_asp_net_users_usuario_id",
                table: "bancos",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_categorias_asp_net_users_usuario_id",
                table: "categorias",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_contas_asp_net_users_usuario_id",
                table: "contas",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_contas_vencimento_asp_net_users_usuario_id",
                table: "contas_vencimento",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_extratos_bancarios_asp_net_users_usuario_id",
                table: "extratos_bancarios",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_asp_net_users_usuario_id",
                table: "itens_extrato_bancario",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_extrato_bancario_id",
                table: "itens_extrato_bancario",
                column: "extrato_bancario_id",
                principalTable: "extratos_bancarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_pessoa_movimentacao_id",
                table: "itens_extrato_bancario",
                column: "pessoa_movimentacao_id",
                principalTable: "pessoas_movimentacao",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_tipo_movimentacao_id",
                table: "itens_extrato_bancario",
                column: "tipo_movimentacao_id",
                principalTable: "tipos_movimentacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_lista_asp_net_users_usuario_id",
                table: "itens_lista",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_listas_asp_net_users_usuario_id",
                table: "listas",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_diarias_asp_net_users_usuario_id",
                table: "movimentacoes_diarias",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_diarias_tipo_movimentacao_id",
                table: "movimentacoes_diarias",
                column: "tipo_movimentacao_id",
                principalTable: "tipos_movimentacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pessoas_movimentacao_asp_net_users_usuario_id",
                table: "pessoas_movimentacao",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tipos_cartao_asp_net_users_usuario_id",
                table: "tipos_cartao",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tipos_movimentacao_asp_net_users_usuario_id",
                table: "tipos_movimentacao",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transacoes_asp_net_users_usuario_id",
                table: "transacoes",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bancos_asp_net_users_usuario_id",
                table: "bancos");

            migrationBuilder.DropForeignKey(
                name: "fk_categorias_asp_net_users_usuario_id",
                table: "categorias");

            migrationBuilder.DropForeignKey(
                name: "fk_contas_asp_net_users_usuario_id",
                table: "contas");

            migrationBuilder.DropForeignKey(
                name: "fk_contas_vencimento_asp_net_users_usuario_id",
                table: "contas_vencimento");

            migrationBuilder.DropForeignKey(
                name: "fk_extratos_bancarios_asp_net_users_usuario_id",
                table: "extratos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_asp_net_users_usuario_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_extrato_bancario_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_pessoa_movimentacao_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_tipo_movimentacao_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_lista_asp_net_users_usuario_id",
                table: "itens_lista");

            migrationBuilder.DropForeignKey(
                name: "fk_listas_asp_net_users_usuario_id",
                table: "listas");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_diarias_asp_net_users_usuario_id",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_diarias_tipo_movimentacao_id",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "fk_pessoas_movimentacao_asp_net_users_usuario_id",
                table: "pessoas_movimentacao");

            migrationBuilder.DropForeignKey(
                name: "fk_tipos_cartao_asp_net_users_usuario_id",
                table: "tipos_cartao");

            migrationBuilder.DropForeignKey(
                name: "fk_tipos_movimentacao_asp_net_users_usuario_id",
                table: "tipos_movimentacao");

            migrationBuilder.DropForeignKey(
                name: "fk_transacoes_asp_net_users_usuario_id",
                table: "transacoes");

            migrationBuilder.AddForeignKey(
                name: "fk_bancos_usuario_usuario_id",
                table: "bancos",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_categorias_usuario_usuario_id",
                table: "categorias",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_contas_usuario_usuario_id",
                table: "contas",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_contas_vencimento_usuario_usuario_id",
                table: "contas_vencimento",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_extratos_bancarios_usuario_usuario_id",
                table: "extratos_bancarios",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_extratos_bancarios_extrato_bancario_",
                table: "itens_extrato_bancario",
                column: "extrato_bancario_id",
                principalTable: "extratos_bancarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_pessoas_movimentacao_pessoa_moviment",
                table: "itens_extrato_bancario",
                column: "pessoa_movimentacao_id",
                principalTable: "pessoas_movimentacao",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_tipos_movimentacao_tipo_movimentacao",
                table: "itens_extrato_bancario",
                column: "tipo_movimentacao_id",
                principalTable: "tipos_movimentacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_usuario_usuario_id",
                table: "itens_extrato_bancario",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_lista_usuario_usuario_id",
                table: "itens_lista",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_listas_usuario_usuario_id",
                table: "listas",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_diarias_tipos_movimentacao_tipo_movimentacao_",
                table: "movimentacoes_diarias",
                column: "tipo_movimentacao_id",
                principalTable: "tipos_movimentacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_diarias_usuario_usuario_id",
                table: "movimentacoes_diarias",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_pessoas_movimentacao_usuario_usuario_id",
                table: "pessoas_movimentacao",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tipos_cartao_usuario_usuario_id",
                table: "tipos_cartao",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tipos_movimentacao_usuario_usuario_id",
                table: "tipos_movimentacao",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transacoes_usuario_usuario_id",
                table: "transacoes",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
