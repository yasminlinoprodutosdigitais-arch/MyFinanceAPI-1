-- =====================================================================================
-- Remover o lançamento de teste "testando" (R$ 15.000,00, conta 14 "Entrada apartamento",
-- 01/10/2025) — PRODUÇÃO (branch "development")
-- Card: [DB] Transações duplicadas (mesma conta e mesma data), decisão de 24/10/2026.
-- Feito no branch "dev" em 24/10/2026 (id 59, pela API).
--
-- Funciona antes ou depois do [DB] P3 (aplicar-producao-convencoes.sql): usa "Transactions"
-- ou transacoes, o que existir.
--
-- PASSO 0 — CONFERÊNCIA (só leitura). Precisa voltar exatamente 1 linha, com o id 59.
-- Antes do [DB] P3:
-- SELECT "Id", "IdAccount", "Name", "Value", "Date", "Status" FROM "Transactions"
-- WHERE "IdAccount" = 14 AND "Name" = 'testando' AND "Value" = 15000;
-- Depois do [DB] P3:
-- SELECT id, conta_id, nome, valor, data, situacao FROM transacoes
-- WHERE conta_id = 14 AND nome = 'testando' AND valor = 15000;
--
-- PASSO 1 — o bloco abaixo. Só apaga se achar exatamente o registro conferido; senão aborta.
-- =====================================================================================

START TRANSACTION;

DO $$
DECLARE
    apagadas integer;
BEGIN
    IF to_regclass('public.transacoes') IS NOT NULL THEN
        DELETE FROM transacoes
        WHERE id = 59 AND conta_id = 14 AND nome = 'testando' AND valor = 15000 AND data::date = DATE '2025-10-01';
    ELSE
        DELETE FROM "Transactions"
        WHERE "Id" = 59 AND "IdAccount" = 14 AND "Name" = 'testando' AND "Value" = 15000 AND "Date"::date = DATE '2025-10-01';
    END IF;

    GET DIAGNOSTICS apagadas = ROW_COUNT;
    IF apagadas <> 1 THEN
        RAISE EXCEPTION 'Esperava apagar 1 lançamento "testando" e apagaria %. Nada foi alterado.', apagadas;
    END IF;
END $$;

COMMIT;
