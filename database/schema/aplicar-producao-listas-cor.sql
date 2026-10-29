-- =====================================================================================
-- Listas: cor escolhida junto com o ícone — PRODUÇÃO (branch "development")
-- Card: [LISTAS] Cadastro de Listas (#63) · checklist "Plano", item 1
-- Testado no branch "dev" em 29/10/2026 (migration ListasCor): 13 listas, todas sem cor (o front deriva a cor dos
-- campos); o CHECK recusa cor fora da paleta. Ida e volta (Up + Down) conferida.
--
-- O que muda:
--  * listas: cor varchar(20) null + CHECK listas_cor_check (as 10 chaves da paleta de Categorias).
--
-- PRÉ-REQUISITO: aplicar-producao-listas-agenda.sql já aplicado (histórico com 20261029141958_ListasAgenda).
-- Publicar junto com a API e os fronts #63/#66 (ver os outros scripts de listas).
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
--
-- PASSO 2 — CONFERÊNCIA (só leitura): coluna criada, nenhuma lista com cor ainda.
-- SELECT count(*) AS listas, count(cor) AS com_cor FROM listas;
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029163658_ListasCor') THEN
    ALTER TABLE listas ADD cor character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029163658_ListasCor') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_cor_check CHECK (cor IN ('azul', 'roxo', 'verde-agua', 'verde', 'laranja', 'indigo', 'rosa', 'vermelho', 'amarelo', 'cinza'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261029163658_ListasCor') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261029163658_ListasCor', '8.0.8');
    END IF;
END $EF$;
COMMIT;

