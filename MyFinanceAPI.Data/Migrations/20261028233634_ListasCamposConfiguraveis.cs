using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <summary>
    /// Card #67 (28/10/2026): o tipo fixo da lista (1 Checklist, 2 Cronograma, 3 Orçamento) vira campos
    /// configuráveis, "inativa" vira arquivada_em e o item fica só com feito/pendente. A conversão acontece
    /// com as colunas antigas ainda no lugar, e a contagem, a soma e os feitos de cada lista são conferidos
    /// antes de apagá-las. A mesma conversão está em database/schema/aplicar-producao-listas-campos.sql.
    /// </summary>
    public partial class ListasCamposConfiguraveis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "arquivada_em",
                table: "listas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "icone",
                table: "listas",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "limite",
                table: "listas",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modelo",
                table: "listas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "BRANCO");

            migrationBuilder.AddColumn<string>(
                name: "modo_valor",
                table: "listas",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "TOTAL");

            migrationBuilder.AddColumn<bool>(
                name: "usa_data",
                table: "listas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "usa_limite",
                table: "listas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "usa_observacao",
                table: "listas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "usa_quantidade",
                table: "listas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "usa_valor",
                table: "listas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "feito",
                table: "itens_lista",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "feito_em",
                table: "itens_lista",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ordem",
                table: "itens_lista",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(ConverterDados);

            migrationBuilder.DropCheckConstraint(
                name: "listas_tipo_lista_check",
                table: "listas");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_quantidade_check",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_situacao_check",
                table: "itens_lista");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "tipo_lista",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "situacao",
                table: "itens_lista");

            migrationBuilder.RenameColumn(
                name: "data_tarefa",
                table: "itens_lista",
                newName: "data");

            migrationBuilder.AddCheckConstraint(
                name: "listas_limite_check",
                table: "listas",
                sql: "limite >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "listas_limite_com_valor_check",
                table: "listas",
                sql: "NOT usa_limite OR usa_valor");

            migrationBuilder.AddCheckConstraint(
                name: "listas_modelo_check",
                table: "listas",
                sql: "modelo IN ('COMPRAS', 'ORCAMENTO', 'TAREFAS', 'AGENDA', 'BRANCO')");

            migrationBuilder.AddCheckConstraint(
                name: "listas_modo_valor_check",
                table: "listas",
                sql: "modo_valor IN ('UNITARIO', 'TOTAL')");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_quantidade_check",
                table: "itens_lista",
                sql: "quantidade >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "itens_lista_valor_check",
                table: "itens_lista",
                sql: "valor >= 0");
        }

        // Orçamento: valor total + observação (a quantidade, sempre 1, fica gravada e desligada).
        // Checklist: quantidade. Cronograma: data. AGUARDANDO não existe mais: vira pendente e o
        // "(aguardando)" vai para a observação, para não perder a informação (decisão dela, 28/10).
        // A conferência compara, por lista, itens, soma do valor e feitos (OK antes, feito depois).
        internal const string ConverterDados = @"
CREATE TEMP TABLE conferencia_listas ON COMMIT DROP AS
SELECT lista_id, count(*) AS itens, coalesce(sum(valor), 0) AS soma,
       count(*) FILTER (WHERE situacao = 'OK') AS feitos,
       count(*) FILTER (WHERE situacao = 'AGUARDANDO') AS aguardando
FROM itens_lista GROUP BY lista_id;

UPDATE listas SET
    usa_valor      = tipo_lista = 3,
    usa_observacao = tipo_lista = 3,
    usa_quantidade = tipo_lista = 1,
    usa_data       = tipo_lista = 2,
    modo_valor     = 'TOTAL',
    modelo         = CASE tipo_lista WHEN 1 THEN 'COMPRAS' WHEN 2 THEN 'AGENDA' ELSE 'ORCAMENTO' END,
    icone          = CASE tipo_lista WHEN 1 THEN 'pi pi-shopping-cart' WHEN 2 THEN 'pi pi-calendar' ELSE 'pi pi-wallet' END,
    arquivada_em   = CASE WHEN ativo THEN NULL ELSE now() END;

UPDATE itens_lista SET
    feito      = situacao = 'OK',
    feito_em   = CASE WHEN situacao = 'OK' THEN data_alteracao END,
    observacao = CASE
        WHEN situacao <> 'AGUARDANDO' OR situacao IS NULL THEN observacao
        WHEN observacao IS NULL OR btrim(observacao) = '' THEN 'aguardando'
        ELSE left(btrim(observacao), 487) || ' (aguardando)'
    END;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM conferencia_listas c
        FULL JOIN (
            SELECT lista_id, count(*) AS itens, coalesce(sum(valor), 0) AS soma,
                   count(*) FILTER (WHERE feito) AS feitos,
                   count(*) FILTER (WHERE situacao = 'AGUARDANDO' AND observacao LIKE '%aguardando%') AS aguardando
            FROM itens_lista GROUP BY lista_id
        ) d USING (lista_id)
        WHERE c.itens IS DISTINCT FROM d.itens
           OR c.soma IS DISTINCT FROM d.soma
           OR c.feitos IS DISTINCT FROM d.feitos
           OR c.aguardando IS DISTINCT FROM d.aguardando
    ) THEN
        RAISE EXCEPTION 'Conferência das listas falhou: itens, soma ou feitos mudaram na conversão.';
    END IF;
END $$;";

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "listas_limite_check",
                table: "listas");

            migrationBuilder.DropCheckConstraint(
                name: "listas_limite_com_valor_check",
                table: "listas");

            migrationBuilder.DropCheckConstraint(
                name: "listas_modelo_check",
                table: "listas");

            migrationBuilder.DropCheckConstraint(
                name: "listas_modo_valor_check",
                table: "listas");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_quantidade_check",
                table: "itens_lista");

            migrationBuilder.DropCheckConstraint(
                name: "itens_lista_valor_check",
                table: "itens_lista");

            migrationBuilder.RenameColumn(
                name: "data",
                table: "itens_lista",
                newName: "data_tarefa");

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "listas",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "tipo_lista",
                table: "listas",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "situacao",
                table: "itens_lista",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            // Volta pelo caminho inverso: o modelo diz o tipo, e o "(aguardando)" da observação volta a ser situação.
            migrationBuilder.Sql(@"
UPDATE listas SET
    tipo_lista = CASE modelo WHEN 'ORCAMENTO' THEN 3 WHEN 'AGENDA' THEN 2 ELSE 1 END,
    ativo      = arquivada_em IS NULL;

UPDATE itens_lista SET
    situacao   = CASE
        WHEN feito THEN 'OK'
        WHEN observacao = 'aguardando' OR observacao LIKE '% (aguardando)' THEN 'AGUARDANDO'
        ELSE 'PENDENTE'
    END,
    observacao = CASE
        WHEN feito THEN observacao
        WHEN observacao = 'aguardando' THEN NULL
        WHEN observacao LIKE '% (aguardando)' THEN left(observacao, length(observacao) - 13)
        ELSE observacao
    END;");

            migrationBuilder.DropColumn(
                name: "arquivada_em",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "icone",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "limite",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "modelo",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "modo_valor",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "usa_data",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "usa_limite",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "usa_observacao",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "usa_quantidade",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "usa_valor",
                table: "listas");

            migrationBuilder.DropColumn(
                name: "feito",
                table: "itens_lista");

            migrationBuilder.DropColumn(
                name: "feito_em",
                table: "itens_lista");

            migrationBuilder.DropColumn(
                name: "ordem",
                table: "itens_lista");

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
        }
    }
}
