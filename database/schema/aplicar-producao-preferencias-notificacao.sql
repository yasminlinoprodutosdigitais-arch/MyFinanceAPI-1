-- =====================================================================================
-- Preferências de notificação — PRODUÇÃO (branch "development"). Card #73.
-- O que muda:
--   * tabela nova preferencias_notificacao (uma linha por usuário: lidas_ate + config jsonb).
--     Sem linha = padrões; nada é copiado nem alterado nas tabelas que já existem.
-- PRÉ-REQUISITO: aplicar-producao-nomes-do-extrato.sql já aplicado.
-- Idempotente: pode rodar de novo sem efeito.
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261111114222_PreferenciasNotificacao') THEN
    CREATE TABLE preferencias_notificacao (
        usuario_id integer NOT NULL,
        lidas_ate timestamp with time zone,
        config jsonb NOT NULL DEFAULT ('{}'::jsonb),
        data_alteracao timestamp with time zone NOT NULL DEFAULT (now()),
        data_criacao timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT pk_preferencias_notificacao PRIMARY KEY (usuario_id),
        CONSTRAINT fk_preferencias_notificacao_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261111114222_PreferenciasNotificacao') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261111114222_PreferenciasNotificacao', '8.0.8');
    END IF;
END $EF$;
COMMIT;


-- Conferência: a tabela existe e está vazia.
SELECT count(*) AS preferencias FROM preferencias_notificacao;
