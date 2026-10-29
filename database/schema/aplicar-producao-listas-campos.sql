-- =====================================================================================
-- Listas com campos configuráveis + arquivar — PRODUÇÃO (branch "development")
-- Card: Listas — modelo com campos configuráveis + arquivar (#67) · checklist "Plano — Parte 1", item 3
-- Testado no branch "dev" em 28/10/2026 (migration 20261028233634_ListasCamposConfiguraveis): 13 listas e
-- 194 itens com a mesma contagem, soma e feitos; "Casamento" arquivada; 16 itens AGUARDANDO viraram
-- pendentes com "(aguardando)" na observação. Ida e volta (Up + Down) devolveu o banco idêntico.
--
-- O que muda:
--  * listas: tipo_lista e ativo saem; entram icone, usa_quantidade, usa_valor, modo_valor, usa_limite, limite,
--    usa_data, usa_observacao, modelo e arquivada_em. Orçamento → valor (TOTAL) + observação; Checklist →
--    quantidade; Cronograma → data; inativa → arquivada_em = agora.
--  * itens_lista: situacao sai; entram feito, feito_em e ordem; data_tarefa vira data. OK → feito.
--  * A conversão confere, por lista, itens, soma do valor e feitos; se algo não bater, o script inteiro
--    é desfeito (RAISE EXCEPTION dentro da transação).
--
-- PRÉ-REQUISITO: aplicar-producao-conta-valor-fixo.sql já aplicado (o histórico de migrations precisa
-- ter 20261027171229_ContaValorFixo). Publicar a API e o front desta mudança JUNTOS, e só com os cards
-- de front #63 (Cadastro de Listas) e #66 (Tela interna da lista) prontos: a API antiga e as telas
-- antigas de listas não funcionam com o modelo novo.
-- ANTES DE TUDO: branch de restauração no Neon. Rodar o Passo 0 e guardar o resultado.
--
-- PASSO 0 — CONFERÊNCIA (só leitura). Guardar para comparar depois:
-- SELECT l.id, l.nome, l.tipo_lista, l.ativo, count(i.id) AS itens,
--        count(*) FILTER (WHERE i.situacao = 'OK') AS ok,
--        count(*) FILTER (WHERE i.situacao = 'AGUARDANDO') AS aguardando,
--        sum(i.valor) AS soma
-- FROM listas l LEFT JOIN itens_lista i ON i.lista_id = l.id GROUP BY l.id ORDER BY l.id;
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
--
-- PASSO 2 — CONFERÊNCIA (só leitura): itens e soma iguais ao Passo 0; feitos = ok; aguardando aparece
-- na observação; arquivada = as que tinham ativo = false.
-- SELECT l.id, l.nome, l.modelo, l.arquivada_em IS NOT NULL AS arquivada, count(i.id) AS itens,
--        count(*) FILTER (WHERE i.feito) AS feitos,
--        count(*) FILTER (WHERE i.observacao LIKE '%aguardando%') AS aguardando,
--        sum(i.valor) AS soma
-- FROM listas l LEFT JOIN itens_lista i ON i.lista_id = l.id GROUP BY l.id ORDER BY l.id;
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD arquivada_em timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD icone character varying(40);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD limite numeric(10,2);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD modelo character varying(20) NOT NULL DEFAULT 'BRANCO';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD modo_valor character varying(10) NOT NULL DEFAULT 'TOTAL';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD usa_data boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD usa_limite boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD usa_observacao boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD usa_quantidade boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD usa_valor boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE itens_lista ADD feito boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE itens_lista ADD feito_em timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE itens_lista ADD ordem integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN

    CREATE TEMP TABLE conferencia_listas ON COMMIT DROP AS
    SELECT lista_id, count(*) AS itens, coalesce(sum(valor), 0) AS soma,
           count(*) FILTER (WHERE situacao = 'OK') AS feitos,
           count(*) FILTER (WHERE situacao = 'AGUARDANDO') AS aguardando
    FROM itens_lista GROUP BY lista_id;

    UPDATE listas SET
        usa_valor      = tipo_lista = 3,
        usa_observacao = tipo_lista = 3,
        usa_quantidade = tipo_lista = 1,
        usa_data       = tipo_lista = 2,
        modo_valor     = 'TOTAL',
        modelo         = CASE tipo_lista WHEN 1 THEN 'COMPRAS' WHEN 2 THEN 'AGENDA' ELSE 'ORCAMENTO' END,
        icone          = CASE tipo_lista WHEN 1 THEN 'pi pi-shopping-cart' WHEN 2 THEN 'pi pi-calendar' ELSE 'pi pi-wallet' END,
        arquivada_em   = CASE WHEN ativo THEN NULL ELSE now() END;

    UPDATE itens_lista SET
        feito      = situacao = 'OK',
        feito_em   = CASE WHEN situacao = 'OK' THEN data_alteracao END,
        observacao = CASE
            WHEN situacao <> 'AGUARDANDO' OR situacao IS NULL THEN observacao
            WHEN observacao IS NULL OR btrim(observacao) = '' THEN 'aguardando'
            ELSE left(btrim(observacao), 487) || ' (aguardando)'
        END;

    DO $$
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM conferencia_listas c
            FULL JOIN (
                SELECT lista_id, count(*) AS itens, coalesce(sum(valor), 0) AS soma,
                       count(*) FILTER (WHERE feito) AS feitos,
                       count(*) FILTER (WHERE situacao = 'AGUARDANDO' AND observacao LIKE '%aguardando%') AS aguardando
                FROM itens_lista GROUP BY lista_id
            ) d USING (lista_id)
            WHERE c.itens IS DISTINCT FROM d.itens
               OR c.soma IS DISTINCT FROM d.soma
               OR c.feitos IS DISTINCT FROM d.feitos
               OR c.aguardando IS DISTINCT FROM d.aguardando
        ) THEN
            RAISE EXCEPTION 'Conferência das listas falhou: itens, soma ou feitos mudaram na conversão.';
        END IF;
    END $$;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas DROP CONSTRAINT listas_tipo_lista_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_quantidade_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_situacao_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas DROP COLUMN ativo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas DROP COLUMN tipo_lista;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE itens_lista DROP COLUMN situacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE itens_lista RENAME COLUMN data_tarefa TO data;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_limite_check CHECK (limite >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_limite_com_valor_check CHECK (NOT usa_limite OR usa_valor);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_modelo_check CHECK (modelo IN ('COMPRAS', 'ORCAMENTO', 'TAREFAS', 'AGENDA', 'BRANCO'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_modo_valor_check CHECK (modo_valor IN ('UNITARIO', 'TOTAL'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_quantidade_check CHECK (quantidade >= 1);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_valor_check CHECK (valor >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261028233634_ListasCamposConfiguraveis') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261028233634_ListasCamposConfiguraveis', '8.0.8');
    END IF;
END $EF$;
COMMIT;

