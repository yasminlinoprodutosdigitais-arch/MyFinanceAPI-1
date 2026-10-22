using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConvencoesDeNomes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AspNetUsers_UserId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Categories_CategoryId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Banco_AspNetUsers_UerId",
                table: "Banco");

            migrationBuilder.DropForeignKey(
                name: "fk_banco_tipocartao",
                table: "Banco");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "ContaVencimento_ContaId_fkey",
                table: "ContaVencimento");

            migrationBuilder.DropForeignKey(
                name: "contavencimento_user_fkey",
                table: "ContaVencimento");

            migrationBuilder.DropForeignKey(
                name: "FK_ExtratoBancario_TipoCartao_TipoCartaoId",
                table: "ExtratoBancario");

            migrationBuilder.DropForeignKey(
                name: "extratobancario_bancoid_fkey",
                table: "ExtratoBancario");

            migrationBuilder.DropForeignKey(
                name: "extratobancario_user_fkey",
                table: "ExtratoBancario");

            migrationBuilder.DropForeignKey(
                name: "FK_ExtratoBancarioItem_AspNetUsers_UserId",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropForeignKey(
                name: "extratoBancarioItem_bancoId_fkey",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropForeignKey(
                name: "extratobancarioitem_bancoid_fkey",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropForeignKey(
                name: "extratobancarioitem_extrato_fkey",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropForeignKey(
                name: "extratobancarioitem_pessoamovimentacaoid_fkey",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropForeignKey(
                name: "extratobancarioitem_tipocartaoid_fkey",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropForeignKey(
                name: "extratobancarioitem_tipomovimentacaoid_fkey",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemLista_AspNetUsers_UserId",
                table: "ItemLista");

            migrationBuilder.DropForeignKey(
                name: "itemlista_listaid_fkey",
                table: "ItemLista");

            migrationBuilder.DropForeignKey(
                name: "FK_Lista_AspNetUsers_UserId",
                table: "Lista");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacaoDiaria_AspNetUsers_UserId",
                table: "MovimentacaoDiaria");

            migrationBuilder.DropForeignKey(
                name: "movimentacaodiaria_bancoid_fkey",
                table: "MovimentacaoDiaria");

            migrationBuilder.DropForeignKey(
                name: "movimentacaodiaria_tipocartaoid_fkey",
                table: "MovimentacaoDiaria");

            migrationBuilder.DropForeignKey(
                name: "movimentacaodiaria_tipomovimentacaoid_fkey",
                table: "MovimentacaoDiaria");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaMovimentacao_AspNetUsers_UserId",
                table: "PessoaMovimentacao");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaMovimentacao_Categories_CategoriaId",
                table: "PessoaMovimentacao");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaMovimentacao_TipoMovimentacao_TipoMovimentacaoId",
                table: "PessoaMovimentacao");

            migrationBuilder.DropForeignKey(
                name: "fk_tipocartao_aspnetusers",
                table: "TipoCartao");

            migrationBuilder.DropForeignKey(
                name: "fk_tipomovimentacao_aspnetusers",
                table: "TipoMovimentacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_IdAccount",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_UserId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "transactions_parcelas_check",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "transactions_status_check",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "tipomovimentacao_pkey",
                table: "TipoMovimentacao");

            migrationBuilder.DropCheckConstraint(
                name: "tipomovimentacao_valormeta_check",
                table: "TipoMovimentacao");

            migrationBuilder.DropPrimaryKey(
                name: "tipocartao_pkey",
                table: "TipoCartao");

            migrationBuilder.DropIndex(
                name: "UX_TipoCartao_Sistema_NomeTipoCartao",
                table: "TipoCartao");

            migrationBuilder.DropPrimaryKey(
                name: "PessoaMovimentacao_pkey",
                table: "PessoaMovimentacao");

            migrationBuilder.DropPrimaryKey(
                name: "movimentacaodiaria_pkey",
                table: "MovimentacaoDiaria");

            migrationBuilder.DropPrimaryKey(
                name: "lista_pkey",
                table: "Lista");

            migrationBuilder.DropCheckConstraint(
                name: "lista_tipolista_check",
                table: "Lista");

            migrationBuilder.DropPrimaryKey(
                name: "itemlista_pkey",
                table: "ItemLista");

            migrationBuilder.DropCheckConstraint(
                name: "itemlista_quantidade_check",
                table: "ItemLista");

            migrationBuilder.DropCheckConstraint(
                name: "itemlista_status_check",
                table: "ItemLista");

            migrationBuilder.DropPrimaryKey(
                name: "ExtratoBancarioItem_pkey",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropIndex(
                name: "UX_ExtratoBancarioItem_ChaveImportacao",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropCheckConstraint(
                name: "extratobancarioitem_parcelas_check",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropCheckConstraint(
                name: "extratobancarioitem_tipolancamento_check",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropPrimaryKey(
                name: "ExtratoBancario_pkey",
                table: "ExtratoBancario");

            migrationBuilder.DropPrimaryKey(
                name: "ContaVencimento_pkey",
                table: "ContaVencimento");

            migrationBuilder.DropCheckConstraint(
                name: "contavencimento_dia_check",
                table: "ContaVencimento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "categories_naturezaoperacao_check",
                table: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "categories_status_check",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "banco_pkey",
                table: "Banco");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "accounts_parcelas_check",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "accounts_status_check",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "transacoes");

            migrationBuilder.RenameTable(
                name: "TipoMovimentacao",
                newName: "tipos_movimentacao");

            migrationBuilder.RenameTable(
                name: "TipoCartao",
                newName: "tipos_cartao");

            migrationBuilder.RenameTable(
                name: "PessoaMovimentacao",
                newName: "pessoas_movimentacao");

            migrationBuilder.RenameTable(
                name: "MovimentacaoDiaria",
                newName: "movimentacoes_diarias");

            migrationBuilder.RenameTable(
                name: "Lista",
                newName: "listas");

            migrationBuilder.RenameTable(
                name: "ItemLista",
                newName: "itens_lista");

            migrationBuilder.RenameTable(
                name: "ExtratoBancarioItem",
                newName: "extratos_bancarios_itens");

            migrationBuilder.RenameTable(
                name: "ExtratoBancario",
                newName: "extratos_bancarios");

            migrationBuilder.RenameTable(
                name: "ContaVencimento",
                newName: "contas_vencimento");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "categorias");

            migrationBuilder.RenameTable(
                name: "Banco",
                newName: "bancos");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "asp_net_user_tokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "asp_net_users");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "asp_net_user_roles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "asp_net_user_logins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "asp_net_user_claims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "asp_net_roles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "asp_net_role_claims");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "contas");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "transacoes",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Observacao",
                table: "transacoes",
                newName: "observacao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "transacoes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "QuantidadeParcelas",
                table: "transacoes",
                newName: "quantidade_parcelas");

            migrationBuilder.RenameColumn(
                name: "ParcelaAtual",
                table: "transacoes",
                newName: "parcela_atual");

            migrationBuilder.RenameColumn(
                name: "EhParcelado",
                table: "transacoes",
                newName: "eh_parcelado");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "transacoes",
                newName: "data_criacao");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "transacoes",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "transacoes",
                newName: "valor");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "transacoes",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "transacoes",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "IdAccount",
                table: "transacoes",
                newName: "conta_id");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "transacoes",
                newName: "data");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "transacoes",
                newName: "categoria_id");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_UserId_Date",
                table: "transacoes",
                newName: "ix_transacoes_usuario_id_data");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_IdAccount",
                table: "transacoes",
                newName: "ix_transacoes_conta_id");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_CategoryId",
                table: "transacoes",
                newName: "ix_transacoes_categoria_id");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "tipos_movimentacao",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tipos_movimentacao",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ValorMeta",
                table: "tipos_movimentacao",
                newName: "valor_meta");

            migrationBuilder.RenameColumn(
                name: "NomeTipoMovimentacao",
                table: "tipos_movimentacao",
                newName: "nome_tipo_movimentacao");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "tipos_movimentacao",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "tipos_movimentacao",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_TipoMovimentacao_UserId",
                table: "tipos_movimentacao",
                newName: "ix_tipos_movimentacao_usuario_id");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "tipos_cartao",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tipos_cartao",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "NomeTipoCartao",
                table: "tipos_cartao",
                newName: "nome_tipo_cartao");

            migrationBuilder.RenameColumn(
                name: "EhCredito",
                table: "tipos_cartao",
                newName: "eh_credito");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "tipos_cartao",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "UX_TipoCartao_UserId_NomeTipoCartao",
                table: "tipos_cartao",
                newName: "ux_tipos_cartao_usuario_id_nome_tipo_cartao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "pessoas_movimentacao",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TipoMovimentacaoId",
                table: "pessoas_movimentacao",
                newName: "tipo_movimentacao_id");

            migrationBuilder.RenameColumn(
                name: "NomePessoa",
                table: "pessoas_movimentacao",
                newName: "nome_pessoa");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "pessoas_movimentacao",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "pessoas_movimentacao",
                newName: "categoria_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "pessoas_movimentacao",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "UX_PessoaMovimentacao_UserId_NomePessoa",
                table: "pessoas_movimentacao",
                newName: "ux_pessoas_movimentacao_usuario_id_nome_pessoa");

            migrationBuilder.RenameIndex(
                name: "IX_PessoaMovimentacao_TipoMovimentacaoId",
                table: "pessoas_movimentacao",
                newName: "ix_pessoas_movimentacao_tipo_movimentacao_id");

            migrationBuilder.RenameIndex(
                name: "IX_PessoaMovimentacao_CategoriaId",
                table: "pessoas_movimentacao",
                newName: "ix_pessoas_movimentacao_categoria_id");

            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "movimentacoes_diarias",
                newName: "valor");

            migrationBuilder.RenameColumn(
                name: "Identificador",
                table: "movimentacoes_diarias",
                newName: "identificador");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "movimentacoes_diarias",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "movimentacoes_diarias",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TipoMovimentacaoId",
                table: "movimentacoes_diarias",
                newName: "tipo_movimentacao_id");

            migrationBuilder.RenameColumn(
                name: "TipoLancamento",
                table: "movimentacoes_diarias",
                newName: "tipo_lancamento");

            migrationBuilder.RenameColumn(
                name: "TipoCartaoId",
                table: "movimentacoes_diarias",
                newName: "tipo_cartao_id");

            migrationBuilder.RenameColumn(
                name: "NomePessoaTransacao",
                table: "movimentacoes_diarias",
                newName: "nome_pessoa_transacao");

            migrationBuilder.RenameColumn(
                name: "DataMovimentacao",
                table: "movimentacoes_diarias",
                newName: "data_movimentacao");

            migrationBuilder.RenameColumn(
                name: "BancoId",
                table: "movimentacoes_diarias",
                newName: "banco_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "movimentacoes_diarias",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoDiaria_UserId",
                table: "movimentacoes_diarias",
                newName: "ix_movimentacoes_diarias_usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoDiaria_TipoMovimentacaoId",
                table: "movimentacoes_diarias",
                newName: "ix_movimentacoes_diarias_tipo_movimentacao_id");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoDiaria_TipoCartaoId",
                table: "movimentacoes_diarias",
                newName: "ix_movimentacoes_diarias_tipo_cartao_id");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoDiaria_BancoId",
                table: "movimentacoes_diarias",
                newName: "ix_movimentacoes_diarias_banco_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "listas",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "listas",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "listas",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TipoLista",
                table: "listas",
                newName: "tipo_lista");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "listas",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "listas",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_Lista_UserId",
                table: "listas",
                newName: "ix_listas_usuario_id");

            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "itens_lista",
                newName: "valor");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "itens_lista",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "itens_lista",
                newName: "quantidade");

            migrationBuilder.RenameColumn(
                name: "Observacao",
                table: "itens_lista",
                newName: "observacao");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "itens_lista",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "itens_lista",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ListaId",
                table: "itens_lista",
                newName: "lista_id");

            migrationBuilder.RenameColumn(
                name: "HorarioTarefa",
                table: "itens_lista",
                newName: "horario_tarefa");

            migrationBuilder.RenameColumn(
                name: "DataTarefa",
                table: "itens_lista",
                newName: "data_tarefa");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "itens_lista",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "itens_lista",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_ItemLista_UserId",
                table: "itens_lista",
                newName: "ix_itens_lista_usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_ItemLista_ListaId",
                table: "itens_lista",
                newName: "ix_itens_lista_lista_id");

            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "extratos_bancarios_itens",
                newName: "valor");

            migrationBuilder.RenameColumn(
                name: "Observacao",
                table: "extratos_bancarios_itens",
                newName: "observacao");

            migrationBuilder.RenameColumn(
                name: "Identificador",
                table: "extratos_bancarios_itens",
                newName: "identificador");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "extratos_bancarios_itens",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "extratos_bancarios_itens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TipoMovimentacaoId",
                table: "extratos_bancarios_itens",
                newName: "tipo_movimentacao_id");

            migrationBuilder.RenameColumn(
                name: "TipoLancamento",
                table: "extratos_bancarios_itens",
                newName: "tipo_lancamento");

            migrationBuilder.RenameColumn(
                name: "TipoCartaoId",
                table: "extratos_bancarios_itens",
                newName: "tipo_cartao_id");

            migrationBuilder.RenameColumn(
                name: "QuantidadeParcelas",
                table: "extratos_bancarios_itens",
                newName: "quantidade_parcelas");

            migrationBuilder.RenameColumn(
                name: "PessoaMovimentacaoId",
                table: "extratos_bancarios_itens",
                newName: "pessoa_movimentacao_id");

            migrationBuilder.RenameColumn(
                name: "ParcelaAtual",
                table: "extratos_bancarios_itens",
                newName: "parcela_atual");

            migrationBuilder.RenameColumn(
                name: "NumeroFatura",
                table: "extratos_bancarios_itens",
                newName: "numero_fatura");

            migrationBuilder.RenameColumn(
                name: "NomePessoaTransacao",
                table: "extratos_bancarios_itens",
                newName: "nome_pessoa_transacao");

            migrationBuilder.RenameColumn(
                name: "GrupoParcelamento",
                table: "extratos_bancarios_itens",
                newName: "grupo_parcelamento");

            migrationBuilder.RenameColumn(
                name: "ExtratoBancarioId",
                table: "extratos_bancarios_itens",
                newName: "extrato_bancario_id");

            migrationBuilder.RenameColumn(
                name: "EhParcelado",
                table: "extratos_bancarios_itens",
                newName: "eh_parcelado");

            migrationBuilder.RenameColumn(
                name: "DataMovimentacao",
                table: "extratos_bancarios_itens",
                newName: "data_movimentacao");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "extratos_bancarios_itens",
                newName: "data_criacao");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "extratos_bancarios_itens",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "ChaveImportacao",
                table: "extratos_bancarios_itens",
                newName: "chave_importacao");

            migrationBuilder.RenameColumn(
                name: "ChaveDescricao",
                table: "extratos_bancarios_itens",
                newName: "chave_descricao");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "extratos_bancarios_itens",
                newName: "categoria_id");

            migrationBuilder.RenameColumn(
                name: "BancoId",
                table: "extratos_bancarios_itens",
                newName: "banco_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "extratos_bancarios_itens",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancarioItem_UserId_NumeroFatura",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_usuario_id_numero_fatura");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancarioItem_UserId_DataMovimentacao",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_usuario_id_data_movimentacao");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancarioItem_TipoMovimentacaoId",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_tipo_movimentacao_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancarioItem_TipoCartaoId",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_tipo_cartao_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancarioItem_PessoaMovimentacaoId",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_pessoa_movimentacao_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancarioItem_CategoriaId",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_categoria_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancarioItem_BancoId",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_banco_id");

            migrationBuilder.RenameIndex(
                name: "idx_extratobancarioitem_extrato",
                table: "extratos_bancarios_itens",
                newName: "ix_extratos_bancarios_itens_extrato_bancario_id");

            migrationBuilder.RenameColumn(
                name: "Situacao",
                table: "extratos_bancarios",
                newName: "situacao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "extratos_bancarios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ValorTotal",
                table: "extratos_bancarios",
                newName: "valor_total");

            migrationBuilder.RenameColumn(
                name: "TipoCartaoId",
                table: "extratos_bancarios",
                newName: "tipo_cartao_id");

            migrationBuilder.RenameColumn(
                name: "QuantidadeLancamentos",
                table: "extratos_bancarios",
                newName: "quantidade_lancamentos");

            migrationBuilder.RenameColumn(
                name: "NomeArquivoOrigem",
                table: "extratos_bancarios",
                newName: "nome_arquivo_origem");

            migrationBuilder.RenameColumn(
                name: "NomeArquivo",
                table: "extratos_bancarios",
                newName: "nome_arquivo");

            migrationBuilder.RenameColumn(
                name: "LoteImportacaoId",
                table: "extratos_bancarios",
                newName: "lote_importacao_id");

            migrationBuilder.RenameColumn(
                name: "DataInicioPeriodo",
                table: "extratos_bancarios",
                newName: "data_inicio_periodo");

            migrationBuilder.RenameColumn(
                name: "DataImportacao",
                table: "extratos_bancarios",
                newName: "data_importacao");

            migrationBuilder.RenameColumn(
                name: "DataFimPeriodo",
                table: "extratos_bancarios",
                newName: "data_fim_periodo");

            migrationBuilder.RenameColumn(
                name: "BancoId",
                table: "extratos_bancarios",
                newName: "banco_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "extratos_bancarios",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancario_UserId",
                table: "extratos_bancarios",
                newName: "ix_extratos_bancarios_usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancario_TipoCartaoId",
                table: "extratos_bancarios",
                newName: "ix_extratos_bancarios_tipo_cartao_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExtratoBancario_BancoId",
                table: "extratos_bancarios",
                newName: "ix_extratos_bancarios_banco_id");

            migrationBuilder.RenameColumn(
                name: "Dia",
                table: "contas_vencimento",
                newName: "dia");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "contas_vencimento",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "contas_vencimento",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "ContaId",
                table: "contas_vencimento",
                newName: "conta_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "contas_vencimento",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_ContaVencimento_UserId",
                table: "contas_vencimento",
                newName: "ix_contas_vencimento_usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_ContaVencimento_ContaId",
                table: "contas_vencimento",
                newName: "ix_contas_vencimento_conta_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "categorias",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "categorias",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "NaturezaOperacao",
                table: "categorias",
                newName: "natureza_operacao");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "categorias",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "categorias",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "SubCategory",
                table: "categorias",
                newName: "subcategoria");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "categorias",
                newName: "nome");

            migrationBuilder.RenameIndex(
                name: "UX_Categories_UserId_Name_SubCategory",
                table: "categorias",
                newName: "ux_categorias_usuario_id_nome_subcategoria");

            migrationBuilder.RenameColumn(
                name: "Ativo",
                table: "bancos",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "Agencia",
                table: "bancos",
                newName: "agencia");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "bancos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TipoCartaoId",
                table: "bancos",
                newName: "tipo_cartao_id");

            migrationBuilder.RenameColumn(
                name: "SaldoInicial",
                table: "bancos",
                newName: "saldo_inicial");

            migrationBuilder.RenameColumn(
                name: "NumeroConta",
                table: "bancos",
                newName: "numero_conta");

            migrationBuilder.RenameColumn(
                name: "NomeBanco",
                table: "bancos",
                newName: "nome_banco");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "bancos",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "bancos",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "UX_Banco_UserId_NomeBanco",
                table: "bancos",
                newName: "ux_bancos_usuario_id_nome_banco");

            migrationBuilder.RenameIndex(
                name: "IX_Banco_TipoCartaoId",
                table: "bancos",
                newName: "ix_bancos_tipo_cartao_id");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "asp_net_user_tokens",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "asp_net_user_tokens",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "asp_net_user_tokens",
                newName: "login_provider");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "asp_net_user_tokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "asp_net_users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "asp_net_users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "asp_net_users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "asp_net_users",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "TwoFactorEnabled",
                table: "asp_net_users",
                newName: "two_factor_enabled");

            migrationBuilder.RenameColumn(
                name: "StatusAtivo",
                table: "asp_net_users",
                newName: "status_ativo");

            migrationBuilder.RenameColumn(
                name: "SecurityStamp",
                table: "asp_net_users",
                newName: "security_stamp");

            migrationBuilder.RenameColumn(
                name: "PhoneNumberConfirmed",
                table: "asp_net_users",
                newName: "phone_number_confirmed");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "asp_net_users",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "asp_net_users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "NormalizedUserName",
                table: "asp_net_users",
                newName: "normalized_user_name");

            migrationBuilder.RenameColumn(
                name: "NormalizedEmail",
                table: "asp_net_users",
                newName: "normalized_email");

            migrationBuilder.RenameColumn(
                name: "NomeUsuario",
                table: "asp_net_users",
                newName: "nome_usuario");

            migrationBuilder.RenameColumn(
                name: "LockoutEnd",
                table: "asp_net_users",
                newName: "lockout_end");

            migrationBuilder.RenameColumn(
                name: "LockoutEnabled",
                table: "asp_net_users",
                newName: "lockout_enabled");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmed",
                table: "asp_net_users",
                newName: "email_confirmed");

            migrationBuilder.RenameColumn(
                name: "CriadoEm",
                table: "asp_net_users",
                newName: "criado_em");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                table: "asp_net_users",
                newName: "concurrency_stamp");

            migrationBuilder.RenameColumn(
                name: "AtualizadoEm",
                table: "asp_net_users",
                newName: "atualizado_em");

            migrationBuilder.RenameColumn(
                name: "AccessFailedCount",
                table: "asp_net_users",
                newName: "access_failed_count");

            migrationBuilder.RenameIndex(
                name: "UserNameIndex",
                table: "asp_net_users",
                newName: "user_name_index");

            migrationBuilder.RenameIndex(
                name: "EmailIndex",
                table: "asp_net_users",
                newName: "email_index");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "asp_net_user_roles",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "asp_net_user_roles",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "asp_net_user_roles",
                newName: "ix_asp_net_user_roles_role_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "asp_net_user_logins",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ProviderDisplayName",
                table: "asp_net_user_logins",
                newName: "provider_display_name");

            migrationBuilder.RenameColumn(
                name: "ProviderKey",
                table: "asp_net_user_logins",
                newName: "provider_key");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "asp_net_user_logins",
                newName: "login_provider");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "asp_net_user_logins",
                newName: "ix_asp_net_user_logins_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "asp_net_user_claims",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "asp_net_user_claims",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "asp_net_user_claims",
                newName: "claim_value");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                table: "asp_net_user_claims",
                newName: "claim_type");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "asp_net_user_claims",
                newName: "ix_asp_net_user_claims_user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "asp_net_roles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "asp_net_roles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                table: "asp_net_roles",
                newName: "normalized_name");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                table: "asp_net_roles",
                newName: "concurrency_stamp");

            migrationBuilder.RenameIndex(
                name: "RoleNameIndex",
                table: "asp_net_roles",
                newName: "role_name_index");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "asp_net_role_claims",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "asp_net_role_claims",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "asp_net_role_claims",
                newName: "claim_value");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                table: "asp_net_role_claims",
                newName: "claim_type");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "asp_net_role_claims",
                newName: "ix_asp_net_role_claims_role_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "contas",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "contas",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "QuantidadeParcelas",
                table: "contas",
                newName: "quantidade_parcelas");

            migrationBuilder.RenameColumn(
                name: "ParcelaAtual",
                table: "contas",
                newName: "parcela_atual");

            migrationBuilder.RenameColumn(
                name: "EhParcelado",
                table: "contas",
                newName: "eh_parcelado");

            migrationBuilder.RenameColumn(
                name: "DataOperacao",
                table: "contas",
                newName: "data_operacao");

            migrationBuilder.RenameColumn(
                name: "DataAlteracao",
                table: "contas",
                newName: "data_alteracao");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "contas",
                newName: "valor");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "contas",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "contas",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "contas",
                newName: "categoria_id");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_UserId",
                table: "contas",
                newName: "ix_contas_usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_CategoryId",
                table: "contas",
                newName: "ix_contas_categoria_id");

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
                name: "extratos_bancarios_itens_pkey",
                table: "extratos_bancarios_itens",
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
                name: "categorias_pkey",
                table: "categorias",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "bancos_pkey",
                table: "bancos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "asp_net_user_tokens_pkey",
                table: "asp_net_user_tokens",
                columns: new[] { "user_id", "login_provider", "name" });

            migrationBuilder.AddPrimaryKey(
                name: "asp_net_users_pkey",
                table: "asp_net_users",
                column: "id");

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
                name: "contas_pkey",
                table: "contas",
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

            migrationBuilder.CreateIndex(
                name: "ux_tipos_cartao_sistema_nome_tipo_cartao",
                table: "tipos_cartao",
                column: "nome_tipo_cartao",
                unique: true,
                filter: "\"usuario_id\" IS NULL");

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

            migrationBuilder.CreateIndex(
                name: "ux_extratos_bancarios_itens_chave_importacao",
                table: "extratos_bancarios_itens",
                columns: new[] { "usuario_id", "banco_id", "chave_importacao" },
                unique: true,
                filter: "\"chave_importacao\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "extratos_bancarios_itens_parcelas_check",
                table: "extratos_bancarios_itens",
                sql: "\"quantidade_parcelas\" >= 1 AND \"parcela_atual\" BETWEEN 1 AND \"quantidade_parcelas\"");

            migrationBuilder.AddCheckConstraint(
                name: "extratos_bancarios_itens_tipo_lancamento_check",
                table: "extratos_bancarios_itens",
                sql: "\"tipo_lancamento\" IN ('Entrada', 'Saída')");

            migrationBuilder.AddCheckConstraint(
                name: "contas_vencimento_dia_check",
                table: "contas_vencimento",
                sql: "\"dia\" BETWEEN 1 AND 31");

            migrationBuilder.AddCheckConstraint(
                name: "categorias_natureza_operacao_check",
                table: "categorias",
                sql: "\"natureza_operacao\" BETWEEN 0 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "categorias_status_check",
                table: "categorias",
                sql: "\"status\" IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "contas_parcelas_check",
                table: "contas",
                sql: "NOT \"eh_parcelado\" OR (\"quantidade_parcelas\" >= 1 AND \"parcela_atual\" BETWEEN 0 AND \"quantidade_parcelas\")");

            migrationBuilder.AddCheckConstraint(
                name: "contas_status_check",
                table: "contas",
                sql: "\"status\" IN (0, 1)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "ux_tipos_cartao_sistema_nome_tipo_cartao",
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
                name: "extratos_bancarios_itens_pkey",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropIndex(
                name: "ux_extratos_bancarios_itens_chave_importacao",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropCheckConstraint(
                name: "extratos_bancarios_itens_parcelas_check",
                table: "extratos_bancarios_itens");

            migrationBuilder.DropCheckConstraint(
                name: "extratos_bancarios_itens_tipo_lancamento_check",
                table: "extratos_bancarios_itens");

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

            migrationBuilder.RenameTable(
                name: "transacoes",
                newName: "Transactions");

            migrationBuilder.RenameTable(
                name: "tipos_movimentacao",
                newName: "TipoMovimentacao");

            migrationBuilder.RenameTable(
                name: "tipos_cartao",
                newName: "TipoCartao");

            migrationBuilder.RenameTable(
                name: "pessoas_movimentacao",
                newName: "PessoaMovimentacao");

            migrationBuilder.RenameTable(
                name: "movimentacoes_diarias",
                newName: "MovimentacaoDiaria");

            migrationBuilder.RenameTable(
                name: "listas",
                newName: "Lista");

            migrationBuilder.RenameTable(
                name: "itens_lista",
                newName: "ItemLista");

            migrationBuilder.RenameTable(
                name: "extratos_bancarios_itens",
                newName: "ExtratoBancarioItem");

            migrationBuilder.RenameTable(
                name: "extratos_bancarios",
                newName: "ExtratoBancario");

            migrationBuilder.RenameTable(
                name: "contas_vencimento",
                newName: "ContaVencimento");

            migrationBuilder.RenameTable(
                name: "contas",
                newName: "Accounts");

            migrationBuilder.RenameTable(
                name: "categorias",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "bancos",
                newName: "Banco");

            migrationBuilder.RenameTable(
                name: "asp_net_users",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "asp_net_user_tokens",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "asp_net_user_roles",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "asp_net_user_logins",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "asp_net_user_claims",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "asp_net_roles",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "asp_net_role_claims",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Transactions",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "observacao",
                table: "Transactions",
                newName: "Observacao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Transactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "quantidade_parcelas",
                table: "Transactions",
                newName: "QuantidadeParcelas");

            migrationBuilder.RenameColumn(
                name: "parcela_atual",
                table: "Transactions",
                newName: "ParcelaAtual");

            migrationBuilder.RenameColumn(
                name: "eh_parcelado",
                table: "Transactions",
                newName: "EhParcelado");

            migrationBuilder.RenameColumn(
                name: "data_criacao",
                table: "Transactions",
                newName: "DataCriacao");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "Transactions",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "valor",
                table: "Transactions",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "Transactions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Transactions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "data",
                table: "Transactions",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "conta_id",
                table: "Transactions",
                newName: "IdAccount");

            migrationBuilder.RenameColumn(
                name: "categoria_id",
                table: "Transactions",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "ix_transacoes_usuario_id_data",
                table: "Transactions",
                newName: "IX_Transactions_UserId_Date");

            migrationBuilder.RenameIndex(
                name: "ix_transacoes_conta_id",
                table: "Transactions",
                newName: "IX_Transactions_IdAccount");

            migrationBuilder.RenameIndex(
                name: "ix_transacoes_categoria_id",
                table: "Transactions",
                newName: "IX_Transactions_CategoryId");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "TipoMovimentacao",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TipoMovimentacao",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "valor_meta",
                table: "TipoMovimentacao",
                newName: "ValorMeta");

            migrationBuilder.RenameColumn(
                name: "nome_tipo_movimentacao",
                table: "TipoMovimentacao",
                newName: "NomeTipoMovimentacao");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "TipoMovimentacao",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "TipoMovimentacao",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_tipos_movimentacao_usuario_id",
                table: "TipoMovimentacao",
                newName: "IX_TipoMovimentacao_UserId");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "TipoCartao",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TipoCartao",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nome_tipo_cartao",
                table: "TipoCartao",
                newName: "NomeTipoCartao");

            migrationBuilder.RenameColumn(
                name: "eh_credito",
                table: "TipoCartao",
                newName: "EhCredito");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "TipoCartao",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ux_tipos_cartao_usuario_id_nome_tipo_cartao",
                table: "TipoCartao",
                newName: "UX_TipoCartao_UserId_NomeTipoCartao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PessoaMovimentacao",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tipo_movimentacao_id",
                table: "PessoaMovimentacao",
                newName: "TipoMovimentacaoId");

            migrationBuilder.RenameColumn(
                name: "nome_pessoa",
                table: "PessoaMovimentacao",
                newName: "NomePessoa");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "PessoaMovimentacao",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "categoria_id",
                table: "PessoaMovimentacao",
                newName: "CategoriaId");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "PessoaMovimentacao",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ux_pessoas_movimentacao_usuario_id_nome_pessoa",
                table: "PessoaMovimentacao",
                newName: "UX_PessoaMovimentacao_UserId_NomePessoa");

            migrationBuilder.RenameIndex(
                name: "ix_pessoas_movimentacao_tipo_movimentacao_id",
                table: "PessoaMovimentacao",
                newName: "IX_PessoaMovimentacao_TipoMovimentacaoId");

            migrationBuilder.RenameIndex(
                name: "ix_pessoas_movimentacao_categoria_id",
                table: "PessoaMovimentacao",
                newName: "IX_PessoaMovimentacao_CategoriaId");

            migrationBuilder.RenameColumn(
                name: "valor",
                table: "MovimentacaoDiaria",
                newName: "Valor");

            migrationBuilder.RenameColumn(
                name: "identificador",
                table: "MovimentacaoDiaria",
                newName: "Identificador");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "MovimentacaoDiaria",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "MovimentacaoDiaria",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tipo_movimentacao_id",
                table: "MovimentacaoDiaria",
                newName: "TipoMovimentacaoId");

            migrationBuilder.RenameColumn(
                name: "tipo_lancamento",
                table: "MovimentacaoDiaria",
                newName: "TipoLancamento");

            migrationBuilder.RenameColumn(
                name: "tipo_cartao_id",
                table: "MovimentacaoDiaria",
                newName: "TipoCartaoId");

            migrationBuilder.RenameColumn(
                name: "nome_pessoa_transacao",
                table: "MovimentacaoDiaria",
                newName: "NomePessoaTransacao");

            migrationBuilder.RenameColumn(
                name: "data_movimentacao",
                table: "MovimentacaoDiaria",
                newName: "DataMovimentacao");

            migrationBuilder.RenameColumn(
                name: "banco_id",
                table: "MovimentacaoDiaria",
                newName: "BancoId");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "MovimentacaoDiaria",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_diarias_usuario_id",
                table: "MovimentacaoDiaria",
                newName: "IX_MovimentacaoDiaria_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_diarias_tipo_movimentacao_id",
                table: "MovimentacaoDiaria",
                newName: "IX_MovimentacaoDiaria_TipoMovimentacaoId");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_diarias_tipo_cartao_id",
                table: "MovimentacaoDiaria",
                newName: "IX_MovimentacaoDiaria_TipoCartaoId");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_diarias_banco_id",
                table: "MovimentacaoDiaria",
                newName: "IX_MovimentacaoDiaria_BancoId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Lista",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Lista",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Lista",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tipo_lista",
                table: "Lista",
                newName: "TipoLista");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "Lista",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "Lista",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_listas_usuario_id",
                table: "Lista",
                newName: "IX_Lista_UserId");

            migrationBuilder.RenameColumn(
                name: "valor",
                table: "ItemLista",
                newName: "Valor");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "ItemLista",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "quantidade",
                table: "ItemLista",
                newName: "Quantidade");

            migrationBuilder.RenameColumn(
                name: "observacao",
                table: "ItemLista",
                newName: "Observacao");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "ItemLista",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ItemLista",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "lista_id",
                table: "ItemLista",
                newName: "ListaId");

            migrationBuilder.RenameColumn(
                name: "horario_tarefa",
                table: "ItemLista",
                newName: "HorarioTarefa");

            migrationBuilder.RenameColumn(
                name: "data_tarefa",
                table: "ItemLista",
                newName: "DataTarefa");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "ItemLista",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "ItemLista",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_itens_lista_usuario_id",
                table: "ItemLista",
                newName: "IX_ItemLista_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_itens_lista_lista_id",
                table: "ItemLista",
                newName: "IX_ItemLista_ListaId");

            migrationBuilder.RenameColumn(
                name: "valor",
                table: "ExtratoBancarioItem",
                newName: "Valor");

            migrationBuilder.RenameColumn(
                name: "observacao",
                table: "ExtratoBancarioItem",
                newName: "Observacao");

            migrationBuilder.RenameColumn(
                name: "identificador",
                table: "ExtratoBancarioItem",
                newName: "Identificador");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "ExtratoBancarioItem",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ExtratoBancarioItem",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tipo_movimentacao_id",
                table: "ExtratoBancarioItem",
                newName: "TipoMovimentacaoId");

            migrationBuilder.RenameColumn(
                name: "tipo_lancamento",
                table: "ExtratoBancarioItem",
                newName: "TipoLancamento");

            migrationBuilder.RenameColumn(
                name: "tipo_cartao_id",
                table: "ExtratoBancarioItem",
                newName: "TipoCartaoId");

            migrationBuilder.RenameColumn(
                name: "quantidade_parcelas",
                table: "ExtratoBancarioItem",
                newName: "QuantidadeParcelas");

            migrationBuilder.RenameColumn(
                name: "pessoa_movimentacao_id",
                table: "ExtratoBancarioItem",
                newName: "PessoaMovimentacaoId");

            migrationBuilder.RenameColumn(
                name: "parcela_atual",
                table: "ExtratoBancarioItem",
                newName: "ParcelaAtual");

            migrationBuilder.RenameColumn(
                name: "numero_fatura",
                table: "ExtratoBancarioItem",
                newName: "NumeroFatura");

            migrationBuilder.RenameColumn(
                name: "nome_pessoa_transacao",
                table: "ExtratoBancarioItem",
                newName: "NomePessoaTransacao");

            migrationBuilder.RenameColumn(
                name: "grupo_parcelamento",
                table: "ExtratoBancarioItem",
                newName: "GrupoParcelamento");

            migrationBuilder.RenameColumn(
                name: "extrato_bancario_id",
                table: "ExtratoBancarioItem",
                newName: "ExtratoBancarioId");

            migrationBuilder.RenameColumn(
                name: "eh_parcelado",
                table: "ExtratoBancarioItem",
                newName: "EhParcelado");

            migrationBuilder.RenameColumn(
                name: "data_movimentacao",
                table: "ExtratoBancarioItem",
                newName: "DataMovimentacao");

            migrationBuilder.RenameColumn(
                name: "data_criacao",
                table: "ExtratoBancarioItem",
                newName: "DataCriacao");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "ExtratoBancarioItem",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "chave_importacao",
                table: "ExtratoBancarioItem",
                newName: "ChaveImportacao");

            migrationBuilder.RenameColumn(
                name: "chave_descricao",
                table: "ExtratoBancarioItem",
                newName: "ChaveDescricao");

            migrationBuilder.RenameColumn(
                name: "categoria_id",
                table: "ExtratoBancarioItem",
                newName: "CategoriaId");

            migrationBuilder.RenameColumn(
                name: "banco_id",
                table: "ExtratoBancarioItem",
                newName: "BancoId");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "ExtratoBancarioItem",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_usuario_id_numero_fatura",
                table: "ExtratoBancarioItem",
                newName: "IX_ExtratoBancarioItem_UserId_NumeroFatura");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_usuario_id_data_movimentacao",
                table: "ExtratoBancarioItem",
                newName: "IX_ExtratoBancarioItem_UserId_DataMovimentacao");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_tipo_movimentacao_id",
                table: "ExtratoBancarioItem",
                newName: "IX_ExtratoBancarioItem_TipoMovimentacaoId");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_tipo_cartao_id",
                table: "ExtratoBancarioItem",
                newName: "IX_ExtratoBancarioItem_TipoCartaoId");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_pessoa_movimentacao_id",
                table: "ExtratoBancarioItem",
                newName: "IX_ExtratoBancarioItem_PessoaMovimentacaoId");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_extrato_bancario_id",
                table: "ExtratoBancarioItem",
                newName: "idx_extratobancarioitem_extrato");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_categoria_id",
                table: "ExtratoBancarioItem",
                newName: "IX_ExtratoBancarioItem_CategoriaId");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_itens_banco_id",
                table: "ExtratoBancarioItem",
                newName: "IX_ExtratoBancarioItem_BancoId");

            migrationBuilder.RenameColumn(
                name: "situacao",
                table: "ExtratoBancario",
                newName: "Situacao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ExtratoBancario",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "valor_total",
                table: "ExtratoBancario",
                newName: "ValorTotal");

            migrationBuilder.RenameColumn(
                name: "tipo_cartao_id",
                table: "ExtratoBancario",
                newName: "TipoCartaoId");

            migrationBuilder.RenameColumn(
                name: "quantidade_lancamentos",
                table: "ExtratoBancario",
                newName: "QuantidadeLancamentos");

            migrationBuilder.RenameColumn(
                name: "nome_arquivo_origem",
                table: "ExtratoBancario",
                newName: "NomeArquivoOrigem");

            migrationBuilder.RenameColumn(
                name: "nome_arquivo",
                table: "ExtratoBancario",
                newName: "NomeArquivo");

            migrationBuilder.RenameColumn(
                name: "lote_importacao_id",
                table: "ExtratoBancario",
                newName: "LoteImportacaoId");

            migrationBuilder.RenameColumn(
                name: "data_inicio_periodo",
                table: "ExtratoBancario",
                newName: "DataInicioPeriodo");

            migrationBuilder.RenameColumn(
                name: "data_importacao",
                table: "ExtratoBancario",
                newName: "DataImportacao");

            migrationBuilder.RenameColumn(
                name: "data_fim_periodo",
                table: "ExtratoBancario",
                newName: "DataFimPeriodo");

            migrationBuilder.RenameColumn(
                name: "banco_id",
                table: "ExtratoBancario",
                newName: "BancoId");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "ExtratoBancario",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_usuario_id",
                table: "ExtratoBancario",
                newName: "IX_ExtratoBancario_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_tipo_cartao_id",
                table: "ExtratoBancario",
                newName: "IX_ExtratoBancario_TipoCartaoId");

            migrationBuilder.RenameIndex(
                name: "ix_extratos_bancarios_banco_id",
                table: "ExtratoBancario",
                newName: "IX_ExtratoBancario_BancoId");

            migrationBuilder.RenameColumn(
                name: "dia",
                table: "ContaVencimento",
                newName: "Dia");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ContaVencimento",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "ContaVencimento",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "conta_id",
                table: "ContaVencimento",
                newName: "ContaId");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "ContaVencimento",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_contas_vencimento_usuario_id",
                table: "ContaVencimento",
                newName: "IX_ContaVencimento_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_contas_vencimento_conta_id",
                table: "ContaVencimento",
                newName: "IX_ContaVencimento_ContaId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Accounts",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Accounts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "quantidade_parcelas",
                table: "Accounts",
                newName: "QuantidadeParcelas");

            migrationBuilder.RenameColumn(
                name: "parcela_atual",
                table: "Accounts",
                newName: "ParcelaAtual");

            migrationBuilder.RenameColumn(
                name: "eh_parcelado",
                table: "Accounts",
                newName: "EhParcelado");

            migrationBuilder.RenameColumn(
                name: "data_operacao",
                table: "Accounts",
                newName: "DataOperacao");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "Accounts",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "valor",
                table: "Accounts",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "Accounts",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Accounts",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "categoria_id",
                table: "Accounts",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "ix_contas_usuario_id",
                table: "Accounts",
                newName: "IX_Accounts_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_contas_categoria_id",
                table: "Accounts",
                newName: "IX_Accounts_CategoryId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Categories",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Categories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "natureza_operacao",
                table: "Categories",
                newName: "NaturezaOperacao");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "Categories",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "Categories",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "subcategoria",
                table: "Categories",
                newName: "SubCategory");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Categories",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "ux_categorias_usuario_id_nome_subcategoria",
                table: "Categories",
                newName: "UX_Categories_UserId_Name_SubCategory");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "Banco",
                newName: "Ativo");

            migrationBuilder.RenameColumn(
                name: "agencia",
                table: "Banco",
                newName: "Agencia");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Banco",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tipo_cartao_id",
                table: "Banco",
                newName: "TipoCartaoId");

            migrationBuilder.RenameColumn(
                name: "saldo_inicial",
                table: "Banco",
                newName: "SaldoInicial");

            migrationBuilder.RenameColumn(
                name: "numero_conta",
                table: "Banco",
                newName: "NumeroConta");

            migrationBuilder.RenameColumn(
                name: "nome_banco",
                table: "Banco",
                newName: "NomeBanco");

            migrationBuilder.RenameColumn(
                name: "data_alteracao",
                table: "Banco",
                newName: "DataAlteracao");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "Banco",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ux_bancos_usuario_id_nome_banco",
                table: "Banco",
                newName: "UX_Banco_UserId_NomeBanco");

            migrationBuilder.RenameIndex(
                name: "ix_bancos_tipo_cartao_id",
                table: "Banco",
                newName: "IX_Banco_TipoCartaoId");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "AspNetUsers",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "AspNetUsers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetUsers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "AspNetUsers",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "two_factor_enabled",
                table: "AspNetUsers",
                newName: "TwoFactorEnabled");

            migrationBuilder.RenameColumn(
                name: "status_ativo",
                table: "AspNetUsers",
                newName: "StatusAtivo");

            migrationBuilder.RenameColumn(
                name: "security_stamp",
                table: "AspNetUsers",
                newName: "SecurityStamp");

            migrationBuilder.RenameColumn(
                name: "phone_number_confirmed",
                table: "AspNetUsers",
                newName: "PhoneNumberConfirmed");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "AspNetUsers",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "AspNetUsers",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "normalized_user_name",
                table: "AspNetUsers",
                newName: "NormalizedUserName");

            migrationBuilder.RenameColumn(
                name: "normalized_email",
                table: "AspNetUsers",
                newName: "NormalizedEmail");

            migrationBuilder.RenameColumn(
                name: "nome_usuario",
                table: "AspNetUsers",
                newName: "NomeUsuario");

            migrationBuilder.RenameColumn(
                name: "lockout_end",
                table: "AspNetUsers",
                newName: "LockoutEnd");

            migrationBuilder.RenameColumn(
                name: "lockout_enabled",
                table: "AspNetUsers",
                newName: "LockoutEnabled");

            migrationBuilder.RenameColumn(
                name: "email_confirmed",
                table: "AspNetUsers",
                newName: "EmailConfirmed");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "AspNetUsers",
                newName: "CriadoEm");

            migrationBuilder.RenameColumn(
                name: "concurrency_stamp",
                table: "AspNetUsers",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "atualizado_em",
                table: "AspNetUsers",
                newName: "AtualizadoEm");

            migrationBuilder.RenameColumn(
                name: "access_failed_count",
                table: "AspNetUsers",
                newName: "AccessFailedCount");

            migrationBuilder.RenameIndex(
                name: "user_name_index",
                table: "AspNetUsers",
                newName: "UserNameIndex");

            migrationBuilder.RenameIndex(
                name: "email_index",
                table: "AspNetUsers",
                newName: "EmailIndex");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "AspNetUserTokens",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "AspNetUserTokens",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "login_provider",
                table: "AspNetUserTokens",
                newName: "LoginProvider");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AspNetUserTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "AspNetUserRoles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AspNetUserRoles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AspNetUserLogins",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "provider_display_name",
                table: "AspNetUserLogins",
                newName: "ProviderDisplayName");

            migrationBuilder.RenameColumn(
                name: "provider_key",
                table: "AspNetUserLogins",
                newName: "ProviderKey");

            migrationBuilder.RenameColumn(
                name: "login_provider",
                table: "AspNetUserLogins",
                newName: "LoginProvider");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetUserClaims",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AspNetUserClaims",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "claim_value",
                table: "AspNetUserClaims",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "claim_type",
                table: "AspNetUserClaims",
                newName: "ClaimType");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "AspNetRoles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetRoles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "normalized_name",
                table: "AspNetRoles",
                newName: "NormalizedName");

            migrationBuilder.RenameColumn(
                name: "concurrency_stamp",
                table: "AspNetRoles",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameIndex(
                name: "role_name_index",
                table: "AspNetRoles",
                newName: "RoleNameIndex");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetRoleClaims",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "AspNetRoleClaims",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "claim_value",
                table: "AspNetRoleClaims",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "claim_type",
                table: "AspNetRoleClaims",
                newName: "ClaimType");

            migrationBuilder.RenameIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "tipomovimentacao_pkey",
                table: "TipoMovimentacao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "tipocartao_pkey",
                table: "TipoCartao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PessoaMovimentacao_pkey",
                table: "PessoaMovimentacao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "movimentacaodiaria_pkey",
                table: "MovimentacaoDiaria",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "lista_pkey",
                table: "Lista",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "itemlista_pkey",
                table: "ItemLista",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "ExtratoBancarioItem_pkey",
                table: "ExtratoBancarioItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "ExtratoBancario_pkey",
                table: "ExtratoBancario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "ContaVencimento_pkey",
                table: "ContaVencimento",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "banco_pkey",
                table: "Banco",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "transactions_parcelas_check",
                table: "Transactions",
                sql: "\"QuantidadeParcelas\" >= 1 AND \"ParcelaAtual\" BETWEEN 1 AND \"QuantidadeParcelas\"");

            migrationBuilder.AddCheckConstraint(
                name: "transactions_status_check",
                table: "Transactions",
                sql: "\"Status\" IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO')");

            migrationBuilder.AddCheckConstraint(
                name: "tipomovimentacao_valormeta_check",
                table: "TipoMovimentacao",
                sql: "\"ValorMeta\" >= 0");

            migrationBuilder.CreateIndex(
                name: "UX_TipoCartao_Sistema_NomeTipoCartao",
                table: "TipoCartao",
                column: "NomeTipoCartao",
                unique: true,
                filter: "\"UserId\" IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "lista_tipolista_check",
                table: "Lista",
                sql: "\"TipoLista\" IN (1, 2, 3)");

            migrationBuilder.AddCheckConstraint(
                name: "itemlista_quantidade_check",
                table: "ItemLista",
                sql: "\"Quantidade\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "itemlista_status_check",
                table: "ItemLista",
                sql: "\"Status\" IN ('PENDENTE', 'AGUARDANDO', 'OK')");

            migrationBuilder.CreateIndex(
                name: "UX_ExtratoBancarioItem_ChaveImportacao",
                table: "ExtratoBancarioItem",
                columns: new[] { "UserId", "BancoId", "ChaveImportacao" },
                unique: true,
                filter: "\"ChaveImportacao\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "extratobancarioitem_parcelas_check",
                table: "ExtratoBancarioItem",
                sql: "\"QuantidadeParcelas\" >= 1 AND \"ParcelaAtual\" BETWEEN 1 AND \"QuantidadeParcelas\"");

            migrationBuilder.AddCheckConstraint(
                name: "extratobancarioitem_tipolancamento_check",
                table: "ExtratoBancarioItem",
                sql: "\"TipoLancamento\" IN ('Entrada', 'Saída')");

            migrationBuilder.AddCheckConstraint(
                name: "contavencimento_dia_check",
                table: "ContaVencimento",
                sql: "\"Dia\" BETWEEN 1 AND 31");

            migrationBuilder.AddCheckConstraint(
                name: "accounts_parcelas_check",
                table: "Accounts",
                sql: "NOT \"EhParcelado\" OR (\"QuantidadeParcelas\" >= 1 AND \"ParcelaAtual\" BETWEEN 0 AND \"QuantidadeParcelas\")");

            migrationBuilder.AddCheckConstraint(
                name: "accounts_status_check",
                table: "Accounts",
                sql: "\"Status\" IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "categories_naturezaoperacao_check",
                table: "Categories",
                sql: "\"NaturezaOperacao\" BETWEEN 0 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "categories_status_check",
                table: "Categories",
                sql: "\"Status\" IN (0, 1)");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AspNetUsers_UserId",
                table: "Accounts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Categories_CategoryId",
                table: "Accounts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Banco_AspNetUsers_UerId",
                table: "Banco",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_banco_tipocartao",
                table: "Banco",
                column: "TipoCartaoId",
                principalTable: "TipoCartao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ContaVencimento_ContaId_fkey",
                table: "ContaVencimento",
                column: "ContaId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "contavencimento_user_fkey",
                table: "ContaVencimento",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtratoBancario_TipoCartao_TipoCartaoId",
                table: "ExtratoBancario",
                column: "TipoCartaoId",
                principalTable: "TipoCartao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "extratobancario_bancoid_fkey",
                table: "ExtratoBancario",
                column: "BancoId",
                principalTable: "Banco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "extratobancario_user_fkey",
                table: "ExtratoBancario",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtratoBancarioItem_AspNetUsers_UserId",
                table: "ExtratoBancarioItem",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "extratoBancarioItem_bancoId_fkey",
                table: "ExtratoBancarioItem",
                column: "CategoriaId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "extratobancarioitem_bancoid_fkey",
                table: "ExtratoBancarioItem",
                column: "BancoId",
                principalTable: "Banco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "extratobancarioitem_extrato_fkey",
                table: "ExtratoBancarioItem",
                column: "ExtratoBancarioId",
                principalTable: "ExtratoBancario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "extratobancarioitem_pessoamovimentacaoid_fkey",
                table: "ExtratoBancarioItem",
                column: "PessoaMovimentacaoId",
                principalTable: "PessoaMovimentacao",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "extratobancarioitem_tipocartaoid_fkey",
                table: "ExtratoBancarioItem",
                column: "TipoCartaoId",
                principalTable: "TipoCartao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "extratobancarioitem_tipomovimentacaoid_fkey",
                table: "ExtratoBancarioItem",
                column: "TipoMovimentacaoId",
                principalTable: "TipoMovimentacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLista_AspNetUsers_UserId",
                table: "ItemLista",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "itemlista_listaid_fkey",
                table: "ItemLista",
                column: "ListaId",
                principalTable: "Lista",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lista_AspNetUsers_UserId",
                table: "Lista",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacaoDiaria_AspNetUsers_UserId",
                table: "MovimentacaoDiaria",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "movimentacaodiaria_bancoid_fkey",
                table: "MovimentacaoDiaria",
                column: "BancoId",
                principalTable: "Banco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "movimentacaodiaria_tipocartaoid_fkey",
                table: "MovimentacaoDiaria",
                column: "TipoCartaoId",
                principalTable: "TipoCartao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "movimentacaodiaria_tipomovimentacaoid_fkey",
                table: "MovimentacaoDiaria",
                column: "TipoMovimentacaoId",
                principalTable: "TipoMovimentacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaMovimentacao_AspNetUsers_UserId",
                table: "PessoaMovimentacao",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaMovimentacao_Categories_CategoriaId",
                table: "PessoaMovimentacao",
                column: "CategoriaId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PessoaMovimentacao_TipoMovimentacao_TipoMovimentacaoId",
                table: "PessoaMovimentacao",
                column: "TipoMovimentacaoId",
                principalTable: "TipoMovimentacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tipocartao_aspnetusers",
                table: "TipoCartao",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tipomovimentacao_aspnetusers",
                table: "TipoMovimentacao",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_IdAccount",
                table: "Transactions",
                column: "IdAccount",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
