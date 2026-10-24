using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtivoBooleano : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // contas.ativo e categorias.ativo eram integer 0/1 com CHECK; viram boolean, como bancos e listas.
            // A conversão é explícita (1 = ativo); o CHECK garantia que não havia outro valor.
            migrationBuilder.DropCheckConstraint(
                name: "contas_ativo_check",
                table: "contas");

            migrationBuilder.DropCheckConstraint(
                name: "categorias_ativo_check",
                table: "categorias");

            migrationBuilder.Sql("ALTER TABLE contas ALTER COLUMN ativo TYPE boolean USING ativo = 1;");
            migrationBuilder.Sql("ALTER TABLE categorias ALTER COLUMN ativo TYPE boolean USING ativo = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE contas ALTER COLUMN ativo TYPE integer USING CASE WHEN ativo THEN 1 ELSE 0 END;");
            migrationBuilder.Sql("ALTER TABLE categorias ALTER COLUMN ativo TYPE integer USING CASE WHEN ativo THEN 1 ELSE 0 END;");

            migrationBuilder.AddCheckConstraint(
                name: "contas_ativo_check",
                table: "contas",
                sql: "ativo IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "categorias_ativo_check",
                table: "categorias",
                sql: "ativo IN (0, 1)");
        }
    }
}
