using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinanceAPI.Data.Migrations
{
    /// <summary>
    /// Card #45, parte 2: os lançamentos antigos de encargo (IOF, juros, anuidade…) passam para o "Eu" e a categoria
    /// "Tarifas e encargos", como a importação faz desde então. A expressão é a mesma do ExtratoBancarioService.EhEncargo
    /// (\m e \M = início e fim de palavra no Postgres). As "pessoas" que eram só encargo ficam sem uso e são apagadas.
    /// Só dados: o Down não desfaz (não há como saber a pessoa e a categoria de antes).
    /// </summary>
    public partial class EncargosDadosAntigos : Migration
    {
        // Encargo pela descrição ou pelo nome no campo Pessoa: há lançamentos sem descrição, só com "DEBITO IOF" na pessoa.
        private const string ItemDeEncargo = "(i.descricao ~* " + Encargo + " OR i.nome_pessoa_transacao ~* " + Encargo
            + " OR EXISTS (SELECT 1 FROM pessoas_movimentacao pe WHERE pe.id = i.pessoa_movimentacao_id AND pe.nome_pessoa ~* " + Encargo + "))";

        private const string Encargo = @"'\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M'";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. A pessoa "EU" que já existir vira a titular.
            migrationBuilder.Sql(@"
UPDATE pessoas_movimentacao p SET eh_titular = true
WHERE p.nome_pessoa = 'EU'
  AND NOT EXISTS (SELECT 1 FROM pessoas_movimentacao t WHERE t.usuario_id = p.usuario_id AND t.eh_titular);");

            // 2. Quem tem lançamento de encargo e ainda não tem o "Eu" ganha um.
            migrationBuilder.Sql($@"
INSERT INTO pessoas_movimentacao (nome_pessoa, eh_titular, entra_nos_acertos, usuario_id)
SELECT DISTINCT 'EU', true, false, i.usuario_id
FROM itens_extrato_bancario i
WHERE {ItemDeEncargo}
  AND NOT EXISTS (SELECT 1 FROM pessoas_movimentacao t WHERE t.usuario_id = i.usuario_id AND t.eh_titular);");

            // 3. E a categoria "Tarifas e encargos" (Despesa, Essencial), se ainda não tiver.
            migrationBuilder.Sql($@"
INSERT INTO categorias (nome, subcategoria, natureza_operacao, ativo, usuario_id)
SELECT DISTINCT 'Tarifas e encargos', 'Despesa', 1, true, i.usuario_id
FROM itens_extrato_bancario i
WHERE {ItemDeEncargo}
  AND NOT EXISTS (SELECT 1 FROM categorias c WHERE c.usuario_id = i.usuario_id
                  AND upper(trim(c.nome)) = 'TARIFAS E ENCARGOS' AND upper(trim(c.subcategoria)) = 'DESPESA');");

            // 4. Os lançamentos de encargo passam para o "Eu" e a categoria. Sem descrição, o nome que estava na pessoa
            //    ("DEBITO IOF") vira a descrição, para o lançamento não ficar sem rótulo.
            migrationBuilder.Sql($@"
UPDATE itens_extrato_bancario i
SET descricao = coalesce(nullif(trim(i.descricao), ''), i.nome_pessoa_transacao),
    pessoa_movimentacao_id = eu.id,
    nome_pessoa_transacao = eu.nome_pessoa,
    categoria_id = (SELECT min(c.id) FROM categorias c WHERE c.usuario_id = i.usuario_id
                    AND upper(trim(c.nome)) = 'TARIFAS E ENCARGOS' AND upper(trim(c.subcategoria)) = 'DESPESA')
FROM pessoas_movimentacao eu
WHERE eu.usuario_id = i.usuario_id AND eu.eh_titular
  AND {ItemDeEncargo};");

            // 5. As "pessoas" que eram só encargo (ex.: "DEBITO IOF") e ficaram sem uso.
            migrationBuilder.Sql($@"
DELETE FROM pessoas_movimentacao p
WHERE NOT p.eh_titular
  AND p.nome_pessoa ~* {Encargo}
  AND NOT EXISTS (SELECT 1 FROM itens_extrato_bancario i WHERE i.pessoa_movimentacao_id = p.id);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
