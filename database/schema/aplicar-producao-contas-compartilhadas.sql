-- =====================================================================================
-- Contas compartilhadas — PRODUÇÃO (branch "development"). Card #77, entrega 1.
-- O que muda: colunas em bancos (compartilhado, dia de fechamento, convite dispensado) e a
-- tabela nova bancos_pessoas; não mexe nos dados.
-- PRÉ-REQUISITO: aplicar-producao-importacoes-historico.sql já aplicado.
-- =====================================================================================

﻿START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110000741_ContasCompartilhadas') THEN
    ALTER TABLE bancos ADD compartilhado boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110000741_ContasCompartilhadas') THEN
    ALTER TABLE bancos ADD convite_compartilhar_dispensado boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110000741_ContasCompartilhadas') THEN
    ALTER TABLE bancos ADD dia_fechamento integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110000741_ContasCompartilhadas') THEN
    CREATE TABLE bancos_pessoas (
        banco_id integer NOT NULL,
        pessoa_movimentacao_id integer NOT NULL,
        CONSTRAINT pk_bancos_pessoas PRIMARY KEY (banco_id, pessoa_movimentacao_id),
        CONSTRAINT fk_bancos_pessoas_bancos_banco_id FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE CASCADE,
        CONSTRAINT fk_bancos_pessoas_pessoas_movimentacao_pessoa_movimentacao_id FOREIGN KEY (pessoa_movimentacao_id) REFERENCES pessoas_movimentacao (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110000741_ContasCompartilhadas') THEN
    CREATE INDEX ix_bancos_pessoas_pessoa_movimentacao_id ON bancos_pessoas (pessoa_movimentacao_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110000741_ContasCompartilhadas') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261110000741_ContasCompartilhadas', '8.0.8');
    END IF;
END $EF$;
COMMIT;

