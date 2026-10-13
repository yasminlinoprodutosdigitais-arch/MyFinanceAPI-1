-- =====================================================================================
-- [DB] P1 subprojeto 1 — Domínios e CHECKs — aplicação em PRODUÇÃO (branch "development" do Neon)
-- Card: [DB] P1 — Modelagem (13) · checklist "Plano — Subprojeto 1 (Domínios e CHECKs)"
-- Testado no branch "dev" em 12/10/2026 (migration 20261013014852_DominiosECheck).
--
-- PRÉ-REQUISITOS, nesta ordem: aplicar-producao-2026-10-05.sql e aplicar-producao-integridade.sql.
-- Publicar a API da branch db-p1-modelagem JUNTO: a coluna Lista."TipoMovimentacao" vira
-- "TipoLista", e a API antiga para de ler as listas.
-- ANTES DE TUDO: branch de restauração no Neon a partir de "development". Os UPDATEs de
-- normalização (CONCLUIDO -> OK, Status 2 -> 0, grafias) não têm volta.
--
-- PASSO 0 — CONFERÊNCIA (só leitura). Rodar e conferir que TODAS as contagens dão 0.
-- Os valores já são calculados como ficam DEPOIS da normalização. Se alguma der > 0,
-- PARAR: há valor que a migration não normaliza e o CHECK vai falhar.
--
-- SELECT 'accounts_status', count(*) FROM "Accounts" WHERE NOT (CASE WHEN "Status" = 2 THEN 0 ELSE "Status" END IN (0, 1))
-- UNION ALL SELECT 'accounts_parcelas', count(*) FROM "Accounts" WHERE NOT (NOT "EhParcelado" OR ("QuantidadeParcelas" >= 1 AND "ParcelaAtual" BETWEEN 0 AND "QuantidadeParcelas"))
-- UNION ALL SELECT 'categories_status', count(*) FROM "Categories" WHERE NOT ("Status" IN (0, 1))
-- UNION ALL SELECT 'categories_natureza', count(*) FROM "Categories" WHERE NOT ("NaturezaOperacao" BETWEEN 0 AND 5)
-- UNION ALL SELECT 'contavencimento_dia', count(*) FROM "ContaVencimento" WHERE NOT ("Dia" BETWEEN 1 AND 31)
-- UNION ALL SELECT 'transactions_status', count(*) FROM "Transactions" WHERE NOT (UPPER(TRIM("Status")) IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO'))
-- UNION ALL SELECT 'transactions_parcelas', count(*) FROM "Transactions" WHERE NOT ("QuantidadeParcelas" >= 1 AND "ParcelaAtual" BETWEEN 1 AND "QuantidadeParcelas")
-- UNION ALL SELECT 'itemlista_status', count(*) FROM "ItemLista" WHERE NOT (CASE WHEN UPPER(TRIM("Status")) = 'CONCLUIDO' THEN 'OK' ELSE UPPER(TRIM("Status")) END IN ('PENDENTE', 'AGUARDANDO', 'OK'))
-- UNION ALL SELECT 'itemlista_quantidade', count(*) FROM "ItemLista" WHERE NOT ("Quantidade" >= 0)
-- UNION ALL SELECT 'extrato_tipolancamento', count(*) FROM "ExtratoBancarioItem" WHERE NOT (CASE WHEN UPPER(TRIM("TipoLancamento")) IN ('SAIDA', 'SAÍDA') THEN 'Saída' WHEN UPPER(TRIM("TipoLancamento")) = 'ENTRADA' THEN 'Entrada' ELSE "TipoLancamento" END IN ('Entrada', 'Saída'))
-- UNION ALL SELECT 'extrato_parcelas', count(*) FROM "ExtratoBancarioItem" WHERE NOT ("QuantidadeParcelas" >= 1 AND "ParcelaAtual" BETWEEN 1 AND "QuantidadeParcelas")
-- UNION ALL SELECT 'lista_tipo', count(*) FROM "Lista" WHERE NOT ("TipoMovimentacao" IN (1, 2, 3));
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent). Roda numa transação só.
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "Lista" RENAME COLUMN "TipoMovimentacao" TO "TipoLista";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    UPDATE "Accounts" SET "Status" = 0 WHERE "Status" = 2;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    UPDATE "Transactions" SET "Status" = UPPER(TRIM("Status")) WHERE "Status" <> UPPER(TRIM("Status"));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    UPDATE "ItemLista" SET "Status" = UPPER(TRIM("Status")) WHERE "Status" <> UPPER(TRIM("Status"));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    UPDATE "ItemLista" SET "Status" = 'OK' WHERE "Status" = 'CONCLUIDO';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    UPDATE "ExtratoBancarioItem" SET "TipoLancamento" = 'Saída' WHERE UPPER(TRIM("TipoLancamento")) IN ('SAIDA', 'SAÍDA') AND "TipoLancamento" <> 'Saída';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    UPDATE "ExtratoBancarioItem" SET "TipoLancamento" = 'Entrada' WHERE UPPER(TRIM("TipoLancamento")) = 'ENTRADA' AND "TipoLancamento" <> 'Entrada';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT transactions_parcelas_check CHECK ("QuantidadeParcelas" >= 1 AND "ParcelaAtual" BETWEEN 1 AND "QuantidadeParcelas");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT transactions_status_check CHECK ("Status" IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "Lista" ADD CONSTRAINT lista_tipolista_check CHECK ("TipoLista" IN (1, 2, 3));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "ItemLista" ADD CONSTRAINT itemlista_quantidade_check CHECK ("Quantidade" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "ItemLista" ADD CONSTRAINT itemlista_status_check CHECK ("Status" IN ('PENDENTE', 'AGUARDANDO', 'OK'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_parcelas_check CHECK ("QuantidadeParcelas" >= 1 AND "ParcelaAtual" BETWEEN 1 AND "QuantidadeParcelas");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_tipolancamento_check CHECK ("TipoLancamento" IN ('Entrada', 'Saída'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "ContaVencimento" ADD CONSTRAINT contavencimento_dia_check CHECK ("Dia" BETWEEN 1 AND 31);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "Categories" ADD CONSTRAINT categories_naturezaoperacao_check CHECK ("NaturezaOperacao" BETWEEN 0 AND 5);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "Categories" ADD CONSTRAINT categories_status_check CHECK ("Status" IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "Accounts" ADD CONSTRAINT accounts_parcelas_check CHECK (NOT "EhParcelado" OR ("QuantidadeParcelas" >= 1 AND "ParcelaAtual" BETWEEN 0 AND "QuantidadeParcelas"));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    ALTER TABLE "Accounts" ADD CONSTRAINT accounts_status_check CHECK ("Status" IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013014852_DominiosECheck') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261013014852_DominiosECheck', '8.0.8');
    END IF;
END $EF$;
COMMIT;

