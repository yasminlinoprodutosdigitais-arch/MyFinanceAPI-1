-- =====================================================================================
-- Nomes do extrato — PRODUÇÃO (branch "development"). Card #79.
-- O que muda:
--   * itens_extrato_bancario ganha classificado_por_semelhanca;
--   * "&AMP;" vira "&" na descrição e no nome do extrato dos lançamentos;
--   * cada pessoa (menos o "Eu") com categoria ou tipo vira a regra "nome do extrato é igual a NOME";
--   * lançamentos e partes deixam a pessoa (nome do extrato e categoria ficam) e TODAS as pessoas,
--     menos o "Eu", são apagadas (as pessoas de verdade são recriadas à mão depois);
--   * "Tansferência entre contas" é juntada na "Transferência entre contas" (ou só renomeada);
--   * sai pessoas_movimentacao.entra_nos_acertos.
-- PRÉ-REQUISITO: aplicar-producao-contas-compartilhadas.sql já aplicado.
-- ATENÇÃO: a parte 1 (lojas) do aplicar-producao-limpeza-pessoas.sql fica sem efeito depois deste
-- (não sobra loja); as partes 2 e 3 dele continuam valendo depois de recriar as pessoas.
-- Rodar a consulta "antes" e "depois": lançamentos e soma de valores têm que ser iguais.
-- =====================================================================================

SELECT 'antes' AS momento,
       (SELECT count(*) FROM itens_extrato_bancario) AS lancamentos,
       (SELECT sum(valor) FROM itens_extrato_bancario) AS soma_valores,
       (SELECT count(*) FROM pessoas_movimentacao) AS pessoas,
       (SELECT count(*) FROM regras_classificacao) AS regras;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110184935_NomesDoExtrato') THEN
    ALTER TABLE itens_extrato_bancario ADD classificado_por_semelhanca boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110184935_NomesDoExtrato') THEN

                    UPDATE itens_extrato_bancario
                    SET descricao = regexp_replace(descricao, '&amp;', '&', 'gi'),
                        nome_pessoa_transacao = regexp_replace(nome_pessoa_transacao, '&amp;', '&', 'gi')
                    WHERE descricao ~* '&amp;' OR nome_pessoa_transacao ~* '&amp;';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110184935_NomesDoExtrato') THEN

                    WITH n AS (
                        SELECT p.id, p.usuario_id, p.categoria_id, p.tipo_movimentacao_id,
                               regexp_replace(btrim(p.nome_pessoa), '&amp;', '&', 'gi') AS nome
                        FROM pessoas_movimentacao p
                        WHERE NOT p.eh_titular AND (p.categoria_id IS NOT NULL OR p.tipo_movimentacao_id IS NOT NULL)
                    ), c AS (
                        SELECT DISTINCT ON (n.usuario_id, 
                btrim(regexp_replace(
                    translate(upper(
                        regexp_replace(
                            CASE WHEN position('*' IN n.nome) > 0 AND length(btrim(left(n.nome, position('*' IN n.nome) - 1))) > 3
                                 THEN left(n.nome, position('*' IN n.nome) - 1) ELSE n.nome END,
                            '[0-9]', '', 'g')),
                        'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇÑ', 'AAAAAEEEEIIIIOOOOOUUUUCN'),
                    '\s+', ' ', 'g')))
                               n.usuario_id, 
                btrim(regexp_replace(
                    translate(upper(
                        regexp_replace(
                            CASE WHEN position('*' IN n.nome) > 0 AND length(btrim(left(n.nome, position('*' IN n.nome) - 1))) > 3
                                 THEN left(n.nome, position('*' IN n.nome) - 1) ELSE n.nome END,
                            '[0-9]', '', 'g')),
                        'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇÑ', 'AAAAAEEEEIIIIOOOOOUUUUCN'),
                    '\s+', ' ', 'g')) AS texto, n.categoria_id, n.tipo_movimentacao_id, n.id
                        FROM n
                        ORDER BY n.usuario_id, 
                btrim(regexp_replace(
                    translate(upper(
                        regexp_replace(
                            CASE WHEN position('*' IN n.nome) > 0 AND length(btrim(left(n.nome, position('*' IN n.nome) - 1))) > 3
                                 THEN left(n.nome, position('*' IN n.nome) - 1) ELSE n.nome END,
                            '[0-9]', '', 'g')),
                        'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇÑ', 'AAAAAEEEEIIIIOOOOOUUUUCN'),
                    '\s+', ' ', 'g')), n.id
                    )
                    INSERT INTO regras_classificacao (usuario_id, texto, campo, operador, categoria_id, tipo_movimentacao_id, ativa, ordem, vezes_aplicada)
                    SELECT c.usuario_id, c.texto, 'nome_extrato', 'igual', c.categoria_id, c.tipo_movimentacao_id, true,
                           coalesce((SELECT max(r.ordem) FROM regras_classificacao r WHERE r.usuario_id = c.usuario_id), -1)
                               + row_number() OVER (PARTITION BY c.usuario_id ORDER BY c.texto),
                           0
                    FROM c
                    WHERE length(c.texto) >= 2
                      AND NOT EXISTS (SELECT 1 FROM regras_classificacao r
                                      WHERE r.usuario_id = c.usuario_id AND r.campo = 'nome_extrato' AND r.operador = 'igual'
                                        AND r.texto = c.texto AND r.banco_id IS NULL);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110184935_NomesDoExtrato') THEN

                    UPDATE itens_extrato_bancario i SET pessoa_movimentacao_id = NULL
                    FROM pessoas_movimentacao p WHERE p.id = i.pessoa_movimentacao_id AND NOT p.eh_titular;
                    UPDATE extrato_item_partes x SET pessoa_movimentacao_id = NULL
                    FROM pessoas_movimentacao p WHERE p.id = x.pessoa_movimentacao_id AND NOT p.eh_titular;
                    DELETE FROM pessoas_movimentacao WHERE NOT eh_titular;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110184935_NomesDoExtrato') THEN

                    CREATE TEMP TABLE categorias_juntar ON COMMIT DROP AS
                    SELECT e.id AS errada, c.id AS certa
                    FROM categorias e
                    JOIN categorias c ON c.usuario_id = e.usuario_id AND c.id <> e.id
                                     AND c.subcategoria IS NOT DISTINCT FROM e.subcategoria
                                     AND lower(translate(c.nome, 'ê', 'e')) = 'transferencia entre contas'
                    WHERE lower(translate(e.nome, 'ê', 'e')) = 'tansferencia entre contas';

                    UPDATE itens_extrato_bancario t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                    UPDATE extrato_item_partes t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                    UPDATE regras_classificacao t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                    UPDATE notas_fiscais_itens t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                    UPDATE contas t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                    UPDATE transacoes t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                    UPDATE pessoas_movimentacao t SET categoria_id = j.certa FROM categorias_juntar j WHERE t.categoria_id = j.errada;
                    DELETE FROM categorias WHERE id IN (SELECT errada FROM categorias_juntar);

                    UPDATE categorias SET nome = 'Transferência entre contas'
                    WHERE lower(translate(nome, 'ê', 'e')) = 'tansferencia entre contas';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110184935_NomesDoExtrato') THEN
    ALTER TABLE pessoas_movimentacao DROP COLUMN entra_nos_acertos;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261110184935_NomesDoExtrato') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261110184935_NomesDoExtrato', '8.0.8');
    END IF;
END $EF$;
COMMIT;


SELECT 'depois' AS momento,
       (SELECT count(*) FROM itens_extrato_bancario) AS lancamentos,
       (SELECT sum(valor) FROM itens_extrato_bancario) AS soma_valores,
       (SELECT count(*) FROM pessoas_movimentacao) AS pessoas,
       (SELECT count(*) FROM regras_classificacao) AS regras;
