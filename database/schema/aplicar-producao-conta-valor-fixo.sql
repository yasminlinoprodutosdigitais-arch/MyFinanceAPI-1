-- =====================================================================================
-- Ajustes pontuais de regra — valor fixo da conta — PRODUÇÃO (branch "development")
-- Card: Ajustes pontuais de regra (#47) · checklist "Plano", item 1
-- Testado no branch "dev" em 27/10/2026 (migration 20261027171229_ContaValorFixo): uma coluna nova, boolean,
-- padrão false; todas as contas existentes ficam como valor NÃO fixo (decisão do card).
--
-- PRÉ-REQUISITO: aplicar-producao-conta-observacao.sql já aplicado (o histórico de migrations
-- precisa ter 20261026182515_ContaObservacao). Publicar a API e o front desta mudança juntos.
--
-- PASSO 0 — CONFERÊNCIA (só leitura): a coluna ainda não existe.
-- SELECT column_name FROM information_schema.columns WHERE table_name = 'contas' ORDER BY ordinal_position;
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261027171229_ContaValorFixo') THEN
    ALTER TABLE contas ADD valor_fixo boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261027171229_ContaValorFixo') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261027171229_ContaValorFixo', '8.0.8');
    END IF;
END $EF$;
COMMIT;

