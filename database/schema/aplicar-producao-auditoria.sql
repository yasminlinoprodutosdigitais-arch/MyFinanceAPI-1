-- =====================================================================================
-- [DB] P1 subprojeto 4 — Auditoria (DataCriacao) — aplicação em PRODUÇÃO (branch "development")
-- Card: [DB] P1 — Modelagem (13) · checklist "Plano — Subprojeto 4 (Auditoria)"
-- Testado no branch "dev" em 13/10/2026 (migration 20261013182638_DataCriacaoTransacional).
--
-- PRÉ-REQUISITOS, nesta ordem: aplicar-producao-2026-10-05.sql, aplicar-producao-integridade.sql,
-- aplicar-producao-dominios.sql, aplicar-producao-datas.sql, aplicar-producao-tipocartao.sql.
-- Só acrescenta colunas: a API antiga continua funcionando. A atualização automática de
-- DataAlteracao/DataCriacao vem com a API da branch db-p1-modelagem (DataAlteracaoInterceptor).
--
-- Para as linhas existentes, DataCriacao recebe o DataAlteracao atual (a data do INSERT, ou a do
-- rebaseline para linhas mais antigas que ele). Não há dado de criação mais preciso em lugar nenhum.
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013182638_DataCriacaoTransacional') THEN
    ALTER TABLE "Transactions" ADD "DataCriacao" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013182638_DataCriacaoTransacional') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD "DataCriacao" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013182638_DataCriacaoTransacional') THEN
    UPDATE "Transactions" SET "DataCriacao" = "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013182638_DataCriacaoTransacional') THEN
    UPDATE "ExtratoBancarioItem" SET "DataCriacao" = "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013182638_DataCriacaoTransacional') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261013182638_DataCriacaoTransacional', '8.0.8');
    END IF;
END $EF$;
COMMIT;

