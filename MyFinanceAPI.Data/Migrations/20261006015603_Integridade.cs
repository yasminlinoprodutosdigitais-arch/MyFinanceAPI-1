using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class Integridade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Categories_CategoryId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "constraint_1",
                table: "PessoaMovimentacao");

            migrationBuilder.DropForeignKey(
                name: "constraint_2",
                table: "PessoaMovimentacao");

            migrationBuilder.DropIndex(
                name: "IX_TipoCartao_UserId",
                table: "TipoCartao");

            migrationBuilder.DropIndex(
                name: "IX_PessoaMovimentacao_UserId",
                table: "PessoaMovimentacao");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Banco_UserId",
                table: "Banco");

            migrationBuilder.AddColumn<string>(
                name: "ChaveImportacao",
                table: "ExtratoBancarioItem",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            // ---- Dados (escrito à mão; precisa vir antes das uniques) --------------------------------
            // Limpeza de pessoas decidida no card [DB] P0, subprojeto 3. Não tem Down: a volta é o
            // branch de restauração do Neon.

            // a) "Pessoa sem nome" era um placeholder do cadastro manual de item: os itens ficam sem pessoa.
            migrationBuilder.Sql(@"
UPDATE ""ExtratoBancarioItem"" SET ""PessoaMovimentacaoId"" = NULL
 WHERE ""PessoaMovimentacaoId"" IN (SELECT ""Id"" FROM ""PessoaMovimentacao"" WHERE upper(trim(""NomePessoa"")) = 'PESSOA SEM NOME');
DELETE FROM ""PessoaMovimentacao"" WHERE upper(trim(""NomePessoa"")) = 'PESSOA SEM NOME';");

            // b) Fusão por (UserId, nome normalizado): fica o menor Id; categoria/tipo vêm do registro
            //    mais recente que tenha algum dos dois; os itens passam a apontar para quem fica.
            migrationBuilder.Sql(@"
UPDATE ""PessoaMovimentacao"" k
   SET ""CategoriaId"" = c.""CategoriaId"", ""TipoMovimentacaoId"" = c.""TipoMovimentacaoId""
  FROM (SELECT DISTINCT ON (""UserId"", upper(trim(""NomePessoa"")))
               ""UserId"", upper(trim(""NomePessoa"")) AS n, ""CategoriaId"", ""TipoMovimentacaoId""
          FROM ""PessoaMovimentacao""
         WHERE ""CategoriaId"" IS NOT NULL OR ""TipoMovimentacaoId"" IS NOT NULL
         ORDER BY ""UserId"", upper(trim(""NomePessoa"")), ""Id"" DESC) c,
       (SELECT ""UserId"", upper(trim(""NomePessoa"")) AS n, min(""Id"") AS keep
          FROM ""PessoaMovimentacao"" GROUP BY 1, 2 HAVING count(*) > 1) g
 WHERE k.""Id"" = g.keep AND c.""UserId"" = g.""UserId"" AND c.n = g.n;");

            migrationBuilder.Sql(@"
UPDATE ""ExtratoBancarioItem"" i SET ""PessoaMovimentacaoId"" = m.keep
  FROM (SELECT ""Id"", min(""Id"") OVER (PARTITION BY ""UserId"", upper(trim(""NomePessoa""))) AS keep
          FROM ""PessoaMovimentacao"") m
 WHERE i.""PessoaMovimentacaoId"" = m.""Id"" AND m.""Id"" <> m.keep;");

            migrationBuilder.Sql(@"
DELETE FROM ""PessoaMovimentacao"" p
 USING (SELECT ""Id"", min(""Id"") OVER (PARTITION BY ""UserId"", upper(trim(""NomePessoa""))) AS keep
          FROM ""PessoaMovimentacao"") m
 WHERE p.""Id"" = m.""Id"" AND m.""Id"" <> m.keep;");

            migrationBuilder.Sql(@"
UPDATE ""PessoaMovimentacao"" SET ""NomePessoa"" = upper(trim(""NomePessoa""))
 WHERE ""NomePessoa"" <> upper(trim(""NomePessoa""));");

            // c) Chave de idempotência dos itens já importados. Mesma fórmula de
            //    ExtratoBancarioService.GerarChaveImportacao: id | data | valor com sinal | descrição.
            migrationBuilder.Sql(@"
UPDATE ""ExtratoBancarioItem"" SET ""ChaveImportacao"" = encode(sha256(convert_to(
         trim(""Identificador"") || '|' ||
         to_char(""DataMovimentacao"", 'YYYY-MM-DD') || '|' ||
         to_char(CASE WHEN ""TipoLancamento"" = 'Saída' THEN -""Valor"" ELSE ""Valor"" END, 'FM9999999990.00') || '|' ||
         coalesce(trim(""Descricao""), ''), 'UTF8')), 'hex')
 WHERE ""Identificador"" IS NOT NULL AND ""ExtratoBancarioId"" IS NOT NULL;");
            // ------------------------------------------------------------------------------------------

            migrationBuilder.CreateIndex(
                name: "UX_TipoCartao_UserId_NomeTipoCartao",
                table: "TipoCartao",
                columns: new[] { "UserId", "NomeTipoCartao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_PessoaMovimentacao_UserId_NomePessoa",
                table: "PessoaMovimentacao",
                columns: new[] { "UserId", "NomePessoa" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ExtratoBancarioItem_ChaveImportacao",
                table: "ExtratoBancarioItem",
                columns: new[] { "UserId", "BancoId", "ChaveImportacao" },
                unique: true,
                filter: "\"ChaveImportacao\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExtratoBancario_TipoCartaoId",
                table: "ExtratoBancario",
                column: "TipoCartaoId");

            migrationBuilder.CreateIndex(
                name: "UX_Categories_UserId_Name_SubCategory",
                table: "Categories",
                columns: new[] { "UserId", "Name", "SubCategory" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Banco_UserId_NomeBanco",
                table: "Banco",
                columns: new[] { "UserId", "NomeBanco" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Categories_CategoryId",
                table: "Accounts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtratoBancario_TipoCartao_TipoCartaoId",
                table: "ExtratoBancario",
                column: "TipoCartaoId",
                principalTable: "TipoCartao",
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
                name: "FK_PessoaMovimentacao_TipoMovimentacao_TipoMovimentacaoId",
                table: "PessoaMovimentacao",
                column: "TipoMovimentacaoId",
                principalTable: "TipoMovimentacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <summary>
        /// Desfaz só o schema (uniques, FK nova, coluna, nomes, CASCADE de Accounts→Categories).
        /// A limpeza de pessoas e a chave dos itens não voltam — para isso, o branch de restauração.
        /// </summary>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Categories_CategoryId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ExtratoBancario_TipoCartao_TipoCartaoId",
                table: "ExtratoBancario");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaMovimentacao_AspNetUsers_UserId",
                table: "PessoaMovimentacao");

            migrationBuilder.DropForeignKey(
                name: "FK_PessoaMovimentacao_TipoMovimentacao_TipoMovimentacaoId",
                table: "PessoaMovimentacao");

            migrationBuilder.DropIndex(
                name: "UX_TipoCartao_UserId_NomeTipoCartao",
                table: "TipoCartao");

            migrationBuilder.DropIndex(
                name: "UX_PessoaMovimentacao_UserId_NomePessoa",
                table: "PessoaMovimentacao");

            migrationBuilder.DropIndex(
                name: "UX_ExtratoBancarioItem_ChaveImportacao",
                table: "ExtratoBancarioItem");

            migrationBuilder.DropIndex(
                name: "IX_ExtratoBancario_TipoCartaoId",
                table: "ExtratoBancario");

            migrationBuilder.DropIndex(
                name: "UX_Categories_UserId_Name_SubCategory",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "UX_Banco_UserId_NomeBanco",
                table: "Banco");

            migrationBuilder.DropColumn(
                name: "ChaveImportacao",
                table: "ExtratoBancarioItem");

            migrationBuilder.CreateIndex(
                name: "IX_TipoCartao_UserId",
                table: "TipoCartao",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PessoaMovimentacao_UserId",
                table: "PessoaMovimentacao",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Banco_UserId",
                table: "Banco",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Categories_CategoryId",
                table: "Accounts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "constraint_1",
                table: "PessoaMovimentacao",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "constraint_2",
                table: "PessoaMovimentacao",
                column: "TipoMovimentacaoId",
                principalTable: "TipoMovimentacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
