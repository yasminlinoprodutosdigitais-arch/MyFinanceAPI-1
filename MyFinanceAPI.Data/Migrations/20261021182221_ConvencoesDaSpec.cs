using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConvencoesDaSpec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "asp_net_role_claims_role_id_fkey",
                table: "asp_net_role_claims");

            migrationBuilder.DropForeignKey(
                name: "asp_net_user_claims_user_id_fkey",
                table: "asp_net_user_claims");

            migrationBuilder.DropForeignKey(
                name: "asp_net_user_logins_user_id_fkey",
                table: "asp_net_user_logins");

            migrationBuilder.DropForeignKey(
                name: "asp_net_user_roles_role_id_fkey",
                table: "asp_net_user_roles");

            migrationBuilder.DropForeignKey(
                name: "asp_net_user_roles_user_id_fkey",
                table: "asp_net_user_roles");

            migrationBuilder.DropForeignKey(
                name: "asp_net_user_tokens_user_id_fkey",
                table: "asp_net_user_tokens");

            migrationBuilder.DropForeignKey(
                name: "bancos_tipo_cartao_id_fkey",
                table: "bancos");

            migrationBuilder.DropForeignKey(
                name: "bancos_usuario_id_fkey",
                table: "bancos");

            migrationBuilder.DropForeignKey(
                name: "categorias_usuario_id_fkey",
                table: "categorias");

            migrationBuilder.DropForeignKey(
                name: "contas_categoria_id_fkey",
                table: "contas");

            migrationBuilder.DropForeignKey(
                name: "contas_usuario_id_fkey",
                table: "contas");

            migrationBuilder.DropForeignKey(
                name: "contas_vencimento_conta_id_fkey",
                table: "contas_vencimento");

            migrationBuilder.DropForeignKey(
                name: "contas_vencimento_usuario_id_fkey",
                table: "contas_vencimento");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_banco_id_fkey",
                table: "extratos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_tipo_cartao_id_fkey",
                table: "extratos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_usuario_id_fkey",
                table: "extratos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_itens_banco_id_fkey",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_itens_categoria_id_fkey",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_itens_extrato_bancario_id_fkey",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_itens_pessoa_movimentacao_id_fkey",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_itens_tipo_cartao_id_fkey",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_itens_tipo_movimentacao_id_fkey",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropForeignKey(
                name: "extratos_bancarios_itens_usuario_id_fkey",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropForeignKey(
                name: "itens_lista_lista_id_fkey",
                table: "itens_lista");

            migrationBuilder.DropForeignKey(
                name: "itens_lista_usuario_id_fkey",
                table: "itens_lista");

            migrationBuilder.DropForeignKey(
                name: "listas_usuario_id_fkey",
                table: "listas");

            migrationBuilder.DropForeignKey(
                name: "movimentacoes_diarias_banco_id_fkey",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "movimentacoes_diarias_tipo_cartao_id_fkey",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "movimentacoes_diarias_tipo_movimentacao_id_fkey",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "movimentacoes_diarias_usuario_id_fkey",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "pessoas_movimentacao_categoria_id_fkey",
                table: "pessoas_movimentacao");

            migrationBuilder.DropForeignKey(
                name: "pessoas_movimentacao_tipo_movimentacao_id_fkey",
                table: "pessoas_movimentacao");

            migrationBuilder.DropForeignKey(
                name: "pessoas_movimentacao_usuario_id_fkey",
                table: "pessoas_movimentacao");

            migrationBuilder.DropForeignKey(
                name: "tipos_cartao_usuario_id_fkey",
                table: "tipos_cartao");

            migrationBuilder.DropForeignKey(
                name: "tipos_movimentacao_usuario_id_fkey",
                table: "tipos_movimentacao");

            migrationBuilder.DropForeignKey(
                name: "transacoes_categoria_id_fkey",
                table: "transacoes");

            migrationBuilder.DropForeignKey(
                name: "transacoes_conta_id_fkey",
                table: "transacoes");

            migrationBuilder.DropForeignKey(
                name: "transacoes_usuario_id_fkey",
                table: "transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "transacoes_pkey",
                table: "transacoes");

            migrationBuilder.DropCheckConstraint(
                name: "transacoes_parcelas_check",
                table: "transacoes");

            migrationBuilder.DropCheckConstraint(
                name: "transacoes_status_check",
                table: "transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "tipos_movimentacao_pkey",
                table: "tipos_movimentacao");

            migrationBuilder.DropCheckConstraint(
                name: "tipos_movimentacao_valor_meta_check",
                table: "tipos_movimentacao");

            migrationBuilder.DropPrimaryKey(
                name: "tipos_cartao_pkey",
                table: "tipos_cartao");

            migrationBuilder.DropPrimaryKey(
                name: "pessoas_movimentacao_pkey",
                table: "pessoas_movimentacao");

            migrationBuilder.DropPrimaryKey(
                name: "movimentacoes_diarias_pkey",
                table: "movimentacoes_diarias");

            migrationBuilder.DropPrimaryKey(
                name: "listas_pkey",
                table: "listas");

            migrationBuilder.DropCheckConstraint(
                name: "listas_tipo_lista_check",
                table: "listas");

            migrationBuilder.DropPrimaryKey(
                name: "itens_lista_pkey",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_quantidade_check",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_status_check",
                table: "itens_lista");

            migrationBuilder.DropPrimaryKey(
                name: "extratos_bancarios_pkey",
                table: "extratos_bancarios");

            migrationBuilder.DropPrimaryKey(
                name: "contas_vencimento_pkey",
                table: "contas_vencimento");

            migrationBuilder.DropCheckConstraint(
                name: "contas_vencimento_dia_check",
                table: "contas_vencimento");

            migrationBuilder.DropPrimaryKey(
                name: "contas_pkey",
                table: "contas");

            migrationBuilder.DropCheckConstraint(
                name: "contas_parcelas_check",
                table: "contas");

            migrationBuilder.DropCheckConstraint(
                name: "contas_status_check",
                table: "contas");

            migrationBuilder.DropPrimaryKey(
                name: "categorias_pkey",
                table: "categorias");

            migrationBuilder.DropCheckConstraint(
                name: "categorias_natureza_operacao_check",
                table: "categorias");

            migrationBuilder.DropCheckConstraint(
                name: "categorias_status_check",
                table: "categorias");

            migrationBuilder.DropPrimaryKey(
                name: "bancos_pkey",
                table: "bancos");

            migrationBuilder.DropPrimaryKey(
                name: "asp_net_users_pkey",
                table: "asp_net_users");

            migrationBuilder.DropPrimaryKey(
                name: "asp_net_user_tokens_pkey",
                table: "asp_net_user_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "asp_net_user_roles_pkey",
                table: "asp_net_user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "asp_net_user_logins_pkey",
                table: "asp_net_user_logins");

            migrationBuilder.DropPrimaryKey(
                name: "asp_net_user_claims_pkey",
                table: "asp_net_user_claims");

            migrationBuilder.DropPrimaryKey(
                name: "asp_net_roles_pkey",
                table: "asp_net_roles");

            migrationBuilder.DropPrimaryKey(
                name: "asp_net_role_claims_pkey",
                table: "asp_net_role_claims");

            migrationBuilder.DropPrimaryKey(
                name: "extratos_bancarios_itens_pkey",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropCheckConstraint(
                name: "extratos_bancarios_itens_parcelas_check",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropCheckConstraint(
                name: "extratos_bancarios_itens_tipo_lancamento_check",
                table: "extratos_bancarios_itens");

            migrationBuilder.RenameTable(
                name: "extratos_bancarios_itens",
                newName: "itens_extrato_bancario");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "transacoes",
                newName: "situacao");

            migrationBuilder.RenameIndex(
                name: "ux_tipos_cartao_usuario_id_nome_tipo_cartao",
                table: "tipos_cartao",
                newName: "ix_tipos_cartao_usuario_id_nome_tipo_cartao");

            migrationBuilder.RenameIndex(
                name: "ux_pessoas_movimentacao_usuario_id_nome_pessoa",
                table: "pessoas_movimentacao",
                newName: "ix_pessoas_movimentacao_usuario_id_nome_pessoa");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "listas",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "itens_lista",
                newName: "situacao");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "contas",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "categorias",
                newName: "ativo");

            migrationBuilder.RenameIndex(
                name: "ux_categorias_usuario_id_nome_subcategoria",
                table: "categorias",
                newName: "ix_categorias_usuario_id_nome_subcategoria");

            migrationBuilder.RenameIndex(
                name: "ux_bancos_usuario_id_nome_banco",
                table: "bancos",
                newName: "ix_bancos_usuario_id_nome_banco");

            migrationBuilder.RenameColumn(
                name: "status_ativo",
                table: "asp_net_users",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "asp_net_users",
                newName: "perfil");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "asp_net_users",
                newName: "data_criacao");

            migrationBuilder.RenameColumn(
                name: "atualizado_em",
                table: "asp_net_users",
                newName: "data_alteracao");

            migrationBuilder.RenameIndex(
                name: "ux_extratos_bancarios_itens_chave_importacao",
                table: "itens_extrato_bancario",
                newName: "ux_itens_extrato_bancario_chave_importacao");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_usuario_id_numero_fatura",
                table: "itens_extrato_bancario",
                newName: "ix_itens_extrato_bancario_usuario_id_numero_fatura");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_usuario_id_data_movimentacao",
                table: "itens_extrato_bancario",
                newName: "ix_itens_extrato_bancario_usuario_id_data_movimentacao");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_tipo_movimentacao_id",
                table: "itens_extrato_bancario",
                newName: "ix_itens_extrato_bancario_tipo_movimentacao_id");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_tipo_cartao_id",
                table: "itens_extrato_bancario",
                newName: "ix_itens_extrato_bancario_tipo_cartao_id");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_pessoa_movimentacao_id",
                table: "itens_extrato_bancario",
                newName: "ix_itens_extrato_bancario_pessoa_movimentacao_id");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_extrato_bancario_id",
                table: "itens_extrato_bancario",
                newName: "ix_itens_extrato_bancario_extrato_bancario_id");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_categoria_id",
                table: "itens_extrato_bancario",
                newName: "ix_itens_extrato_bancario_categoria_id");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_banco_id",
                table: "itens_extrato_bancario",
                newName: "ix_itens_extrato_bancario_banco_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_transacoes",
                table: "transacoes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tipos_movimentacao",
                table: "tipos_movimentacao",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tipos_cartao",
                table: "tipos_cartao",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pessoas_movimentacao",
                table: "pessoas_movimentacao",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_movimentacoes_diarias",
                table: "movimentacoes_diarias",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_listas",
                table: "listas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_itens_lista",
                table: "itens_lista",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_extratos_bancarios",
                table: "extratos_bancarios",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_contas_vencimento",
                table: "contas_vencimento",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_contas",
                table: "contas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_categorias",
                table: "categorias",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bancos",
                table: "bancos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_users",
                table: "asp_net_users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_user_tokens",
                table: "asp_net_user_tokens",
                columns: new[] { "user_id", "login_provider", "name" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_user_roles",
                table: "asp_net_user_roles",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_user_logins",
                table: "asp_net_user_logins",
                columns: new[] { "login_provider", "provider_key" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_user_claims",
                table: "asp_net_user_claims",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_roles",
                table: "asp_net_roles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_asp_net_role_claims",
                table: "asp_net_role_claims",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_itens_extrato_bancario",
                table: "itens_extrato_bancario",
                column: "id");

            migrationBuilder.AddCheckConstraint(
                name: "transacoes_parcelas_check",
                table: "transacoes",
                sql: "quantidade_parcelas >= 1 AND parcela_atual BETWEEN 1 AND quantidade_parcelas");

            migrationBuilder.AddCheckConstraint(
                name: "transacoes_situacao_check",
                table: "transacoes",
                sql: "situacao IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO')");

            migrationBuilder.AddCheckConstraint(
                name: "tipos_movimentacao_valor_meta_check",
                table: "tipos_movimentacao",
                sql: "valor_meta >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "listas_tipo_lista_check",
                table: "listas",
                sql: "tipo_lista IN (1, 2, 3)");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_quantidade_check",
                table: "itens_lista",
                sql: "quantidade >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_situacao_check",
                table: "itens_lista",
                sql: "situacao IN ('PENDENTE', 'AGUARDANDO', 'OK')");

            migrationBuilder.AddCheckConstraint(
                name: "contas_vencimento_dia_check",
                table: "contas_vencimento",
                sql: "dia BETWEEN 1 AND 31");

            migrationBuilder.AddCheckConstraint(
                name: "contas_ativo_check",
                table: "contas",
                sql: "ativo IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "contas_parcelas_check",
                table: "contas",
                sql: "NOT eh_parcelado OR (quantidade_parcelas >= 1 AND parcela_atual BETWEEN 0 AND quantidade_parcelas)");

            migrationBuilder.AddCheckConstraint(
                name: "categorias_ativo_check",
                table: "categorias",
                sql: "ativo IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "categorias_natureza_operacao_check",
                table: "categorias",
                sql: "natureza_operacao BETWEEN 0 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "itens_extrato_bancario_parcelas_check",
                table: "itens_extrato_bancario",
                sql: "quantidade_parcelas >= 1 AND parcela_atual BETWEEN 1 AND quantidade_parcelas");

            migrationBuilder.AddCheckConstraint(
                name: "itens_extrato_bancario_tipo_lancamento_check",
                table: "itens_extrato_bancario",
                sql: "tipo_lancamento IN ('Entrada', 'Saída')");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                table: "asp_net_role_claims",
                column: "role_id",
                principalTable: "asp_net_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_claims_asp_net_users_user_id",
                table: "asp_net_user_claims",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_logins_asp_net_users_user_id",
                table: "asp_net_user_logins",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                table: "asp_net_user_roles",
                column: "role_id",
                principalTable: "asp_net_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_roles_asp_net_users_user_id",
                table: "asp_net_user_roles",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                table: "asp_net_user_tokens",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bancos_tipos_cartao_tipo_cartao_id",
                table: "bancos",
                column: "tipo_cartao_id",
                principalTable: "tipos_cartao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
                name: "fk_contas_categorias_categoria_id",
                table: "contas",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_contas_usuario_usuario_id",
                table: "contas",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_contas_vencimento_contas_conta_id",
                table: "contas_vencimento",
                column: "conta_id",
                principalTable: "contas",
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
                name: "fk_extratos_bancarios_bancos_banco_id",
                table: "extratos_bancarios",
                column: "banco_id",
                principalTable: "bancos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_extratos_bancarios_tipos_cartao_tipo_cartao_id",
                table: "extratos_bancarios",
                column: "tipo_cartao_id",
                principalTable: "tipos_cartao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_extratos_bancarios_usuario_usuario_id",
                table: "extratos_bancarios",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_bancos_banco_id",
                table: "itens_extrato_bancario",
                column: "banco_id",
                principalTable: "bancos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_itens_extrato_bancario_categorias_categoria_id",
                table: "itens_extrato_bancario",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id");

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
                name: "fk_itens_extrato_bancario_tipos_cartao_tipo_cartao_id",
                table: "itens_extrato_bancario",
                column: "tipo_cartao_id",
                principalTable: "tipos_cartao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
                name: "fk_itens_lista_listas_lista_id",
                table: "itens_lista",
                column: "lista_id",
                principalTable: "listas",
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
                name: "fk_movimentacoes_diarias_bancos_banco_id",
                table: "movimentacoes_diarias",
                column: "banco_id",
                principalTable: "bancos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_diarias_tipos_cartao_tipo_cartao_id",
                table: "movimentacoes_diarias",
                column: "tipo_cartao_id",
                principalTable: "tipos_cartao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
                name: "fk_pessoas_movimentacao_categorias_categoria_id",
                table: "pessoas_movimentacao",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pessoas_movimentacao_tipos_movimentacao_tipo_movimentacao_id",
                table: "pessoas_movimentacao",
                column: "tipo_movimentacao_id",
                principalTable: "tipos_movimentacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
                name: "fk_transacoes_categorias_categoria_id",
                table: "transacoes",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transacoes_contas_conta_id",
                table: "transacoes",
                column: "conta_id",
                principalTable: "contas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transacoes_usuario_usuario_id",
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
                name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                table: "asp_net_role_claims");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_claims_asp_net_users_user_id",
                table: "asp_net_user_claims");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_logins_asp_net_users_user_id",
                table: "asp_net_user_logins");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                table: "asp_net_user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_roles_asp_net_users_user_id",
                table: "asp_net_user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                table: "asp_net_user_tokens");

            migrationBuilder.DropForeignKey(
                name: "fk_bancos_tipos_cartao_tipo_cartao_id",
                table: "bancos");

            migrationBuilder.DropForeignKey(
                name: "fk_bancos_usuario_usuario_id",
                table: "bancos");

            migrationBuilder.DropForeignKey(
                name: "fk_categorias_usuario_usuario_id",
                table: "categorias");

            migrationBuilder.DropForeignKey(
                name: "fk_contas_categorias_categoria_id",
                table: "contas");

            migrationBuilder.DropForeignKey(
                name: "fk_contas_usuario_usuario_id",
                table: "contas");

            migrationBuilder.DropForeignKey(
                name: "fk_contas_vencimento_contas_conta_id",
                table: "contas_vencimento");

            migrationBuilder.DropForeignKey(
                name: "fk_contas_vencimento_usuario_usuario_id",
                table: "contas_vencimento");

            migrationBuilder.DropForeignKey(
                name: "fk_extratos_bancarios_bancos_banco_id",
                table: "extratos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "fk_extratos_bancarios_tipos_cartao_tipo_cartao_id",
                table: "extratos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "fk_extratos_bancarios_usuario_usuario_id",
                table: "extratos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_bancos_banco_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_categorias_categoria_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_extratos_bancarios_extrato_bancario_",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_pessoas_movimentacao_pessoa_moviment",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_tipos_cartao_tipo_cartao_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_tipos_movimentacao_tipo_movimentacao",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_extrato_bancario_usuario_usuario_id",
                table: "itens_extrato_bancario");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_lista_listas_lista_id",
                table: "itens_lista");

            migrationBuilder.DropForeignKey(
                name: "fk_itens_lista_usuario_usuario_id",
                table: "itens_lista");

            migrationBuilder.DropForeignKey(
                name: "fk_listas_usuario_usuario_id",
                table: "listas");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_diarias_bancos_banco_id",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_diarias_tipos_cartao_tipo_cartao_id",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_diarias_tipos_movimentacao_tipo_movimentacao_",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_diarias_usuario_usuario_id",
                table: "movimentacoes_diarias");

            migrationBuilder.DropForeignKey(
                name: "fk_pessoas_movimentacao_categorias_categoria_id",
                table: "pessoas_movimentacao");

            migrationBuilder.DropForeignKey(
                name: "fk_pessoas_movimentacao_tipos_movimentacao_tipo_movimentacao_id",
                table: "pessoas_movimentacao");

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
                name: "fk_transacoes_categorias_categoria_id",
                table: "transacoes");

            migrationBuilder.DropForeignKey(
                name: "fk_transacoes_contas_conta_id",
                table: "transacoes");

            migrationBuilder.DropForeignKey(
                name: "fk_transacoes_usuario_usuario_id",
                table: "transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_transacoes",
                table: "transacoes");

            migrationBuilder.DropCheckConstraint(
                name: "transacoes_parcelas_check",
                table: "transacoes");

            migrationBuilder.DropCheckConstraint(
                name: "transacoes_situacao_check",
                table: "transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tipos_movimentacao",
                table: "tipos_movimentacao");

            migrationBuilder.DropCheckConstraint(
                name: "tipos_movimentacao_valor_meta_check",
                table: "tipos_movimentacao");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tipos_cartao",
                table: "tipos_cartao");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pessoas_movimentacao",
                table: "pessoas_movimentacao");

            migrationBuilder.DropPrimaryKey(
                name: "pk_movimentacoes_diarias",
                table: "movimentacoes_diarias");

            migrationBuilder.DropPrimaryKey(
                name: "pk_listas",
                table: "listas");

            migrationBuilder.DropCheckConstraint(
                name: "listas_tipo_lista_check",
                table: "listas");

            migrationBuilder.DropPrimaryKey(
                name: "pk_itens_lista",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_quantidade_check",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_situacao_check",
                table: "itens_lista");

            migrationBuilder.DropPrimaryKey(
                name: "pk_extratos_bancarios",
                table: "extratos_bancarios");

            migrationBuilder.DropPrimaryKey(
                name: "pk_contas_vencimento",
                table: "contas_vencimento");

            migrationBuilder.DropCheckConstraint(
                name: "contas_vencimento_dia_check",
                table: "contas_vencimento");

            migrationBuilder.DropPrimaryKey(
                name: "pk_contas",
                table: "contas");

            migrationBuilder.DropCheckConstraint(
                name: "contas_ativo_check",
                table: "contas");

            migrationBuilder.DropCheckConstraint(
                name: "contas_parcelas_check",
                table: "contas");

            migrationBuilder.DropPrimaryKey(
                name: "pk_categorias",
                table: "categorias");

            migrationBuilder.DropCheckConstraint(
                name: "categorias_ativo_check",
                table: "categorias");

            migrationBuilder.DropCheckConstraint(
                name: "categorias_natureza_operacao_check",
                table: "categorias");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bancos",
                table: "bancos");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_users",
                table: "asp_net_users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_user_tokens",
                table: "asp_net_user_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_user_roles",
                table: "asp_net_user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_user_logins",
                table: "asp_net_user_logins");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_user_claims",
                table: "asp_net_user_claims");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_roles",
                table: "asp_net_roles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_asp_net_role_claims",
                table: "asp_net_role_claims");

            migrationBuilder.DropPrimaryKey(
                name: "pk_itens_extrato_bancario",
                table: "itens_extrato_bancario");

            migrationBuilder.DropCheckConstraint(
                name: "itens_extrato_bancario_parcelas_check",
                table: "itens_extrato_bancario");

            migrationBuilder.DropCheckConstraint(
                name: "itens_extrato_bancario_tipo_lancamento_check",
                table: "itens_extrato_bancario");

            migrationBuilder.RenameTable(
                name: "itens_extrato_bancario",
                newName: "extratos_bancarios_itens");

            migrationBuilder.RenameColumn(
                name: "situacao",
                table: "transacoes",
                newName: "status");

            migrationBuilder.RenameIndex(
                name: "ix_tipos_cartao_usuario_id_nome_tipo_cartao",
                table: "tipos_cartao",
                newName: "ux_tipos_cartao_usuario_id_nome_tipo_cartao");

            migrationBuilder.RenameIndex(
                name: "ix_pessoas_movimentacao_usuario_id_nome_pessoa",
                table: "pessoas_movimentacao",
                newName: "ux_pessoas_movimentacao_usuario_id_nome_pessoa");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "listas",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "situacao",
                table: "itens_lista",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "contas",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "categorias",
                newName: "status");

            migrationBuilder.RenameIndex(
                name: "ix_categorias_usuario_id_nome_subcategoria",
                table: "categorias",
                newName: "ux_categorias_usuario_id_nome_subcategoria");

            migrationBuilder.RenameIndex(
                name: "ix_bancos_usuario_id_nome_banco",
                table: "bancos",
                newName: "ux_bancos_usuario_id_nome_banco");

            migrationBuilder.RenameColumn(
                name: "perfil",
                table: "asp_net_users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "data_criacao",
                table: "asp_net_users",
                newName: "criado_em");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "asp_net_users",
                newName: "atualizado_em");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "asp_net_users",
                newName: "status_ativo");

            migrationBuilder.RenameIndex(
                name: "ux_itens_extrato_bancario_chave_importacao",
                table: "extratos_bancarios_itens",
                newName: "ux_extratos_bancarios_itens_chave_importacao");

            migrationBuilder.RenameIndex(
                name: "ix_itens_extrato_bancario_usuario_id_numero_fatura",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_usuario_id_numero_fatura");

            migrationBuilder.RenameIndex(
                name: "ix_itens_extrato_bancario_usuario_id_data_movimentacao",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_usuario_id_data_movimentacao");

            migrationBuilder.RenameIndex(
                name: "ix_itens_extrato_bancario_tipo_movimentacao_id",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_tipo_movimentacao_id");

            migrationBuilder.RenameIndex(
                name: "ix_itens_extrato_bancario_tipo_cartao_id",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_tipo_cartao_id");

            migrationBuilder.RenameIndex(
                name: "ix_itens_extrato_bancario_pessoa_movimentacao_id",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_pessoa_movimentacao_id");

            migrationBuilder.RenameIndex(
                name: "ix_itens_extrato_bancario_extrato_bancario_id",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_extrato_bancario_id");

            migrationBuilder.RenameIndex(
                name: "ix_itens_extrato_bancario_categoria_id",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_categoria_id");

            migrationBuilder.RenameIndex(
                name: "ix_itens_extrato_bancario_banco_id",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_banco_id");

            migrationBuilder.AddPrimaryKey(
                name: "transacoes_pkey",
                table: "transacoes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "tipos_movimentacao_pkey",
                table: "tipos_movimentacao",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "tipos_cartao_pkey",
                table: "tipos_cartao",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pessoas_movimentacao_pkey",
                table: "pessoas_movimentacao",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "movimentacoes_diarias_pkey",
                table: "movimentacoes_diarias",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "listas_pkey",
                table: "listas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "itens_lista_pkey",
                table: "itens_lista",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "extratos_bancarios_pkey",
                table: "extratos_bancarios",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "contas_vencimento_pkey",
                table: "contas_vencimento",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "contas_pkey",
                table: "contas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "categorias_pkey",
                table: "categorias",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "bancos_pkey",
                table: "bancos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "asp_net_users_pkey",
                table: "asp_net_users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "asp_net_user_tokens_pkey",
                table: "asp_net_user_tokens",
                columns: new[] { "user_id", "login_provider", "name" });

            migrationBuilder.AddPrimaryKey(
                name: "asp_net_user_roles_pkey",
                table: "asp_net_user_roles",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "asp_net_user_logins_pkey",
                table: "asp_net_user_logins",
                columns: new[] { "login_provider", "provider_key" });

            migrationBuilder.AddPrimaryKey(
                name: "asp_net_user_claims_pkey",
                table: "asp_net_user_claims",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "asp_net_roles_pkey",
                table: "asp_net_roles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "asp_net_role_claims_pkey",
                table: "asp_net_role_claims",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "extratos_bancarios_itens_pkey",
                table: "extratos_bancarios_itens",
                column: "id");

            migrationBuilder.AddCheckConstraint(
                name: "transacoes_parcelas_check",
                table: "transacoes",
                sql: "\"quantidade_parcelas\" >= 1 AND \"parcela_atual\" BETWEEN 1 AND \"quantidade_parcelas\"");

            migrationBuilder.AddCheckConstraint(
                name: "transacoes_status_check",
                table: "transacoes",
                sql: "\"status\" IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO')");

            migrationBuilder.AddCheckConstraint(
                name: "tipos_movimentacao_valor_meta_check",
                table: "tipos_movimentacao",
                sql: "\"valor_meta\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "listas_tipo_lista_check",
                table: "listas",
                sql: "\"tipo_lista\" IN (1, 2, 3)");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_quantidade_check",
                table: "itens_lista",
                sql: "\"quantidade\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_status_check",
                table: "itens_lista",
                sql: "\"status\" IN ('PENDENTE', 'AGUARDANDO', 'OK')");

            migrationBuilder.AddCheckConstraint(
                name: "contas_vencimento_dia_check",
                table: "contas_vencimento",
                sql: "\"dia\" BETWEEN 1 AND 31");

            migrationBuilder.AddCheckConstraint(
                name: "contas_parcelas_check",
                table: "contas",
                sql: "NOT \"eh_parcelado\" OR (\"quantidade_parcelas\" >= 1 AND \"parcela_atual\" BETWEEN 0 AND \"quantidade_parcelas\")");

            migrationBuilder.AddCheckConstraint(
                name: "contas_status_check",
                table: "contas",
                sql: "\"status\" IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "categorias_natureza_operacao_check",
                table: "categorias",
                sql: "\"natureza_operacao\" BETWEEN 0 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "categorias_status_check",
                table: "categorias",
                sql: "\"status\" IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "extratos_bancarios_itens_parcelas_check",
                table: "extratos_bancarios_itens",
                sql: "\"quantidade_parcelas\" >= 1 AND \"parcela_atual\" BETWEEN 1 AND \"quantidade_parcelas\"");

            migrationBuilder.AddCheckConstraint(
                name: "extratos_bancarios_itens_tipo_lancamento_check",
                table: "extratos_bancarios_itens",
                sql: "\"tipo_lancamento\" IN ('Entrada', 'Saída')");

            migrationBuilder.AddForeignKey(
                name: "asp_net_role_claims_role_id_fkey",
                table: "asp_net_role_claims",
                column: "role_id",
                principalTable: "asp_net_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "asp_net_user_claims_user_id_fkey",
                table: "asp_net_user_claims",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "asp_net_user_logins_user_id_fkey",
                table: "asp_net_user_logins",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "asp_net_user_roles_role_id_fkey",
                table: "asp_net_user_roles",
                column: "role_id",
                principalTable: "asp_net_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "asp_net_user_roles_user_id_fkey",
                table: "asp_net_user_roles",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "asp_net_user_tokens_user_id_fkey",
                table: "asp_net_user_tokens",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "bancos_tipo_cartao_id_fkey",
                table: "bancos",
                column: "tipo_cartao_id",
                principalTable: "tipos_cartao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "bancos_usuario_id_fkey",
                table: "bancos",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "categorias_usuario_id_fkey",
                table: "categorias",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "contas_categoria_id_fkey",
                table: "contas",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "contas_usuario_id_fkey",
                table: "contas",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "contas_vencimento_conta_id_fkey",
                table: "contas_vencimento",
                column: "conta_id",
                principalTable: "contas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "contas_vencimento_usuario_id_fkey",
                table: "contas_vencimento",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_banco_id_fkey",
                table: "extratos_bancarios",
                column: "banco_id",
                principalTable: "bancos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_tipo_cartao_id_fkey",
                table: "extratos_bancarios",
                column: "tipo_cartao_id",
                principalTable: "tipos_cartao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_usuario_id_fkey",
                table: "extratos_bancarios",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_itens_banco_id_fkey",
                table: "extratos_bancarios_itens",
                column: "banco_id",
                principalTable: "bancos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_itens_categoria_id_fkey",
                table: "extratos_bancarios_itens",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_itens_extrato_bancario_id_fkey",
                table: "extratos_bancarios_itens",
                column: "extrato_bancario_id",
                principalTable: "extratos_bancarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_itens_pessoa_movimentacao_id_fkey",
                table: "extratos_bancarios_itens",
                column: "pessoa_movimentacao_id",
                principalTable: "pessoas_movimentacao",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_itens_tipo_cartao_id_fkey",
                table: "extratos_bancarios_itens",
                column: "tipo_cartao_id",
                principalTable: "tipos_cartao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_itens_tipo_movimentacao_id_fkey",
                table: "extratos_bancarios_itens",
                column: "tipo_movimentacao_id",
                principalTable: "tipos_movimentacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "extratos_bancarios_itens_usuario_id_fkey",
                table: "extratos_bancarios_itens",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "itens_lista_lista_id_fkey",
                table: "itens_lista",
                column: "lista_id",
                principalTable: "listas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "itens_lista_usuario_id_fkey",
                table: "itens_lista",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "listas_usuario_id_fkey",
                table: "listas",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "movimentacoes_diarias_banco_id_fkey",
                table: "movimentacoes_diarias",
                column: "banco_id",
                principalTable: "bancos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "movimentacoes_diarias_tipo_cartao_id_fkey",
                table: "movimentacoes_diarias",
                column: "tipo_cartao_id",
                principalTable: "tipos_cartao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "movimentacoes_diarias_tipo_movimentacao_id_fkey",
                table: "movimentacoes_diarias",
                column: "tipo_movimentacao_id",
                principalTable: "tipos_movimentacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "movimentacoes_diarias_usuario_id_fkey",
                table: "movimentacoes_diarias",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "pessoas_movimentacao_categoria_id_fkey",
                table: "pessoas_movimentacao",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "pessoas_movimentacao_tipo_movimentacao_id_fkey",
                table: "pessoas_movimentacao",
                column: "tipo_movimentacao_id",
                principalTable: "tipos_movimentacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "pessoas_movimentacao_usuario_id_fkey",
                table: "pessoas_movimentacao",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "tipos_cartao_usuario_id_fkey",
                table: "tipos_cartao",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "tipos_movimentacao_usuario_id_fkey",
                table: "tipos_movimentacao",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "transacoes_categoria_id_fkey",
                table: "transacoes",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "transacoes_conta_id_fkey",
                table: "transacoes",
                column: "conta_id",
                principalTable: "contas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "transacoes_usuario_id_fkey",
                table: "transacoes",
                column: "usuario_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
