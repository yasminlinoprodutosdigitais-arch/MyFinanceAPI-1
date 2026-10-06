using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <summary>
    /// Leva o banco do estado registrado no Baseline (produção em 03/10/2026) ao modelo atual.
    /// Escrita à mão: o snapshot já descreve o modelo final, então o EF gera esta migration vazia.
    /// Conferência: script(0 → AplicaDecisoes) tem que bater com `dotnet ef dbcontext script`.
    /// Decisões D-1…D-9 e "Modelo vence": card "[DB] P0 — Schema e integridade" e
    /// database/schema/drift-report.md.
    /// </summary>
    public partial class AplicaDecisoes : Migration
    {
        // Colunas aceitas como nulas em produção, com zero nulos na contagem de 03/10/2026.
        private static readonly (string Tabela, string Coluna)[] ColunasNotNull =
        {
            ("Accounts", "Value"),
            ("Accounts", "EhParcelado"),
            ("Banco", "SaldoInicial"),
            ("Banco", "TipoCartaoId"),
            ("Banco", "UserId"),
            ("Categories", "NaturezaOperacao"),
            ("ContaVencimento", "UserId"),
            ("ExtratoBancario", "BancoId"),
            ("ExtratoBancario", "LoteImportacaoId"),
            ("ExtratoBancarioItem", "UserId"),
            ("Lista", "Status"),
            ("Lista", "TipoMovimentacao"),
            ("MovimentacaoDiaria", "TipoLancamento"),
            ("PessoaMovimentacao", "NomePessoa"),
            ("PessoaMovimentacao", "UserId"),
            ("TipoCartao", "UserId"),
            ("TipoMovimentacao", "Descricao"),
            ("TipoMovimentacao", "UserId"),
            ("TipoMovimentacao", "ValorMeta"),
        };

        // D-5: os valores gravados são UTC (DEFAULT now() em sessão UTC e DateTime.UtcNow no código).
        private static readonly (string Tabela, string Coluna)[] ColunasTimestamptz =
        {
            ("Accounts", "DataAlteracao"),
            ("Banco", "DataAlteracao"),
            ("Categories", "DataAlteracao"),
            ("ContaVencimento", "DataAlteracao"),
            ("ExtratoBancario", "DataImportacao"),
            ("ExtratoBancarioItem", "DataAlteracao"),
            ("ItemLista", "DataAlteracao"),
            ("Lista", "DataAlteracao"),
            ("PessoaMovimentacao", "DataAlteracao"),
            ("TipoMovimentacao", "DataAlteracao"),
            ("Transactions", "DataAlteracao"),
        };

        // D-2, D-7, D-8, D-9: FKs existentes que mudam de ON DELETE.
        private static readonly (string Tabela, string Nome, string Coluna, string Principal, ReferentialAction Antes, ReferentialAction Depois)[] FksAlteradas =
        {
            ("Banco", "fk_banco_tipocartao", "TipoCartaoId", "TipoCartao", ReferentialAction.NoAction, ReferentialAction.Restrict),
            ("ContaVencimento", "ContaVencimento_ContaId_fkey", "ContaId", "Accounts", ReferentialAction.NoAction, ReferentialAction.Cascade),
            ("ContaVencimento", "contavencimento_user_fkey", "UserId", "AspNetUsers", ReferentialAction.NoAction, ReferentialAction.Cascade),
            ("ExtratoBancario", "extratobancario_bancoid_fkey", "BancoId", "Banco", ReferentialAction.SetNull, ReferentialAction.Restrict),
            ("ExtratoBancarioItem", "FK_ExtratoBancarioItem_AspNetUsers_UserId", "UserId", "AspNetUsers", ReferentialAction.SetNull, ReferentialAction.Cascade),
            ("ExtratoBancarioItem", "extratobancarioitem_bancoid_fkey", "BancoId", "Banco", ReferentialAction.SetNull, ReferentialAction.Restrict),
            ("ExtratoBancarioItem", "extratobancarioitem_tipocartaoid_fkey", "TipoCartaoId", "TipoCartao", ReferentialAction.SetNull, ReferentialAction.Restrict),
            ("ExtratoBancarioItem", "extratobancarioitem_tipomovimentacaoid_fkey", "TipoMovimentacaoId", "TipoMovimentacao", ReferentialAction.SetNull, ReferentialAction.Restrict),
            ("ItemLista", "FK_ItemLista_AspNetUsers_UserId", "UserId", "AspNetUsers", ReferentialAction.Restrict, ReferentialAction.Cascade),
            ("Lista", "FK_Lista_AspNetUsers_UserId", "UserId", "AspNetUsers", ReferentialAction.Restrict, ReferentialAction.Cascade),
            ("MovimentacaoDiaria", "movimentacaodiaria_bancoid_fkey", "BancoId", "Banco", ReferentialAction.Cascade, ReferentialAction.Restrict),
            ("MovimentacaoDiaria", "movimentacaodiaria_tipocartaoid_fkey", "TipoCartaoId", "TipoCartao", ReferentialAction.SetNull, ReferentialAction.Restrict),
            ("MovimentacaoDiaria", "movimentacaodiaria_tipomovimentacaoid_fkey", "TipoMovimentacaoId", "TipoMovimentacao", ReferentialAction.SetNull, ReferentialAction.Restrict),
            ("PessoaMovimentacao", "constraint_1", "UserId", "AspNetUsers", ReferentialAction.NoAction, ReferentialAction.Cascade),
            ("PessoaMovimentacao", "constraint_2", "TipoMovimentacaoId", "TipoMovimentacao", ReferentialAction.NoAction, ReferentialAction.Restrict),
            ("TipoCartao", "fk_tipocartao_aspnetusers", "UserId", "AspNetUsers", ReferentialAction.NoAction, ReferentialAction.Cascade),
            ("TipoMovimentacao", "fk_tipomovimentacao_aspnetusers", "UserId", "AspNetUsers", ReferentialAction.NoAction, ReferentialAction.Cascade),
            ("Transactions", "FK_Transactions_Accounts_IdAccount", "IdAccount", "Accounts", ReferentialAction.NoAction, ReferentialAction.Restrict),
        };

        // D-6: FKs que só existiam no modelo.
        private static readonly (string Tabela, string Nome, string Coluna)[] FksNovasParaCategories =
        {
            ("Transactions", "FK_Transactions_Categories_CategoryId", "CategoryId"),
            ("PessoaMovimentacao", "FK_PessoaMovimentacao_Categories_CategoriaId", "CategoriaId"),
        };

        // "Modelo vence": índices de FK que o banco não tinha.
        private static readonly (string Tabela, string Coluna)[] IndicesNovos =
        {
            ("Banco", "TipoCartaoId"),
            ("Banco", "UserId"),
            ("ContaVencimento", "ContaId"),
            ("ContaVencimento", "UserId"),
            ("ExtratoBancario", "BancoId"),
            ("ExtratoBancario", "UserId"),
            ("ExtratoBancarioItem", "BancoId"),
            ("ExtratoBancarioItem", "CategoriaId"),
            ("ExtratoBancarioItem", "PessoaMovimentacaoId"),
            ("ExtratoBancarioItem", "TipoCartaoId"),
            ("ExtratoBancarioItem", "TipoMovimentacaoId"),
            ("ExtratoBancarioItem", "UserId"),
            ("ItemLista", "ListaId"),
            ("ItemLista", "UserId"),
            ("Lista", "UserId"),
            ("MovimentacaoDiaria", "BancoId"),
            ("MovimentacaoDiaria", "TipoCartaoId"),
            ("MovimentacaoDiaria", "TipoMovimentacaoId"),
            ("MovimentacaoDiaria", "UserId"),
            ("PessoaMovimentacao", "CategoriaId"),
            ("PessoaMovimentacao", "TipoMovimentacaoId"),
            ("PessoaMovimentacao", "UserId"),
            ("TipoCartao", "UserId"),
            ("TipoMovimentacao", "UserId"),
            ("Transactions", "CategoryId"),
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // D-1: saldo negativo passa a ser permitido.
            migrationBuilder.DropCheckConstraint(name: "banco_saldoinicial_check", table: "Banco");

            foreach (var (tabela, coluna) in ColunasNotNull)
                migrationBuilder.Sql($"ALTER TABLE \"{tabela}\" ALTER COLUMN \"{coluna}\" SET NOT NULL;");

            foreach (var (tabela, coluna) in ColunasTimestamptz)
                migrationBuilder.Sql($"ALTER TABLE \"{tabela}\" ALTER COLUMN \"{coluna}\" TYPE timestamp with time zone USING \"{coluna}\" AT TIME ZONE 'UTC';");

            // D-4: igual às outras tabelas.
            migrationBuilder.Sql("ALTER TABLE \"PessoaMovimentacao\" ALTER COLUMN \"Id\" SET GENERATED BY DEFAULT;");

            foreach (var fk in FksAlteradas)
            {
                migrationBuilder.DropForeignKey(name: fk.Nome, table: fk.Tabela);
                migrationBuilder.AddForeignKey(
                    name: fk.Nome,
                    table: fk.Tabela,
                    column: fk.Coluna,
                    principalTable: fk.Principal,
                    principalColumn: "Id",
                    onDelete: fk.Depois);
            }

            // D-6: referências para categoria inexistente ficam sem categoria antes de a FK existir.
            // A contagem de órfãos é conferida antes de aplicar em produção (etapa 0/9 do card).
            foreach (var (tabela, nome, coluna) in FksNovasParaCategories)
            {
                migrationBuilder.Sql(
                    $"UPDATE \"{tabela}\" t SET \"{coluna}\" = NULL " +
                    $"WHERE t.\"{coluna}\" IS NOT NULL AND NOT EXISTS (SELECT 1 FROM \"Categories\" c WHERE c.\"Id\" = t.\"{coluna}\");");
                migrationBuilder.AddForeignKey(
                    name: nome,
                    table: tabela,
                    column: coluna,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            }

            foreach (var (tabela, coluna) in IndicesNovos)
                migrationBuilder.CreateIndex(name: $"IX_{tabela}_{coluna}", table: tabela, column: coluna);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var (tabela, coluna) in IndicesNovos)
                migrationBuilder.DropIndex(name: $"IX_{tabela}_{coluna}", table: tabela);

            // O UPDATE de órfãos da D-6 não tem volta (eram referências quebradas).
            foreach (var (tabela, nome, _) in FksNovasParaCategories)
                migrationBuilder.DropForeignKey(name: nome, table: tabela);

            foreach (var fk in FksAlteradas)
            {
                migrationBuilder.DropForeignKey(name: fk.Nome, table: fk.Tabela);
                migrationBuilder.AddForeignKey(
                    name: fk.Nome,
                    table: fk.Tabela,
                    column: fk.Coluna,
                    principalTable: fk.Principal,
                    principalColumn: "Id",
                    onDelete: fk.Antes);
            }

            migrationBuilder.Sql("ALTER TABLE \"PessoaMovimentacao\" ALTER COLUMN \"Id\" SET GENERATED ALWAYS;");

            foreach (var (tabela, coluna) in ColunasTimestamptz)
                migrationBuilder.Sql($"ALTER TABLE \"{tabela}\" ALTER COLUMN \"{coluna}\" TYPE timestamp without time zone USING \"{coluna}\" AT TIME ZONE 'UTC';");

            foreach (var (tabela, coluna) in ColunasNotNull)
                migrationBuilder.Sql($"ALTER TABLE \"{tabela}\" ALTER COLUMN \"{coluna}\" DROP NOT NULL;");

            // Falha se algum banco já tiver ficado com saldo negativo depois da D-1.
            migrationBuilder.AddCheckConstraint(name: "banco_saldoinicial_check", table: "Banco", sql: "\"SaldoInicial\" >= 0");
        }
    }
}
