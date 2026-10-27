-- =====================================================================================
-- [FE] Cadastro de Contas — observação da conta — PRODUÇÃO (branch "development")
-- Card: Cadastro de Contas — novo layout (#22) · checklist "Plano", item 1
-- Testado no branch "dev" em 26/10/2026 (migration 20261026182515_ContaObservacao): uma coluna nova,
-- nula; nenhuma linha muda.
--
-- PRÉ-REQUISITO: aplicar-producao-categoria-icone-cor.sql já aplicado (o histórico de migrations
-- precisa ter 20261025000453_CategoriaIconeCor). Publicar a API e o front desta mudança juntos.
--
-- PASSO 0 — CONFERÊNCIA (só leitura): a coluna ainda não existe.
-- SELECT column_name FROM information_schema.columns WHERE table_name = 'contas' ORDER BY ordinal_position;
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026182515_ContaObservacao') THEN
    ALTER TABLE contas ADD observacao character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026182515_ContaObservacao') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261026182515_ContaObservacao', '8.0.8');
    END IF;
END $EF$;
COMMIT;

