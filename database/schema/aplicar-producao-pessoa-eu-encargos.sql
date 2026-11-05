-- =====================================================================================
-- Pessoa "Eu" e encargos — PRODUÇÃO (branch "development")
-- Card: Acertos com pessoas (#45) · Parte 2 · checklist "Plano — Parte 2", item 4
-- Testado no branch "dev" em 04/11/2026 (migrations PessoaEuAcertos + EncargosDadosAntigos).
--
-- O que muda:
--  * pessoas_movimentacao: eh_titular (a pessoa "Eu") e entra_nos_acertos, booleanos, falsos por padrão;
--    índice único parcial: uma titular por usuário.
--  * Dados: lançamentos com encargo na descrição ou no campo Pessoa (IOF, ANUIDADE, JUROS, ENCARGO(S), TARIFA(S), MULTA(S), MORA,
--    palavra inteira) passam para o "Eu" e a categoria "Tarifas e encargos" (Despesa, Essencial), criados
--    quando faltam (sem descrição, o nome da pessoa vira a descrição); as "pessoas" que eram só encargo (ex.: "DEBITO IOF") e ficaram sem uso são apagadas.
--
-- PRÉ-REQUISITO: aplicar-producao-itens-tipo.sql já aplicado (histórico com 20261103220313_ItensTipo).
-- Publicar junto com a API e o front da Parte 2 do card #45.
--
-- PASSO 0 — CONFERÊNCIA ANTES (só leitura): o que vai mudar.
-- SELECT count(*) AS lancamentos_de_encargo,
--        count(*) FILTER (WHERE categoria_id IS NOT NULL) AS ja_tinham_categoria
-- FROM itens_extrato_bancario
-- WHERE descricao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M'
--    OR nome_pessoa_transacao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M';
--
-- SELECT p.id, p.nome_pessoa, count(i.id) AS lancamentos
-- FROM pessoas_movimentacao p LEFT JOIN itens_extrato_bancario i ON i.pessoa_movimentacao_id = p.id
-- WHERE p.nome_pessoa ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M'
-- GROUP BY p.id, p.nome_pessoa ORDER BY p.nome_pessoa;
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
--
-- PASSO 2 — CONFERÊNCIA DEPOIS (só leitura): nenhum encargo fora do "Eu", no máximo um "Eu" por usuário.
-- SELECT count(*) AS encargos_fora_do_eu
-- FROM itens_extrato_bancario i LEFT JOIN pessoas_movimentacao p ON p.id = i.pessoa_movimentacao_id
-- WHERE (i.descricao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M'
--        OR i.nome_pessoa_transacao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M')
--   AND NOT coalesce(p.eh_titular, false);
-- SELECT usuario_id, count(*) FROM pessoas_movimentacao WHERE eh_titular GROUP BY usuario_id HAVING count(*) > 1;
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105003110_PessoaEuAcertos') THEN
    ALTER TABLE pessoas_movimentacao ADD eh_titular boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105003110_PessoaEuAcertos') THEN
    ALTER TABLE pessoas_movimentacao ADD entra_nos_acertos boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105003110_PessoaEuAcertos') THEN
    CREATE UNIQUE INDEX ux_pessoas_movimentacao_titular ON pessoas_movimentacao (usuario_id) WHERE eh_titular;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105003110_PessoaEuAcertos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261105003110_PessoaEuAcertos', '8.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105004130_EncargosDadosAntigos') THEN

    UPDATE pessoas_movimentacao p SET eh_titular = true
    WHERE p.nome_pessoa = 'EU'
      AND NOT EXISTS (SELECT 1 FROM pessoas_movimentacao t WHERE t.usuario_id = p.usuario_id AND t.eh_titular);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105004130_EncargosDadosAntigos') THEN

    INSERT INTO pessoas_movimentacao (nome_pessoa, eh_titular, entra_nos_acertos, usuario_id)
    SELECT DISTINCT 'EU', true, false, i.usuario_id
    FROM itens_extrato_bancario i
    WHERE (i.descricao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M' OR i.nome_pessoa_transacao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M' OR EXISTS (SELECT 1 FROM pessoas_movimentacao pe WHERE pe.id = i.pessoa_movimentacao_id AND pe.nome_pessoa ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M'))
      AND NOT EXISTS (SELECT 1 FROM pessoas_movimentacao t WHERE t.usuario_id = i.usuario_id AND t.eh_titular);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105004130_EncargosDadosAntigos') THEN

    INSERT INTO categorias (nome, subcategoria, natureza_operacao, ativo, usuario_id)
    SELECT DISTINCT 'Tarifas e encargos', 'Despesa', 1, true, i.usuario_id
    FROM itens_extrato_bancario i
    WHERE (i.descricao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M' OR i.nome_pessoa_transacao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M' OR EXISTS (SELECT 1 FROM pessoas_movimentacao pe WHERE pe.id = i.pessoa_movimentacao_id AND pe.nome_pessoa ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M'))
      AND NOT EXISTS (SELECT 1 FROM categorias c WHERE c.usuario_id = i.usuario_id
                      AND upper(trim(c.nome)) = 'TARIFAS E ENCARGOS' AND upper(trim(c.subcategoria)) = 'DESPESA');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105004130_EncargosDadosAntigos') THEN

    UPDATE itens_extrato_bancario i
    SET descricao = coalesce(nullif(trim(i.descricao), ''), i.nome_pessoa_transacao),
        pessoa_movimentacao_id = eu.id,
        nome_pessoa_transacao = eu.nome_pessoa,
        categoria_id = (SELECT min(c.id) FROM categorias c WHERE c.usuario_id = i.usuario_id
                        AND upper(trim(c.nome)) = 'TARIFAS E ENCARGOS' AND upper(trim(c.subcategoria)) = 'DESPESA')
    FROM pessoas_movimentacao eu
    WHERE eu.usuario_id = i.usuario_id AND eu.eh_titular
      AND (i.descricao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M' OR i.nome_pessoa_transacao ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M' OR EXISTS (SELECT 1 FROM pessoas_movimentacao pe WHERE pe.id = i.pessoa_movimentacao_id AND pe.nome_pessoa ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105004130_EncargosDadosAntigos') THEN

    DELETE FROM pessoas_movimentacao p
    WHERE NOT p.eh_titular
      AND p.nome_pessoa ~* '\m(IOF|ANUIDADES?|JUROS|ENCARGOS?|TARIFAS?|MULTAS?|MORA)\M'
      AND NOT EXISTS (SELECT 1 FROM itens_extrato_bancario i WHERE i.pessoa_movimentacao_id = p.id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261105004130_EncargosDadosAntigos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261105004130_EncargosDadosAntigos', '8.0.8');
    END IF;
END $EF$;
COMMIT;

