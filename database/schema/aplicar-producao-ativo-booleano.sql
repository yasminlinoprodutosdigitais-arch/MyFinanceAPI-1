-- =====================================================================================
-- [DB] contas.ativo e categorias.ativo: de int 0/1 para boolean — PRODUÇÃO (branch "development")
-- Card: [DB] contas.ativo e categorias.ativo: de int 0/1 para boolean (#24) · checklist "Plano"
-- Testado no branch "dev" em 24/10/2026 (migration 20261024171824_AtivoBooleano): contas 60 ativas /
-- 22 inativas e categorias 33 / 1, iguais antes e depois.
--
-- PRÉ-REQUISITO: aplicar-producao-convencoes.sql ([DB] P3) já aplicado (tabelas contas/categorias,
-- coluna ativo e os CHECKs contas_ativo_check / categorias_ativo_check com esses nomes).
-- Publicar a API e o front desta mudança JUNTO: a API nova recusa "ativo": 1 e o front antigo não
-- entende "ativo": true.
-- ANTES DE TUDO: branch de restauração no Neon.
--
-- PASSO 0 — CONFERÊNCIA (só leitura). Só pode haver 0 e 1, e os dois CHECKs precisam existir.
-- SELECT 'contas' AS tabela, ativo, count(*) FROM contas GROUP BY ativo
-- UNION ALL SELECT 'categorias', ativo, count(*) FROM categorias GROUP BY ativo ORDER BY 1, 2;
-- SELECT conname FROM pg_constraint WHERE conname IN ('contas_ativo_check', 'categorias_ativo_check');
-- Depois do Passo 1, a mesma contagem deve dar true no lugar de 1 e false no lugar de 0.
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
-- =====================================================================================
START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024171824_AtivoBooleano') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_ativo_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024171824_AtivoBooleano') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_ativo_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024171824_AtivoBooleano') THEN
    ALTER TABLE contas ALTER COLUMN ativo TYPE boolean USING ativo = 1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024171824_AtivoBooleano') THEN
    ALTER TABLE categorias ALTER COLUMN ativo TYPE boolean USING ativo = 1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024171824_AtivoBooleano') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261024171824_AtivoBooleano', '8.0.8');
    END IF;
END $EF$;
COMMIT;

