-- =====================================================================================
-- Limpeza de pessoas — PRODUÇÃO (branch "development"). Card #77, entrega 3.
-- O que muda: SÓ o que estiver nas três listas abaixo (ids da conferir-producao-limpeza-pessoas.sql).
--   1) lojas: os lançamentos e partes deixam de apontar para a "pessoa" (o nome vai para a descrição
--      se ainda não estiver lá); a categoria/tipo padrão vira regra de classificação; a "pessoa" é apagada.
--   2) categorias_pessoa: os lançamentos da categoria (desde a data, se informada) ganham a pessoa quando
--      estavam sem pessoa e trocam para a categoria nova (ou ficam sem); a categoria antiga é inativada.
--      ATENÇÃO: lançamentos que ganham uma pessoa dos acertos passam a contar como devidos por ela.
--   3) contas_pessoa: renomeia a conta e/ou a marca como compartilhada com a pessoa.
-- PRÉ-REQUISITO: aplicar-producao-contas-compartilhadas.sql já aplicado.
-- Tudo numa transação: qualquer conferência que falhar desfaz tudo.
-- NÃO COMMITAR a versão preenchida (tem ids e nomes dos seus dados).
-- =====================================================================================

BEGIN;

-- ---------- PREENCHA AS LISTAS ----------

-- 1) Lojas: troque ARRAY[]::int[] por ARRAY[12, 15, 40] (pessoa_id da conferência, item 1).
CREATE TEMP TABLE lojas (pessoa_id int PRIMARY KEY) ON COMMIT DROP;
INSERT INTO lojas (pessoa_id) SELECT unnest(ARRAY[]::int[]);

-- 2) Categorias com nome de pessoa: troque o SELECT vazio por linhas, por exemplo
--    VALUES (31, 20, 5, NULL::date), (32, 21, NULL, DATE '2026-06-01')
--    = (categoria_id, pessoa_id, nova_categoria_id ou NULL = sem categoria, desde ou NULL = todos os lançamentos).
CREATE TEMP TABLE categorias_pessoa (categoria_id int PRIMARY KEY, pessoa_id int NOT NULL, nova_categoria_id int, desde date) ON COMMIT DROP;
INSERT INTO categorias_pessoa (categoria_id, pessoa_id, nova_categoria_id, desde)
SELECT * FROM (VALUES (0, 0, NULL::int, NULL::date)) AS v(categoria_id, pessoa_id, nova_categoria_id, desde) WHERE false;

-- 3) Contas com pessoa no nome: troque o SELECT vazio por linhas, por exemplo
--    VALUES (7, 'NuBank Cred', 20), (8, NULL, 21)
--    = (banco_id, nome novo ou NULL = não renomeia, pessoa_id para compartilhar ou NULL = não compartilha).
CREATE TEMP TABLE contas_pessoa (banco_id int PRIMARY KEY, novo_nome text, pessoa_id int) ON COMMIT DROP;
INSERT INTO contas_pessoa (banco_id, novo_nome, pessoa_id)
SELECT * FROM (VALUES (0, NULL::text, NULL::int)) AS v(banco_id, novo_nome, pessoa_id) WHERE false;

-- ---------- CONFERÊNCIAS (falhou = nada é gravado) ----------
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM lojas l LEFT JOIN pessoas_movimentacao p ON p.id = l.pessoa_id
               WHERE p.id IS NULL OR p.eh_titular OR p.entra_nos_acertos) THEN
        RAISE EXCEPTION 'Lojas: id inexistente, o titular ou alguém que entra nos acertos.';
    END IF;
    IF EXISTS (SELECT 1 FROM categorias_pessoa cp
               LEFT JOIN categorias c ON c.id = cp.categoria_id
               LEFT JOIN pessoas_movimentacao p ON p.id = cp.pessoa_id
               LEFT JOIN categorias n ON n.id = cp.nova_categoria_id
               WHERE c.id IS NULL OR p.id IS NULL OR p.eh_titular OR c.usuario_id <> p.usuario_id
                  OR cp.nova_categoria_id = cp.categoria_id
                  OR (cp.nova_categoria_id IS NOT NULL AND (n.id IS NULL OR n.usuario_id <> c.usuario_id))) THEN
        RAISE EXCEPTION 'Categorias: id inexistente, pessoa titular, categoria nova igual à antiga ou de outro usuário.';
    END IF;
    IF EXISTS (SELECT 1 FROM categorias_pessoa cp JOIN lojas l ON l.pessoa_id = cp.pessoa_id)
       OR EXISTS (SELECT 1 FROM contas_pessoa cp JOIN lojas l ON l.pessoa_id = cp.pessoa_id) THEN
        RAISE EXCEPTION 'Uma pessoa está na lista de lojas e também numa categoria ou conta.';
    END IF;
    IF EXISTS (SELECT 1 FROM contas_pessoa cp
               LEFT JOIN bancos b ON b.id = cp.banco_id
               LEFT JOIN pessoas_movimentacao p ON p.id = cp.pessoa_id
               WHERE b.id IS NULL
                  OR (cp.pessoa_id IS NOT NULL AND (p.id IS NULL OR p.eh_titular OR p.usuario_id <> b.usuario_id))) THEN
        RAISE EXCEPTION 'Contas: id inexistente ou pessoa titular/de outro usuário.';
    END IF;
END $$;

-- Antes: para comparar com o fim.
SELECT 'antes' AS momento,
       (SELECT count(*) FROM itens_extrato_bancario) AS lancamentos,
       (SELECT sum(valor) FROM itens_extrato_bancario) AS soma_valores,
       (SELECT count(*) FROM pessoas_movimentacao) AS pessoas,
       (SELECT count(*) FROM regras_classificacao) AS regras;

-- ---------- 1) LOJAS ----------
-- A categoria/tipo padrão da loja vira regra (mesmo texto normalizado da API: sem acento, maiúsculas, espaços simples).
INSERT INTO regras_classificacao (usuario_id, texto, campo, operador, categoria_id, tipo_movimentacao_id, ativa, ordem, vezes_aplicada)
SELECT DISTINCT ON (p.usuario_id, t.texto)
       p.usuario_id, t.texto, 'descricao_ou_pessoa', 'contem', p.categoria_id, p.tipo_movimentacao_id, true, 0, 0
FROM pessoas_movimentacao p
JOIN lojas l ON l.pessoa_id = p.id
CROSS JOIN LATERAL (SELECT regexp_replace(translate(upper(trim(p.nome_pessoa)), 'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇ', 'AAAAAEEEEIIIIOOOOOUUUUC'), '\s+', ' ', 'g') AS texto) t
WHERE (p.categoria_id IS NOT NULL OR p.tipo_movimentacao_id IS NOT NULL)
  AND NOT EXISTS (SELECT 1 FROM regras_classificacao r WHERE r.usuario_id = p.usuario_id AND r.texto = t.texto);

-- O nome da loja não se perde: vai para o fim da descrição quando ela ainda não o tem.
UPDATE itens_extrato_bancario i
SET descricao = left(CASE WHEN coalesce(trim(i.descricao), '') = '' THEN p.nome_pessoa ELSE trim(i.descricao) || ' · ' || p.nome_pessoa END, 200)
FROM pessoas_movimentacao p
JOIN lojas l ON l.pessoa_id = p.id
WHERE i.pessoa_movimentacao_id = p.id
  AND position(upper(trim(p.nome_pessoa)) IN upper(coalesce(i.descricao, ''))) = 0;

UPDATE itens_extrato_bancario SET pessoa_movimentacao_id = NULL, nome_pessoa_transacao = NULL
WHERE pessoa_movimentacao_id IN (SELECT pessoa_id FROM lojas);

UPDATE extrato_item_partes SET pessoa_movimentacao_id = NULL
WHERE pessoa_movimentacao_id IN (SELECT pessoa_id FROM lojas);

-- Regras que ligavam a loja ficam sem pessoa (FK SET NULL); bancos_pessoas some em cascata.
DELETE FROM pessoas_movimentacao WHERE id IN (SELECT pessoa_id FROM lojas);

-- ---------- 2) CATEGORIAS COM NOME DE PESSOA ----------
-- Primeiro a pessoa (enquanto a categoria antiga ainda identifica os lançamentos).
UPDATE itens_extrato_bancario i
SET pessoa_movimentacao_id = cp.pessoa_id, nome_pessoa_transacao = p.nome_pessoa
FROM categorias_pessoa cp
JOIN pessoas_movimentacao p ON p.id = cp.pessoa_id
WHERE i.categoria_id = cp.categoria_id
  AND i.pessoa_movimentacao_id IS NULL
  AND (cp.desde IS NULL OR i.data_movimentacao >= cp.desde);

UPDATE extrato_item_partes x
SET pessoa_movimentacao_id = cp.pessoa_id
FROM categorias_pessoa cp, itens_extrato_bancario i
WHERE x.categoria_id = cp.categoria_id
  AND i.id = x.extrato_bancario_item_id
  AND x.pessoa_movimentacao_id IS NULL
  AND (cp.desde IS NULL OR i.data_movimentacao >= cp.desde);

-- Depois a categoria nova, em todos os lugares que usavam a antiga.
UPDATE itens_extrato_bancario i SET categoria_id = cp.nova_categoria_id FROM categorias_pessoa cp WHERE i.categoria_id = cp.categoria_id;
UPDATE extrato_item_partes x SET categoria_id = cp.nova_categoria_id FROM categorias_pessoa cp WHERE x.categoria_id = cp.categoria_id;
UPDATE pessoas_movimentacao p SET categoria_id = cp.nova_categoria_id FROM categorias_pessoa cp WHERE p.categoria_id = cp.categoria_id;
UPDATE regras_classificacao r SET categoria_id = cp.nova_categoria_id FROM categorias_pessoa cp WHERE r.categoria_id = cp.categoria_id;
UPDATE categorias SET ativo = false WHERE id IN (SELECT categoria_id FROM categorias_pessoa);

-- ---------- 3) CONTAS COM PESSOA NO NOME ----------
UPDATE bancos b
SET nome_banco = coalesce(nullif(trim(cp.novo_nome), ''), b.nome_banco),
    compartilhado = b.compartilhado OR cp.pessoa_id IS NOT NULL
FROM contas_pessoa cp
WHERE b.id = cp.banco_id;

INSERT INTO bancos_pessoas (banco_id, pessoa_movimentacao_id)
SELECT banco_id, pessoa_id FROM contas_pessoa WHERE pessoa_id IS NOT NULL
ON CONFLICT DO NOTHING;

-- Depois: lançamentos e soma de valores têm que ser iguais aos de "antes".
SELECT 'depois' AS momento,
       (SELECT count(*) FROM itens_extrato_bancario) AS lancamentos,
       (SELECT sum(valor) FROM itens_extrato_bancario) AS soma_valores,
       (SELECT count(*) FROM pessoas_movimentacao) AS pessoas,
       (SELECT count(*) FROM regras_classificacao) AS regras;

COMMIT;
