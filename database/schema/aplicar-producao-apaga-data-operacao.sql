-- =====================================================================================
-- [DB] Apagar a coluna contas.data_operacao — PRODUÇÃO (branch "development")
-- Card: [DB] Apagar a coluna morta contas.data_operacao (#25) · checklist "Plano"
-- Testado no branch "dev" em 24/10/2026 (migration 20261024184409_ApagaDataOperacao): 10 contas sem
-- vencimento ganharam o dia que estava em data_operacao (83 → 93 vencimentos); "outros" (valor 0)
-- ficou sem dia; a coluna saiu.
--
-- PRÉ-REQUISITO: aplicar-producao-ativo-booleano.sql já aplicado (o histórico de migrations precisa
-- ter 20261024171824_AtivoBooleano). Publicar a API desta mudança junto (a API antiga mapeia a coluna).
-- ANTES DE TUDO: branch de restauração no Neon. Os valores antigos de data_operacao não voltam.
--
-- PASSO 0 — CONFERÊNCIA (só leitura).
-- (a) Contas que vão ganhar vencimento (as que não têm nenhum) e o dia que vão receber:
-- SELECT c.id, c.nome, c.ativo, c.data_operacao FROM contas c
-- WHERE c.data_operacao IS NOT NULL AND NOT EXISTS (SELECT 1 FROM contas_vencimento v WHERE v.conta_id = c.id)
-- ORDER BY c.id;
-- (b) Para guardar, se quiser: todos os valores que a coluna tem hoje.
-- SELECT id, nome, data_operacao FROM contas WHERE data_operacao IS NOT NULL ORDER BY id;
-- Depois do Passo 1, as contas de (a) com dia de 1 a 31 aparecem em contas_vencimento com esse dia.
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024184409_ApagaDataOperacao') THEN

    INSERT INTO contas_vencimento (conta_id, dia, usuario_id)
    SELECT DISTINCT c.id, trim(d.valor)::integer, c.usuario_id
    FROM contas c
    CROSS JOIN LATERAL unnest(string_to_array(c.data_operacao, ',')) AS d(valor)
    WHERE trim(d.valor) ~ '^[0-9]{1,2}$'
      AND trim(d.valor)::integer BETWEEN 1 AND 31
      AND NOT EXISTS (SELECT 1 FROM contas_vencimento v WHERE v.conta_id = c.id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024184409_ApagaDataOperacao') THEN
    ALTER TABLE contas DROP COLUMN data_operacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024184409_ApagaDataOperacao') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261024184409_ApagaDataOperacao', '8.0.8');
    END IF;
END $EF$;
COMMIT;

