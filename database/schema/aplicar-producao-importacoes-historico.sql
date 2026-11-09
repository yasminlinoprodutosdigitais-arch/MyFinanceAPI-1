-- =====================================================================================
-- Histórico de importações — PRODUÇÃO (branch "development"). Card #75.
-- O que muda: colunas opcionais em extratos_bancarios; não mexe nos dados.
-- PRÉ-REQUISITO: aplicar-producao-notas-fiscais.sql já aplicado.
-- =====================================================================================

﻿START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261109013810_ExtratoImportacaoHistorico') THEN
    ALTER TABLE extratos_bancarios ADD desfeito_em timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261109013810_ExtratoImportacaoHistorico') THEN
    ALTER TABLE extratos_bancarios ADD mes_referencia date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261109013810_ExtratoImportacaoHistorico') THEN
    ALTER TABLE extratos_bancarios ADD motivo_desfazer character varying(200);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261109013810_ExtratoImportacaoHistorico') THEN
    ALTER TABLE extratos_bancarios ADD quantidade_duplicados integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261109013810_ExtratoImportacaoHistorico') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261109013810_ExtratoImportacaoHistorico', '8.0.8');
    END IF;
END $EF$;
COMMIT;

