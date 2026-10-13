-- =====================================================================================
-- [DB] P1 subprojeto 2 — Datas — aplicação em PRODUÇÃO (branch "development" do Neon)
-- Card: [DB] P1 — Modelagem (13) · checklist "Plano — Subprojeto 2 (Datas)"
-- Testado no branch "dev" em 12–13/10/2026 (migration 20261013020452_DataTransacaoDate).
--
-- PRÉ-REQUISITOS, nesta ordem: aplicar-producao-2026-10-05.sql, aplicar-producao-integridade.sql,
-- aplicar-producao-dominios.sql.
-- Publicar a API da branch db-p1-modelagem JUNTO: Transaction.Date passa a ser DateOnly. A API nova
-- aceita tanto "YYYY-MM-DD" quanto o instante ISO do front antigo; a API antiga NÃO lê a coluna date.
-- ANTES DE TUDO: branch de restauração no Neon a partir de "development". O horário gravado se perde.
--
-- PASSO 0 — CONFERÊNCIA (só leitura).
-- (a) Quantas datas exibidas vão mudar: são as gravadas à meia-noite UTC, que hoje aparecem
--     um dia antes em BRT. No dev eram 166 de 408.
-- SELECT count(*) FILTER (WHERE ("Date" AT TIME ZONE 'UTC')::date <> ("Date" AT TIME ZONE 'America/Sao_Paulo')::date) AS muda, count(*) AS total FROM "Transactions";
--
-- (b) A regra (data UTC) bate com o dia de vencimento da conta? Esperado: a maioria das 00h
--     batendo pela UTC e quase nenhuma pela BRT. No dev: 00h → 107 pela UTC, 1 pela BRT.
-- SELECT to_char(t."Date" AT TIME ZONE 'UTC','HH24') AS hora_utc, count(*) AS total,
--   count(*) FILTER (WHERE EXISTS (SELECT 1 FROM "ContaVencimento" cv WHERE cv."ContaId" = t."IdAccount" AND cv."Dia" = extract(day FROM (t."Date" AT TIME ZONE 'UTC'))::int)) AS bate_utc,
--   count(*) FILTER (WHERE EXISTS (SELECT 1 FROM "ContaVencimento" cv WHERE cv."ContaId" = t."IdAccount" AND cv."Dia" = extract(day FROM (t."Date" AT TIME ZONE 'America/Sao_Paulo'))::int)) AS bate_brt
-- FROM "Transactions" t GROUP BY 1 ORDER BY 1;
--
-- (c) Alguma hora fora de 00h–18h UTC? Se houver 21h ou mais, a data UTC pode cair no dia
--     seguinte ao pretendido: PARAR e revisar a regra antes de aplicar.
-- SELECT to_char("Date" AT TIME ZONE 'UTC','HH24') AS hora, count(*) FROM "Transactions" GROUP BY 1 ORDER BY 1;
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent). Uma transação só.
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013020452_DataTransacaoDate') THEN
    ALTER TABLE "Transactions" ALTER COLUMN "Date" TYPE date USING ("Date" AT TIME ZONE 'UTC')::date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013020452_DataTransacaoDate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261013020452_DataTransacaoDate', '8.0.8');
    END IF;
END $EF$;
COMMIT;

