-- =====================================================================================
-- Acertos com pessoas: pagamento que abate uma compra — PRODUÇÃO (branch "development")
-- Card: Acertos com pessoas (#45) · Parte 3 · checklist "Plano — Parte 3", item 1
-- Testado no branch "dev" em 05/11/2026 (migration AcertosAbateItem): coluna criada sem mexer nos dados.
--
-- O que muda:
--  * itens_extrato_bancario: abate_item_id integer null, FK para o próprio lançamento (ON DELETE SET NULL) + índice.
--    O pagamento (Entrada) de uma pessoa aponta a compra (Saída) que abate; nulo = saldo geral.
--
-- PRÉ-REQUISITO: aplicar-producao-pessoa-eu-encargos.sql já aplicado (histórico com 20261105004130_EncargosDadosAntigos).
-- Publicar junto com a API e o front da Parte 3 do card #45.
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
--
-- PASSO 2 — CONFERÊNCIA (só leitura): coluna criada e vazia.
-- SELECT count(abate_item_id) AS pagamentos_ligados FROM itens_extrato_bancario;
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105013236_AcertosAbateItem') THEN
    ALTER TABLE itens_extrato_bancario ADD abate_item_id integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105013236_AcertosAbateItem') THEN
    CREATE INDEX ix_itens_extrato_bancario_abate_item_id ON itens_extrato_bancario (abate_item_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105013236_AcertosAbateItem') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_itens_extrato_bancario_abate_item_id FOREIGN KEY (abate_item_id) REFERENCES itens_extrato_bancario (id) ON DELETE SET NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105013236_AcertosAbateItem') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261105013236_AcertosAbateItem', '8.0.8');
    END IF;
END $EF$;
COMMIT;

