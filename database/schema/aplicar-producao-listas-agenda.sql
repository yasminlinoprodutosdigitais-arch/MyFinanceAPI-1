-- =====================================================================================
-- Listas: agenda (hora e repetição) — PRODUÇÃO (branch "development")
-- Card: Listas — modelo com campos configuráveis + arquivar (#67) · checklist "Plano — Parte 3 (Agenda)", item 2
-- Testado no branch "dev" em 29/10/2026 (migration ListasAgenda): 194 itens, todos sem repetição; ida e volta conferida.
--
-- O que muda:
--  * itens_lista: horario_tarefa vira hora (sem hora = dia todo); entram repeticao (NENHUMA | DIARIA | SEMANAL | MENSAL),
--    dias_semana (int[], 1 = seg … 7 = dom) e repetir_ate, com os CHECKs da regra.
--  * Tabela nova itens_lista_ocorrencias: só as ocorrências que diferem da regra (feita, movida ou pulada).
--  * Nada a converter (horario_tarefa estava vazio em todos os itens do dev; conferir no Passo 0).
--
-- PRÉ-REQUISITO: aplicar-producao-listas-serie.sql já aplicado (histórico com ..._ListasSerie).
-- Publicar junto com a API desta parte e com os cards de front #63, #66 e #69.
--
-- PASSO 0 — CONFERÊNCIA (só leitura): quantos itens têm horário (devem continuar com ele, agora em "hora").
-- SELECT count(*) FILTER (WHERE horario_tarefa IS NOT NULL) AS com_horario, count(*) AS itens FROM itens_lista;
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
--
-- PASSO 2 — CONFERÊNCIA (só leitura): mesmo número de itens com hora do Passo 0, todos sem repetição.
-- SELECT count(*) FILTER (WHERE hora IS NOT NULL) AS com_hora, count(*) FILTER (WHERE repeticao = 'NENHUMA') AS sem_repeticao FROM itens_lista;
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    ALTER TABLE itens_lista RENAME COLUMN horario_tarefa TO hora;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    ALTER TABLE itens_lista ADD dias_semana integer[];
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    ALTER TABLE itens_lista ADD repeticao character varying(10) NOT NULL DEFAULT 'NENHUMA';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    ALTER TABLE itens_lista ADD repetir_ate date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    CREATE TABLE itens_lista_ocorrencias (
        id serial NOT NULL,
        item_id integer NOT NULL,
        data date NOT NULL,
        feito boolean NOT NULL DEFAULT FALSE,
        feito_em timestamp with time zone,
        nova_data date,
        nova_hora time without time zone,
        pulada boolean NOT NULL DEFAULT FALSE,
        data_alteracao timestamp with time zone NOT NULL DEFAULT (now()),
        usuario_id integer NOT NULL,
        CONSTRAINT pk_itens_lista_ocorrencias PRIMARY KEY (id),
        CONSTRAINT fk_itens_lista_ocorrencias_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE,
        CONSTRAINT fk_itens_lista_ocorrencias_itens_lista_item_id FOREIGN KEY (item_id) REFERENCES itens_lista (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_dias_semana_check CHECK (dias_semana <@ ARRAY[1, 2, 3, 4, 5, 6, 7]);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_repeticao_check CHECK (repeticao IN ('NENHUMA', 'DIARIA', 'SEMANAL', 'MENSAL'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_repeticao_com_data_check CHECK (repeticao = 'NENHUMA' OR data IS NOT NULL);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_repetir_ate_check CHECK (repetir_ate >= data);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_semanal_com_dias_check CHECK (repeticao <> 'SEMANAL' OR cardinality(dias_semana) > 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    CREATE UNIQUE INDEX ix_itens_lista_ocorrencias_item_id_data ON itens_lista_ocorrencias (item_id, data);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    CREATE INDEX ix_itens_lista_ocorrencias_usuario_id ON itens_lista_ocorrencias (usuario_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029141958_ListasAgenda') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261029141958_ListasAgenda', '8.0.8');
    END IF;
END $EF$;
COMMIT;

