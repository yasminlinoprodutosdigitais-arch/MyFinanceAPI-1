using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class DataTransacaoDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DB-P1-02: vencimento vira data de calendário. A conversão é EXPLÍCITA pela data UTC — o
            // cast padrão (timestamptz::date) usaria o fuso da sessão. Validado no dev em 12/10/2026
            // contra o dia de vencimento das contas: 00:00Z (API hospedada em UTC) → data UTC acerta
            // 107 de 166 (BRT, 1); 03h–18hZ (meia-noite BRT + deriva do +3h) → mesmo dia nas duas.
            migrationBuilder.Sql(
                @"ALTER TABLE ""Transactions"" ALTER COLUMN ""Date"" TYPE date USING (""Date"" AT TIME ZONE 'UTC')::date;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Volta como meia-noite em São Paulo (03:00Z), o valor que o app gravava quando estava certo.
            migrationBuilder.Sql(
                @"ALTER TABLE ""Transactions"" ALTER COLUMN ""Date"" TYPE timestamp with time zone USING (""Date""::timestamp AT TIME ZONE 'America/Sao_Paulo');");
        }
    }
}
