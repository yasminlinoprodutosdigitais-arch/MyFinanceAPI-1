-- =====================================================================================
-- [DB] P0 subprojeto 3 — Integridade — aplicação em PRODUÇÃO (branch "development" do Neon)
-- Card: [DB] P0 — Schema e integridade (7) · checklist "Subprojeto 3 — Integridade", T11
-- Testado no branch "dev" em 06/10/2026 (roteiro .claude/backlog/db-p0-sub3-roteiro-testes.md).
--
-- PRÉ-REQUISITO: aplicar-producao-2026-10-05.sql já aplicado (Baseline + AplicaDecisoes
-- registradas na __EFMigrationsHistory). Este script NÃO roda sem elas.
-- ANTES DE TUDO: branch de restauração no Neon a partir de "development" — a limpeza de
-- pessoas (fusão de duplicadas) não tem volta.
-- Rodar no SQL Editor do branch "development", uma parte por vez.
-- =====================================================================================


-- -------------------------------------------------------------------------------------
-- PARTE 1 — conferências (só leitura). As duas primeiras linhas precisam vir com 0;
-- anote o total de itens para comparar na parte 3.
-- -------------------------------------------------------------------------------------
SELECT 'AplicaDecisoes registrada (precisa ser 1)', count(*) FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes'
UNION ALL SELECT 'orfaos ExtratoBancario.TipoCartaoId', count(*) FROM "ExtratoBancario" e
 WHERE e."TipoCartaoId" IS NOT NULL AND NOT EXISTS (SELECT 1 FROM "TipoCartao" t WHERE t."Id" = e."TipoCartaoId")
UNION ALL SELECT 'colisoes ChaveImportacao', count(*) FROM (
 SELECT 1 FROM "ExtratoBancarioItem" WHERE "Identificador" IS NOT NULL AND "ExtratoBancarioId" IS NOT NULL
 GROUP BY "UserId","BancoId", trim("Identificador"), "DataMovimentacao", "TipoLancamento", "Valor", coalesce(trim("Descricao"),'')
 HAVING count(*) > 1) g
UNION ALL SELECT 'total itens', count(*) FROM "ExtratoBancarioItem";


-- -------------------------------------------------------------------------------------
-- PARTE 2 — migration Integridade (transação única, idempotente).
-- -------------------------------------------------------------------------------------
START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    ALTER TABLE "Accounts" DROP CONSTRAINT "FK_Accounts_Categories_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    ALTER TABLE "PessoaMovimentacao" DROP CONSTRAINT constraint_1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    ALTER TABLE "PessoaMovimentacao" DROP CONSTRAINT constraint_2;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    DROP INDEX "IX_TipoCartao_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    DROP INDEX "IX_PessoaMovimentacao_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    DROP INDEX "IX_Categories_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    DROP INDEX "IX_Banco_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD "ChaveImportacao" character varying(64);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN

    UPDATE "ExtratoBancarioItem" SET "PessoaMovimentacaoId" = NULL
     WHERE "PessoaMovimentacaoId" IN (SELECT "Id" FROM "PessoaMovimentacao" WHERE upper(trim("NomePessoa")) = 'PESSOA SEM NOME');
    DELETE FROM "PessoaMovimentacao" WHERE upper(trim("NomePessoa")) = 'PESSOA SEM NOME';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN

    UPDATE "PessoaMovimentacao" k
       SET "CategoriaId" = c."CategoriaId", "TipoMovimentacaoId" = c."TipoMovimentacaoId"
      FROM (SELECT DISTINCT ON ("UserId", upper(trim("NomePessoa")))
                   "UserId", upper(trim("NomePessoa")) AS n, "CategoriaId", "TipoMovimentacaoId"
              FROM "PessoaMovimentacao"
             WHERE "CategoriaId" IS NOT NULL OR "TipoMovimentacaoId" IS NOT NULL
             ORDER BY "UserId", upper(trim("NomePessoa")), "Id" DESC) c,
           (SELECT "UserId", upper(trim("NomePessoa")) AS n, min("Id") AS keep
              FROM "PessoaMovimentacao" GROUP BY 1, 2 HAVING count(*) > 1) g
     WHERE k."Id" = g.keep AND c."UserId" = g."UserId" AND c.n = g.n;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN

    UPDATE "ExtratoBancarioItem" i SET "PessoaMovimentacaoId" = m.keep
      FROM (SELECT "Id", min("Id") OVER (PARTITION BY "UserId", upper(trim("NomePessoa"))) AS keep
              FROM "PessoaMovimentacao") m
     WHERE i."PessoaMovimentacaoId" = m."Id" AND m."Id" <> m.keep;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN

    DELETE FROM "PessoaMovimentacao" p
     USING (SELECT "Id", min("Id") OVER (PARTITION BY "UserId", upper(trim("NomePessoa"))) AS keep
              FROM "PessoaMovimentacao") m
     WHERE p."Id" = m."Id" AND m."Id" <> m.keep;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN

    UPDATE "PessoaMovimentacao" SET "NomePessoa" = upper(trim("NomePessoa"))
     WHERE "NomePessoa" <> upper(trim("NomePessoa"));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN

    UPDATE "ExtratoBancarioItem" SET "ChaveImportacao" = encode(sha256(convert_to(
             trim("Identificador") || '|' ||
             to_char("DataMovimentacao", 'YYYY-MM-DD') || '|' ||
             to_char(CASE WHEN "TipoLancamento" = 'Saída' THEN -"Valor" ELSE "Valor" END, 'FM9999999990.00') || '|' ||
             coalesce(trim("Descricao"), ''), 'UTF8')), 'hex')
     WHERE "Identificador" IS NOT NULL AND "ExtratoBancarioId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    CREATE UNIQUE INDEX "UX_TipoCartao_UserId_NomeTipoCartao" ON "TipoCartao" ("UserId", "NomeTipoCartao");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    CREATE UNIQUE INDEX "UX_PessoaMovimentacao_UserId_NomePessoa" ON "PessoaMovimentacao" ("UserId", "NomePessoa");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    CREATE UNIQUE INDEX "UX_ExtratoBancarioItem_ChaveImportacao" ON "ExtratoBancarioItem" ("UserId", "BancoId", "ChaveImportacao") WHERE "ChaveImportacao" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    CREATE INDEX "IX_ExtratoBancario_TipoCartaoId" ON "ExtratoBancario" ("TipoCartaoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    CREATE UNIQUE INDEX "UX_Categories_UserId_Name_SubCategory" ON "Categories" ("UserId", "Name", "SubCategory");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    CREATE UNIQUE INDEX "UX_Banco_UserId_NomeBanco" ON "Banco" ("UserId", "NomeBanco");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    ALTER TABLE "Accounts" ADD CONSTRAINT "FK_Accounts_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    ALTER TABLE "ExtratoBancario" ADD CONSTRAINT "FK_ExtratoBancario_TipoCartao_TipoCartaoId" FOREIGN KEY ("TipoCartaoId") REFERENCES "TipoCartao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    ALTER TABLE "PessoaMovimentacao" ADD CONSTRAINT "FK_PessoaMovimentacao_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    ALTER TABLE "PessoaMovimentacao" ADD CONSTRAINT "FK_PessoaMovimentacao_TipoMovimentacao_TipoMovimentacaoId" FOREIGN KEY ("TipoMovimentacaoId") REFERENCES "TipoMovimentacao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261006015603_Integridade') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261006015603_Integridade', '8.0.8');
    END IF;
END $EF$;
COMMIT;



-- -------------------------------------------------------------------------------------
-- PARTE 3 — conferência: todas as linhas com 0, exceto as indicadas.
-- -------------------------------------------------------------------------------------
SELECT 'pessoas duplicadas' AS item, count(*) AS atual FROM (SELECT 1 FROM "PessoaMovimentacao" GROUP BY "UserId","NomePessoa" HAVING count(*) > 1) g
UNION ALL SELECT 'pessoa sem nome', count(*) FROM "PessoaMovimentacao" WHERE "NomePessoa" = 'PESSOA SEM NOME'
UNION ALL SELECT 'nomes nao normalizados', count(*) FROM "PessoaMovimentacao" WHERE "NomePessoa" <> upper(trim("NomePessoa"))
UNION ALL SELECT 'importados sem chave', count(*) FROM "ExtratoBancarioItem" WHERE "Identificador" IS NOT NULL AND "ExtratoBancarioId" IS NOT NULL AND "ChaveImportacao" IS NULL
UNION ALL SELECT 'itens apontando p/ pessoa inexistente', count(*) FROM "ExtratoBancarioItem" i WHERE i."PessoaMovimentacaoId" IS NOT NULL AND NOT EXISTS (SELECT 1 FROM "PessoaMovimentacao" p WHERE p."Id" = i."PessoaMovimentacaoId")
UNION ALL SELECT 'constraint_1/2 restantes', count(*) FROM pg_constraint WHERE conname IN ('constraint_1','constraint_2')
UNION ALL SELECT 'total itens (= parte 1)', count(*) FROM "ExtratoBancarioItem"
UNION ALL SELECT 'uniques UX_* (= 5)', count(*) FROM pg_indexes WHERE schemaname = 'public' AND indexname LIKE 'UX\_%'
UNION ALL SELECT 'migrations registradas (= 3)', count(*) FROM "__EFMigrationsHistory";