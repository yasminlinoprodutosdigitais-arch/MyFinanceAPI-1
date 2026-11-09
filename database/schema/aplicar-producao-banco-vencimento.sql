-- =====================================================================================
-- Dia do vencimento do cartão — PRODUÇÃO (branch "development"). Card #76.
-- O que muda: coluna opcional dia_vencimento em bancos; não mexe nos dados.
-- PRÉ-REQUISITO: aplicar-producao-dividir-lancamento.sql já aplicado.
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261108234404_BancoDiaVencimento') THEN
    ALTER TABLE bancos ADD dia_vencimento integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261108234404_BancoDiaVencimento') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261108234404_BancoDiaVencimento', '8.0.8');
    END IF;
END $EF$;
COMMIT;

