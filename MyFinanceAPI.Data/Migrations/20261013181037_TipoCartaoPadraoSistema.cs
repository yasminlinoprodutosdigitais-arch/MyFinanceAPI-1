using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class TipoCartaoPadraoSistema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TipoCartao",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            // DB-P1-07: os 3 tipos viram padrão do sistema (UserId nulo), iguais para todos os usuários.
            // Para cada nome padrão (sem diferenciar maiúsculas/espaços): o menor Id vira o do sistema
            // (ou é inserido, se não existir); as cópias dos outros usuários têm as referências
            // remapeadas para ele e são apagadas. Idempotente. No dev: 1,2,3 sistema; 13,14,15 apagados.
            // SQL simples (sem DO $$): o script --idempotent do EF já envolve cada comando num DO $EF$.

            // 1) Insere o padrão que não existir com nenhum nome equivalente.
            migrationBuilder.Sql(@"
INSERT INTO ""TipoCartao"" (""NomeTipoCartao"", ""EhCredito"", ""UserId"")
SELECT p.nome, p.credito, NULL
FROM (VALUES ('Cartão de crédito', true), ('Conta corrente', false), ('Conta Poupança', false)) AS p(nome, credito)
WHERE NOT EXISTS (SELECT 1 FROM ""TipoCartao"" t WHERE lower(trim(t.""NomeTipoCartao"")) = lower(p.nome));");

            // 2) Remapeia as referências das cópias para o tipo que fica (menor Id de cada nome).
            const string copias = @"
WITH padrao(nome) AS (VALUES ('Cartão de crédito'), ('Conta corrente'), ('Conta Poupança')),
     sistema AS (SELECT p.nome, min(t.""Id"") AS id
                 FROM padrao p JOIN ""TipoCartao"" t ON lower(trim(t.""NomeTipoCartao"")) = lower(p.nome)
                 GROUP BY p.nome),
     copia AS (SELECT t.""Id"" AS antigo, s.id AS novo
               FROM ""TipoCartao"" t JOIN sistema s ON lower(trim(t.""NomeTipoCartao"")) = lower(s.nome)
               WHERE t.""Id"" <> s.id)";

            foreach (var tabela in new[] { "Banco", "ExtratoBancario", "ExtratoBancarioItem", "MovimentacaoDiaria" })
            {
                migrationBuilder.Sql(copias + $@"
UPDATE ""{tabela}"" x SET ""TipoCartaoId"" = c.novo FROM copia c WHERE x.""TipoCartaoId"" = c.antigo;");
            }

            // 3) Apaga as cópias (já sem referências).
            migrationBuilder.Sql(copias + @"
DELETE FROM ""TipoCartao"" t USING copia c WHERE t.""Id"" = c.antigo;");

            // 4) O que fica vira padrão do sistema, com nome canônico e EhCredito correto.
            migrationBuilder.Sql(@"
UPDATE ""TipoCartao"" t
SET ""UserId"" = NULL, ""NomeTipoCartao"" = p.nome, ""EhCredito"" = p.credito
FROM (VALUES ('Cartão de crédito', true), ('Conta corrente', false), ('Conta Poupança', false)) AS p(nome, credito)
WHERE lower(trim(t.""NomeTipoCartao"")) = lower(p.nome);");

            migrationBuilder.CreateIndex(
                name: "UX_TipoCartao_Sistema_NomeTipoCartao",
                table: "TipoCartao",
                column: "NomeTipoCartao",
                unique: true,
                filter: "\"UserId\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_TipoCartao_Sistema_NomeTipoCartao",
                table: "TipoCartao");

            // Volta parcial: os tipos do sistema passam a ser do primeiro usuário. As cópias apagadas
            // dos outros usuários não voltam (eles seguem apontando para estes tipos).
            migrationBuilder.Sql(@"UPDATE ""TipoCartao"" SET ""UserId"" = (SELECT min(""Id"") FROM ""AspNetUsers"") WHERE ""UserId"" IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TipoCartao",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
