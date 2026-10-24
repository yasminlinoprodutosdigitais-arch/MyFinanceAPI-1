using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApagaDataOperacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // contas.data_operacao é a coluna antiga dos dias de vencimento; desde o [DB] P1 eles vivem em
            // contas_vencimento. Contas sem nenhum vencimento ainda tinham o dia só aqui (11 no dev em
            // 24/10/2026): os dias de 1 a 31 vão para contas_vencimento antes de apagar (decisão dela).
            // O que não for dia válido (como 0) fica de fora; contas que já têm vencimento não mudam.
            migrationBuilder.Sql(@"
INSERT INTO contas_vencimento (conta_id, dia, usuario_id)
SELECT DISTINCT c.id, trim(d.valor)::integer, c.usuario_id
FROM contas c
CROSS JOIN LATERAL unnest(string_to_array(c.data_operacao, ',')) AS d(valor)
WHERE trim(d.valor) ~ '^[0-9]{1,2}$'
  AND trim(d.valor)::integer BETWEEN 1 AND 31
  AND NOT EXISTS (SELECT 1 FROM contas_vencimento v WHERE v.conta_id = c.id);");

            migrationBuilder.DropColumn(
                name: "data_operacao",
                table: "contas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // A coluna volta vazia: os valores antigos não se recuperam, e os vencimentos copiados ficam.
            migrationBuilder.AddColumn<string>(
                name: "data_operacao",
                table: "contas",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }
    }
}
