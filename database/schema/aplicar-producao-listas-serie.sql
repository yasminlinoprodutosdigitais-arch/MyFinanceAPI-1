-- =====================================================================================
-- Listas: série e origem para comparar — PRODUÇÃO (branch "development")
-- Card: Listas — modelo com campos configuráveis + arquivar (#67) · checklist "Plano — Parte 2", item 2
-- Testado no branch "dev" em 28/10/2026 (migration ListasSerie): 13 listas em 13 séries (cada lista existente
-- começa na própria série), nenhuma origem preenchida. Ida e volta (Up + Down) conferida.
--
-- O que muda:
--  * listas: serie_id (uuid, gen_random_uuid() linha a linha) e lista_origem_id (FK para listas, ON DELETE SET NULL).
--  * itens_lista: item_origem_id (FK para itens_lista, ON DELETE SET NULL).
--  * O índice (usuario_id) de listas vira (usuario_id, serie_id).
--
-- PRÉ-REQUISITO: aplicar-producao-listas-campos.sql já aplicado (histórico com 20261028233634_ListasCamposConfiguraveis).
-- Publicar junto com a API desta parte e com os cards de front #63 e #66.
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
--
-- PASSO 2 — CONFERÊNCIA (só leitura): uma série por lista.
-- SELECT count(*) AS listas, count(DISTINCT serie_id) AS series FROM listas;
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    DROP INDEX ix_listas_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    ALTER TABLE listas ADD lista_origem_id integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    ALTER TABLE listas ADD serie_id uuid NOT NULL DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    ALTER TABLE itens_lista ADD item_origem_id integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    CREATE INDEX ix_listas_lista_origem_id ON listas (lista_origem_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    CREATE INDEX ix_listas_usuario_id_serie_id ON listas (usuario_id, serie_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    CREATE INDEX ix_itens_lista_item_origem_id ON itens_lista (item_origem_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT fk_itens_lista_itens_lista_item_origem_id FOREIGN KEY (item_origem_id) REFERENCES itens_lista (id) ON DELETE SET NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    ALTER TABLE listas ADD CONSTRAINT fk_listas_listas_lista_origem_id FOREIGN KEY (lista_origem_id) REFERENCES listas (id) ON DELETE SET NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029003413_ListasSerie') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261029003413_ListasSerie', '8.0.8');
    END IF;
END $EF$;
COMMIT;

