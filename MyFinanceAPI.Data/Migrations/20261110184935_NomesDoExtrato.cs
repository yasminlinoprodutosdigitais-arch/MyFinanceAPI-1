using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <summary>
    /// Card #79 — "Nomes do extrato". O nome que vem no extrato ("CIRCUITO") deixa de ser pessoa: cada pessoa criada pela
    /// importação, com categoria ou tipo, vira a regra "nome do extrato é igual a NOME"; todas as pessoas (menos o "Eu")
    /// saem, e as pessoas de verdade passam a ser cadastradas à mão. Junto: some "entra nos acertos" (toda pessoa entra),
    /// "Tansferência entre contas" é juntada na "Transferência entre contas" e "&amp;AMP;" vira "&amp;".
    /// Não volta os dados no Down (as pessoas apagadas não voltam).
    /// </summary>
    public partial class NomesDoExtrato : Migration
    {
        /// <summary>
        /// Mesmo texto da API (NomeExtrato.Chave sobre o nome da pessoa): sem "&amp;amp;", corte no "*" depois de mais de
        /// 3 letras, sem números, sem acento, maiúsculas e espaços simples.
        /// </summary>
        private const string ChaveSql = @"
            btrim(regexp_replace(
                translate(upper(
                    regexp_replace(
                        CASE WHEN position('*' IN n.nome) > 0 AND length(btrim(left(n.nome, position('*' IN n.nome) - 1))) > 3
                             THEN left(n.nome, position('*' IN n.nome) - 1) ELSE n.nome END,
                        '[0-9]', '', 'g')),
                    'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇÑ', 'AAAAAEEEEIIIIOOOOOUUUUCN'),
                '\s+', ' ', 'g'))";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "classificado_por_semelhanca",
                table: "itens_extrato_bancario",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // 1) "&AMP;" que o banco mandou vira "&".
            migrationBuilder.Sql(@"
                UPDATE itens_extrato_bancario
                SET descricao = regexp_replace(descricao, '&amp;', '&', 'gi'),
                    nome_pessoa_transacao = regexp_replace(nome_pessoa_transacao, '&amp;', '&', 'gi')
                WHERE descricao ~* '&amp;' OR nome_pessoa_transacao ~* '&amp;';");

            // 2) Pessoa com categoria ou tipo (menos o "Eu") vira regra "nome do extrato é igual a", no fim da lista.
            migrationBuilder.Sql($@"
                WITH n AS (
                    SELECT p.id, p.usuario_id, p.categoria_id, p.tipo_movimentacao_id,
                           regexp_replace(btrim(p.nome_pessoa), '&amp;', '&', 'gi') AS nome
                    FROM pessoas_movimentacao p
                    WHERE NOT p.eh_titular AND (p.categoria_id IS NOT NULL OR p.tipo_movimentacao_id IS NOT NULL)
                ), c AS (
                    SELECT DISTINCT ON (n.usuario_id, {ChaveSql})
                           n.usuario_id, {ChaveSql} AS texto, n.categoria_id, n.tipo_movimentacao_id, n.id
                    FROM n
                    ORDER BY n.usuario_id, {ChaveSql}, n.id
                )
                INSERT INTO regras_classificacao (usuario_id, texto, campo, operador, categoria_id, tipo_movimentacao_id, ativa, ordem, vezes_aplicada)
                SELECT c.usuario_id, c.texto, 'nome_extrato', 'igual', c.categoria_id, c.tipo_movimentacao_id, true,
                       coalesce((SELECT max(r.ordem) FROM regras_classificacao r WHERE r.usuario_id = c.usuario_id), -1)
                           + row_number() OVER (PARTITION BY c.usuario_id ORDER BY c.texto),
                       0
                FROM c
                WHERE length(c.texto) >= 2
                  AND NOT EXISTS (SELECT 1 FROM regras_classificacao r
                                  WHERE r.usuario_id = c.usuario_id AND r.campo = 'nome_extrato' AND r.operador = 'igual'
                                    AND r.texto = c.texto AND r.banco_id IS NULL);");

            // 3) Lançamentos e partes deixam a pessoa (o nome do extrato e a categoria ficam); as pessoas saem.
            //    Regras que ligavam a pessoa ficam sem ela (SET NULL); bancos_pessoas some em cascata.
            migrationBuilder.Sql(@"
                UPDATE itens_extrato_bancario i SET pessoa_movimentacao_id = NULL
                FROM pessoas_movimentacao p WHERE p.id = i.pessoa_movimentacao_id AND NOT p.eh_titular;
                UPDATE extrato_item_partes x SET pessoa_movimentacao_id = NULL
                FROM pessoas_movimentacao p WHERE p.id = x.pessoa_movimentacao_id AND NOT p.eh_titular;
                DELETE FROM pessoas_movimentacao WHERE NOT eh_titular;");

            // 4) "Tansferência entre contas" (erro de digitação): junta na certa quando ela existe; senão, só corrige o nome.
            migrationBuilder.Sql(@"
                CREATE TEMP TABLE categorias_juntar ON COMMIT DROP AS
                SELECT e.id AS errada, c.id AS certa
                FROM categorias e
                JOIN categorias c ON c.usuario_id = e.usuario_id AND c.id <> e.id
                                 AND c.subcategoria IS NOT DISTINCT FROM e.subcategoria
                                 AND lower(translate(c.nome, 'ê', 'e')) = 'transferencia entre contas'
                WHERE lower(translate(e.nome, 'ê', 'e')) = 'tansferencia entre contas';

                UPDATE itens_extrato_bancario t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                UPDATE extrato_item_partes t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                UPDATE regras_classificacao t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                UPDATE notas_fiscais_itens t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                UPDATE contas t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                UPDATE transacoes t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                UPDATE pessoas_movimentacao t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                DELETE FROM categorias WHERE id IN (SELECT errada FROM categorias_juntar);

                UPDATE categorias SET nome = 'Transferência entre contas'
                WHERE lower(translate(nome, 'ê', 'e')) = 'tansferencia entre contas';");

            migrationBuilder.DropColumn(
                name: "entra_nos_acertos",
                table: "pessoas_movimentacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "classificado_por_semelhanca",
                table: "itens_extrato_bancario");

            migrationBuilder.AddColumn<bool>(
                name: "entra_nos_acertos",
                table: "pessoas_movimentacao",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
