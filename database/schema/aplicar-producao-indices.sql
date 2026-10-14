-- =====================================================================================
-- [DB] P2 — Índices de consulta — aplicação em PRODUÇÃO (branch "development")
-- Card: [DB] P2 — Índices e performance (7) · checklist "Plano", item 4
-- Testado no branch "dev" em 13/10/2026 (migration 20261014004442_IndicesConsulta).
--
-- PRÉ-REQUISITOS, nesta ordem: aplicar-producao-2026-10-05.sql, aplicar-producao-integridade.sql,
-- aplicar-producao-dominios.sql, aplicar-producao-datas.sql, aplicar-producao-tipocartao.sql,
-- aplicar-producao-auditoria.sql.
-- Só troca índices: a API antiga continua funcionando. O ganho completo vem com a API da branch
-- db-p2-indices, que filtra o mês por intervalo (Date >= início e < início do mês seguinte).
--
-- Cria (UserId, Date) em Transactions e (UserId, DataMovimentacao)/(UserId, NumeroFatura) em
-- ExtratoBancarioItem; tira os índices simples que eles tornam redundantes. DROP com IF EXISTS
-- para não parar se a produção não tiver algum deles.
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261014004442_IndicesConsulta') THEN
    DROP INDEX IF EXISTS "IX_Transactions_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261014004442_IndicesConsulta') THEN
    DROP INDEX IF EXISTS idx_extratobancarioitem_datamov;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261014004442_IndicesConsulta') THEN
    DROP INDEX IF EXISTS "IX_ExtratoBancarioItem_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261014004442_IndicesConsulta') THEN
    CREATE INDEX "IX_Transactions_UserId_Date" ON "Transactions" ("UserId", "Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261014004442_IndicesConsulta') THEN
    CREATE INDEX "IX_ExtratoBancarioItem_UserId_DataMovimentacao" ON "ExtratoBancarioItem" ("UserId", "DataMovimentacao");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261014004442_IndicesConsulta') THEN
    CREATE INDEX "IX_ExtratoBancarioItem_UserId_NumeroFatura" ON "ExtratoBancarioItem" ("UserId", "NumeroFatura");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261014004442_IndicesConsulta') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261014004442_IndicesConsulta', '8.0.8');
    END IF;
END $EF$;
COMMIT;

