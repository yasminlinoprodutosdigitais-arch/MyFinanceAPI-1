-- =====================================================================================
-- [FE] Cadastro de Categorias — ícone e cor da categoria — PRODUÇÃO (branch "development")
-- Card: Cadastro de Categorias — novo layout (#23) · checklist "Plano", item 1
-- Testado no branch "dev" em 25/10/2026 (migration 20261025000453_CategoriaIconeCor): duas colunas
-- novas, nulas; nenhuma linha muda (nulo = o front usa o ícone pelo nome e a cor da natureza).
--
-- PRÉ-REQUISITO: aplicar-producao-apaga-data-operacao.sql já aplicado (o histórico de migrations
-- precisa ter 20261024184409_ApagaDataOperacao). Publicar a API e o front desta mudança juntos.
--
-- PASSO 0 — CONFERÊNCIA (só leitura): as colunas ainda não existem.
-- SELECT column_name FROM information_schema.columns WHERE table_name = 'categorias' ORDER BY ordinal_position;
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261025000453_CategoriaIconeCor') THEN
    ALTER TABLE categorias ADD cor character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261025000453_CategoriaIconeCor') THEN
    ALTER TABLE categorias ADD icone character varying(40);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261025000453_CategoriaIconeCor') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261025000453_CategoriaIconeCor', '8.0.8');
    END IF;
END $EF$;
COMMIT;

