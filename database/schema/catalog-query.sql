-- Consulta complementar ao export de schema do Neon.
-- Rodar no SQL Editor do Neon. Somente leitura: lista objetos e conta NULLs, não lê dados pessoais.
-- Resultado de 03/10/2026: prod-catalog-2026-10-03.tsv

-- 1) Tabelas fora do schema public + qualquer tabela "vinculo" em qualquer schema
SELECT 'tabela' AS tipo, table_schema || '.' || table_name AS nome, table_type::text AS detalhe
FROM information_schema.tables
WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
  AND (table_schema <> 'public' OR table_name ILIKE '%vinculo%')

-- 2) Triggers
UNION ALL
SELECT 'trigger', event_object_table || '.' || trigger_name,
       action_timing || ' ' || event_manipulation || ': ' || action_statement
FROM information_schema.triggers

-- 3) Views
UNION ALL
SELECT 'view', schemaname || '.' || viewname, definition
FROM pg_views
WHERE schemaname NOT IN ('pg_catalog', 'information_schema')

-- 4) Functions criadas à mão (exclui as que vêm de extensions)
UNION ALL
SELECT 'function', n.nspname || '.' || p.proname, pg_get_function_identity_arguments(p.oid)
FROM pg_proc p
JOIN pg_namespace n ON n.oid = p.pronamespace
WHERE n.nspname NOT IN ('pg_catalog', 'information_schema')
  AND NOT EXISTS (SELECT 1 FROM pg_depend d WHERE d.objid = p.oid AND d.deptype = 'e')

-- 5) Tipos enum
UNION ALL
SELECT 'enum', n.nspname || '.' || t.typname, string_agg(e.enumlabel, ', ' ORDER BY e.enumsortorder)
FROM pg_type t
JOIN pg_enum e ON e.enumtypid = t.oid
JOIN pg_namespace n ON n.oid = t.typnamespace
GROUP BY n.nspname, t.typname

-- 6) Extensions e versão do servidor
UNION ALL
SELECT 'extension', extname, extversion FROM pg_extension
UNION ALL
SELECT 'versao', 'server_version', current_setting('server_version')

-- 7) Migrations registradas
UNION ALL
SELECT 'migration', "MigrationId", "ProductVersion" FROM public."__EFMigrationsHistory"

-- 8) Linhas com NULL onde o modelo exige valor (o EF quebra ao ler)
UNION ALL SELECT 'nulos', 'Accounts.Value',            count(*)::text FROM "Accounts"           WHERE "Value" IS NULL
UNION ALL SELECT 'nulos', 'Accounts.EhParcelado',      count(*)::text FROM "Accounts"           WHERE "EhParcelado" IS NULL
UNION ALL SELECT 'nulos', 'Banco.TipoCartaoId',        count(*)::text FROM "Banco"              WHERE "TipoCartaoId" IS NULL
UNION ALL SELECT 'nulos', 'Banco.UserId',              count(*)::text FROM "Banco"              WHERE "UserId" IS NULL
UNION ALL SELECT 'nulos', 'Banco.SaldoInicial',        count(*)::text FROM "Banco"              WHERE "SaldoInicial" IS NULL
UNION ALL SELECT 'nulos', 'Categories.NaturezaOperacao', count(*)::text FROM "Categories"        WHERE "NaturezaOperacao" IS NULL
UNION ALL SELECT 'nulos', 'Lista.TipoMovimentacao',    count(*)::text FROM "Lista"              WHERE "TipoMovimentacao" IS NULL
UNION ALL SELECT 'nulos', 'Lista.Status',              count(*)::text FROM "Lista"              WHERE "Status" IS NULL
UNION ALL SELECT 'nulos', 'TipoMovimentacao.Descricao', count(*)::text FROM "TipoMovimentacao"  WHERE "Descricao" IS NULL
UNION ALL SELECT 'nulos', 'TipoMovimentacao.ValorMeta', count(*)::text FROM "TipoMovimentacao"  WHERE "ValorMeta" IS NULL
UNION ALL SELECT 'nulos', 'TipoMovimentacao.UserId',   count(*)::text FROM "TipoMovimentacao"   WHERE "UserId" IS NULL
UNION ALL SELECT 'nulos', 'TipoCartao.UserId',         count(*)::text FROM "TipoCartao"         WHERE "UserId" IS NULL
UNION ALL SELECT 'nulos', 'PessoaMovimentacao.NomePessoa', count(*)::text FROM "PessoaMovimentacao" WHERE "NomePessoa" IS NULL
UNION ALL SELECT 'nulos', 'PessoaMovimentacao.UserId', count(*)::text FROM "PessoaMovimentacao" WHERE "UserId" IS NULL
UNION ALL SELECT 'nulos', 'ContaVencimento.UserId',    count(*)::text FROM "ContaVencimento"    WHERE "UserId" IS NULL
UNION ALL SELECT 'nulos', 'ExtratoBancario.BancoId',   count(*)::text FROM "ExtratoBancario"    WHERE "BancoId" IS NULL
UNION ALL SELECT 'nulos', 'ExtratoBancario.LoteImportacaoId', count(*)::text FROM "ExtratoBancario" WHERE "LoteImportacaoId" IS NULL
UNION ALL SELECT 'nulos', 'ExtratoBancarioItem.UserId', count(*)::text FROM "ExtratoBancarioItem" WHERE "UserId" IS NULL
UNION ALL SELECT 'nulos', 'MovimentacaoDiaria.TipoLancamento', count(*)::text FROM "MovimentacaoDiaria" WHERE "TipoLancamento" IS NULL
ORDER BY 1, 2;
