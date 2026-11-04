-- =====================================================================================
-- Planejamentos: campo Tipo nos itens — PRODUÇÃO (branch "development")
-- Card: Ajustes encontrados (#61) · checklist "Plano", item 2
-- Testado no branch "dev" em 03/11/2026 (migration ItensTipo): coluna criada sem mexer nos dados.
--
-- O que muda:
--  * listas: usa_tipo boolean not null default false (o campo Tipo começa desligado em todos os planejamentos).
--  * itens_lista: tipo varchar(60) null (texto livre para agrupar e filtrar).
--
-- PRÉ-REQUISITO: aplicar-producao-listas-cor.sql já aplicado (histórico com 20261029163658_ListasCor).
-- Publicar junto com a API e o front do card #61.
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
--
-- PASSO 2 — CONFERÊNCIA (só leitura): colunas criadas, nenhum planejamento com Tipo ligado, nenhum item com tipo.
-- SELECT (SELECT count(*) FROM listas WHERE usa_tipo) AS listas_com_tipo,
--        (SELECT count(tipo) FROM itens_lista) AS itens_com_tipo;
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261103220313_ItensTipo') THEN
    ALTER TABLE listas ADD usa_tipo boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261103220313_ItensTipo') THEN
    ALTER TABLE itens_lista ADD tipo character varying(60);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261103220313_ItensTipo') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261103220313_ItensTipo', '8.0.8');
    END IF;
END $EF$;
COMMIT;
