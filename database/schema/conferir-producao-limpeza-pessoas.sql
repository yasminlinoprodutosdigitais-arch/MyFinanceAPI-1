-- =====================================================================================
-- Limpeza de pessoas — CONFERÊNCIA (só leitura). Card #77, entrega 3.
-- Rode em PRODUÇÃO (branch "development") e anote os ids para o aplicar-producao-limpeza-pessoas.sql.
-- Nada aqui altera dados.
-- =====================================================================================

-- 1) "Pessoas" que parecem lojas: não são o titular ("Eu") e não entram nos acertos.
--    Anote os ids das que forem lojas/estabelecimentos (lista "lojas" do script de aplicação).
SELECT p.usuario_id,
       p.id                                    AS pessoa_id,
       p.nome_pessoa,
       c.nome                                  AS categoria_padrao,
       (SELECT count(*) FROM itens_extrato_bancario i WHERE i.pessoa_movimentacao_id = p.id) AS lancamentos,
       (SELECT count(*) FROM extrato_item_partes x WHERE x.pessoa_movimentacao_id = p.id)   AS partes_de_divisao,
       (SELECT count(*) FROM regras_classificacao r WHERE r.pessoa_movimentacao_id = p.id)  AS regras_que_ligam
FROM pessoas_movimentacao p
LEFT JOIN categorias c ON c.id = p.categoria_id
WHERE NOT p.eh_titular AND NOT p.entra_nos_acertos
ORDER BY p.usuario_id, lancamentos DESC, p.nome_pessoa;

-- Para comparar: quem entra nos acertos (pessoas de verdade). Não entram na lista de lojas.
SELECT p.usuario_id, p.id AS pessoa_id, p.nome_pessoa,
       (SELECT count(*) FROM itens_extrato_bancario i WHERE i.pessoa_movimentacao_id = p.id) AS lancamentos
FROM pessoas_movimentacao p
WHERE p.entra_nos_acertos AND NOT p.eh_titular
ORDER BY p.usuario_id, p.nome_pessoa;

-- 2) Categorias com nome de pessoa (nome ou subcategoria igual ao de uma pessoa cadastrada, sem acento).
--    Para cada uma, decida: qual pessoa ela representa, qual categoria fica no lugar (ou nenhuma) e,
--    se quiser, a partir de que data os lançamentos passam a ser dessa pessoa (lista "categorias_pessoa").
WITH pessoas AS (
    SELECT usuario_id, id, nome_pessoa,
           translate(upper(trim(nome_pessoa)), 'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇ', 'AAAAAEEEEIIIIOOOOOUUUUC') AS chave
    FROM pessoas_movimentacao
    WHERE NOT eh_titular
)
SELECT c.usuario_id,
       c.id            AS categoria_id,
       c.nome,
       c.subcategoria,
       c.ativo,
       p.id            AS pessoa_id_sugerida,
       p.nome_pessoa,
       (SELECT count(*) FROM itens_extrato_bancario i WHERE i.categoria_id = c.id)                                   AS lancamentos,
       (SELECT count(*) FROM itens_extrato_bancario i WHERE i.categoria_id = c.id AND i.pessoa_movimentacao_id IS NULL) AS lancamentos_sem_pessoa,
       (SELECT min(i.data_movimentacao) FROM itens_extrato_bancario i WHERE i.categoria_id = c.id)                   AS primeiro,
       (SELECT max(i.data_movimentacao) FROM itens_extrato_bancario i WHERE i.categoria_id = c.id)                   AS ultimo
FROM categorias c
JOIN pessoas p
  ON p.usuario_id = c.usuario_id
 AND (p.chave = translate(upper(trim(c.nome)), 'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇ', 'AAAAAEEEEIIIIOOOOOUUUUC')
   OR p.chave = translate(upper(trim(coalesce(c.subcategoria, ''))), 'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇ', 'AAAAAEEEEIIIIOOOOOUUUUC'))
ORDER BY c.usuario_id, c.nome;

-- 3) Contas com nome de pessoa (o nome do banco contém o nome de uma pessoa cadastrada com 3+ letras).
--    Decida o nome novo e se a conta vira compartilhada com essa pessoa (lista "contas_pessoa").
WITH pessoas AS (
    SELECT usuario_id, id, nome_pessoa,
           translate(upper(trim(nome_pessoa)), 'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇ', 'AAAAAEEEEIIIIOOOOOUUUUC') AS chave
    FROM pessoas_movimentacao
    WHERE NOT eh_titular AND length(trim(nome_pessoa)) >= 3
)
SELECT b.usuario_id, b.id AS banco_id, b.nome_banco, b.compartilhado, p.id AS pessoa_id_sugerida, p.nome_pessoa
FROM bancos b
JOIN pessoas p
  ON p.usuario_id = b.usuario_id
 AND translate(upper(b.nome_banco), 'ÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇ', 'AAAAAEEEEIIIIOOOOOUUUUC') LIKE '%' || p.chave || '%'
ORDER BY b.usuario_id, b.nome_banco;
