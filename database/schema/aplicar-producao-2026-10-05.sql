-- =====================================================================================
-- [DB] P0 subprojeto 2 — aplicação em PRODUÇÃO (branch "development" do Neon)
-- Card: [DB] P0 — Schema e integridade (7) · checklist "Subprojeto 2 — Rebaseline", etapa 9
-- Testado no branch "dev" em 05/10/2026 (roteiro .claude/backlog/db-p0-sub2-roteiro-testes.md).
--
-- ANTES DE TUDO: criar no console do Neon um branch de restauração a partir de "development".
-- Rodar no SQL Editor do branch "development", uma parte por vez.
-- =====================================================================================


-- -------------------------------------------------------------------------------------
-- PARTE 1 — conferências (só leitura). TODAS as linhas precisam vir com 0.
-- Se alguma vier > 0, PARE: a parte 2 falharia no SET NOT NULL (e não mudaria nada).
-- -------------------------------------------------------------------------------------
SELECT 'Accounts.Value' AS coluna, count(*) AS nulos FROM "Accounts" WHERE "Value" IS NULL
UNION ALL SELECT 'Accounts.EhParcelado' AS coluna, count(*) AS nulos FROM "Accounts" WHERE "EhParcelado" IS NULL
UNION ALL SELECT 'Banco.SaldoInicial' AS coluna, count(*) AS nulos FROM "Banco" WHERE "SaldoInicial" IS NULL
UNION ALL SELECT 'Banco.TipoCartaoId' AS coluna, count(*) AS nulos FROM "Banco" WHERE "TipoCartaoId" IS NULL
UNION ALL SELECT 'Banco.UserId' AS coluna, count(*) AS nulos FROM "Banco" WHERE "UserId" IS NULL
UNION ALL SELECT 'Categories.NaturezaOperacao' AS coluna, count(*) AS nulos FROM "Categories" WHERE "NaturezaOperacao" IS NULL
UNION ALL SELECT 'ContaVencimento.UserId' AS coluna, count(*) AS nulos FROM "ContaVencimento" WHERE "UserId" IS NULL
UNION ALL SELECT 'ExtratoBancario.BancoId' AS coluna, count(*) AS nulos FROM "ExtratoBancario" WHERE "BancoId" IS NULL
UNION ALL SELECT 'ExtratoBancario.LoteImportacaoId' AS coluna, count(*) AS nulos FROM "ExtratoBancario" WHERE "LoteImportacaoId" IS NULL
UNION ALL SELECT 'ExtratoBancarioItem.UserId' AS coluna, count(*) AS nulos FROM "ExtratoBancarioItem" WHERE "UserId" IS NULL
UNION ALL SELECT 'Lista.Status' AS coluna, count(*) AS nulos FROM "Lista" WHERE "Status" IS NULL
UNION ALL SELECT 'Lista.TipoMovimentacao' AS coluna, count(*) AS nulos FROM "Lista" WHERE "TipoMovimentacao" IS NULL
UNION ALL SELECT 'MovimentacaoDiaria.TipoLancamento' AS coluna, count(*) AS nulos FROM "MovimentacaoDiaria" WHERE "TipoLancamento" IS NULL
UNION ALL SELECT 'PessoaMovimentacao.NomePessoa' AS coluna, count(*) AS nulos FROM "PessoaMovimentacao" WHERE "NomePessoa" IS NULL
UNION ALL SELECT 'PessoaMovimentacao.UserId' AS coluna, count(*) AS nulos FROM "PessoaMovimentacao" WHERE "UserId" IS NULL
UNION ALL SELECT 'TipoCartao.UserId' AS coluna, count(*) AS nulos FROM "TipoCartao" WHERE "UserId" IS NULL
UNION ALL SELECT 'TipoMovimentacao.Descricao' AS coluna, count(*) AS nulos FROM "TipoMovimentacao" WHERE "Descricao" IS NULL
UNION ALL SELECT 'TipoMovimentacao.UserId' AS coluna, count(*) AS nulos FROM "TipoMovimentacao" WHERE "UserId" IS NULL
UNION ALL SELECT 'TipoMovimentacao.ValorMeta' AS coluna, count(*) AS nulos FROM "TipoMovimentacao" WHERE "ValorMeta" IS NULL
UNION ALL SELECT 'orfaos Transactions.CategoryId', count(*) FROM "Transactions" x
 WHERE x."CategoryId" IS NOT NULL AND NOT EXISTS (SELECT 1 FROM "Categories" c WHERE c."Id" = x."CategoryId")
UNION ALL SELECT 'orfaos PessoaMovimentacao.CategoriaId', count(*) FROM "PessoaMovimentacao" x
 WHERE x."CategoriaId" IS NOT NULL AND NOT EXISTS (SELECT 1 FROM "Categories" c WHERE c."Id" = x."CategoriaId");


-- -------------------------------------------------------------------------------------
-- PARTE 2 — troca do histórico + AplicaDecisoes, numa transação só (tudo ou nada).
-- Idempotente: rodar de novo não faz nada.
-- -------------------------------------------------------------------------------------
START TRANSACTION;

DELETE FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250924002229_InitialCreate';
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20261005163305_Baseline', '8.0.8'
 WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005163305_Baseline');


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Banco" DROP CONSTRAINT banco_saldoinicial_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Accounts" ALTER COLUMN "Value" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Accounts" ALTER COLUMN "EhParcelado" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Banco" ALTER COLUMN "SaldoInicial" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Banco" ALTER COLUMN "TipoCartaoId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Banco" ALTER COLUMN "UserId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Categories" ALTER COLUMN "NaturezaOperacao" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ContaVencimento" ALTER COLUMN "UserId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancario" ALTER COLUMN "BancoId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancario" ALTER COLUMN "LoteImportacaoId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "UserId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Lista" ALTER COLUMN "Status" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Lista" ALTER COLUMN "TipoMovimentacao" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "MovimentacaoDiaria" ALTER COLUMN "TipoLancamento" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "PessoaMovimentacao" ALTER COLUMN "NomePessoa" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "PessoaMovimentacao" ALTER COLUMN "UserId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "TipoCartao" ALTER COLUMN "UserId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "TipoMovimentacao" ALTER COLUMN "Descricao" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "TipoMovimentacao" ALTER COLUMN "UserId" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "TipoMovimentacao" ALTER COLUMN "ValorMeta" SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Accounts" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Banco" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Categories" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ContaVencimento" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancario" ALTER COLUMN "DataImportacao" TYPE timestamp with time zone USING "DataImportacao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ItemLista" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Lista" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "PessoaMovimentacao" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "TipoMovimentacao" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Transactions" ALTER COLUMN "DataAlteracao" TYPE timestamp with time zone USING "DataAlteracao" AT TIME ZONE 'UTC';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "PessoaMovimentacao" ALTER COLUMN "Id" SET GENERATED BY DEFAULT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Banco" DROP CONSTRAINT fk_banco_tipocartao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Banco" ADD CONSTRAINT fk_banco_tipocartao FOREIGN KEY ("TipoCartaoId") REFERENCES "TipoCartao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ContaVencimento" DROP CONSTRAINT "ContaVencimento_ContaId_fkey";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ContaVencimento" ADD CONSTRAINT "ContaVencimento_ContaId_fkey" FOREIGN KEY ("ContaId") REFERENCES "Accounts" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ContaVencimento" DROP CONSTRAINT contavencimento_user_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ContaVencimento" ADD CONSTRAINT contavencimento_user_fkey FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancario" DROP CONSTRAINT extratobancario_bancoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancario" ADD CONSTRAINT extratobancario_bancoid_fkey FOREIGN KEY ("BancoId") REFERENCES "Banco" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT "FK_ExtratoBancarioItem_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT "FK_ExtratoBancarioItem_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_bancoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_bancoid_fkey FOREIGN KEY ("BancoId") REFERENCES "Banco" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_tipocartaoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_tipocartaoid_fkey FOREIGN KEY ("TipoCartaoId") REFERENCES "TipoCartao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_tipomovimentacaoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_tipomovimentacaoid_fkey FOREIGN KEY ("TipoMovimentacaoId") REFERENCES "TipoMovimentacao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ItemLista" DROP CONSTRAINT "FK_ItemLista_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "ItemLista" ADD CONSTRAINT "FK_ItemLista_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Lista" DROP CONSTRAINT "FK_Lista_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Lista" ADD CONSTRAINT "FK_Lista_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "MovimentacaoDiaria" DROP CONSTRAINT movimentacaodiaria_bancoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "MovimentacaoDiaria" ADD CONSTRAINT movimentacaodiaria_bancoid_fkey FOREIGN KEY ("BancoId") REFERENCES "Banco" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "MovimentacaoDiaria" DROP CONSTRAINT movimentacaodiaria_tipocartaoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "MovimentacaoDiaria" ADD CONSTRAINT movimentacaodiaria_tipocartaoid_fkey FOREIGN KEY ("TipoCartaoId") REFERENCES "TipoCartao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "MovimentacaoDiaria" DROP CONSTRAINT movimentacaodiaria_tipomovimentacaoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "MovimentacaoDiaria" ADD CONSTRAINT movimentacaodiaria_tipomovimentacaoid_fkey FOREIGN KEY ("TipoMovimentacaoId") REFERENCES "TipoMovimentacao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "PessoaMovimentacao" DROP CONSTRAINT constraint_1;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "PessoaMovimentacao" ADD CONSTRAINT constraint_1 FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "PessoaMovimentacao" DROP CONSTRAINT constraint_2;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "PessoaMovimentacao" ADD CONSTRAINT constraint_2 FOREIGN KEY ("TipoMovimentacaoId") REFERENCES "TipoMovimentacao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "TipoCartao" DROP CONSTRAINT fk_tipocartao_aspnetusers;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "TipoCartao" ADD CONSTRAINT fk_tipocartao_aspnetusers FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "TipoMovimentacao" DROP CONSTRAINT fk_tipomovimentacao_aspnetusers;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "TipoMovimentacao" ADD CONSTRAINT fk_tipomovimentacao_aspnetusers FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Transactions" DROP CONSTRAINT "FK_Transactions_Accounts_IdAccount";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT "FK_Transactions_Accounts_IdAccount" FOREIGN KEY ("IdAccount") REFERENCES "Accounts" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    UPDATE "Transactions" t SET "CategoryId" = NULL WHERE t."CategoryId" IS NOT NULL AND NOT EXISTS (SELECT 1 FROM "Categories" c WHERE c."Id" = t."CategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT "FK_Transactions_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    UPDATE "PessoaMovimentacao" t SET "CategoriaId" = NULL WHERE t."CategoriaId" IS NOT NULL AND NOT EXISTS (SELECT 1 FROM "Categories" c WHERE c."Id" = t."CategoriaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    ALTER TABLE "PessoaMovimentacao" ADD CONSTRAINT "FK_PessoaMovimentacao_Categories_CategoriaId" FOREIGN KEY ("CategoriaId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_Banco_TipoCartaoId" ON "Banco" ("TipoCartaoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_Banco_UserId" ON "Banco" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ContaVencimento_ContaId" ON "ContaVencimento" ("ContaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ContaVencimento_UserId" ON "ContaVencimento" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ExtratoBancario_BancoId" ON "ExtratoBancario" ("BancoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ExtratoBancario_UserId" ON "ExtratoBancario" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ExtratoBancarioItem_BancoId" ON "ExtratoBancarioItem" ("BancoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ExtratoBancarioItem_CategoriaId" ON "ExtratoBancarioItem" ("CategoriaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ExtratoBancarioItem_PessoaMovimentacaoId" ON "ExtratoBancarioItem" ("PessoaMovimentacaoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ExtratoBancarioItem_TipoCartaoId" ON "ExtratoBancarioItem" ("TipoCartaoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ExtratoBancarioItem_TipoMovimentacaoId" ON "ExtratoBancarioItem" ("TipoMovimentacaoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ExtratoBancarioItem_UserId" ON "ExtratoBancarioItem" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ItemLista_ListaId" ON "ItemLista" ("ListaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_ItemLista_UserId" ON "ItemLista" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_Lista_UserId" ON "Lista" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_MovimentacaoDiaria_BancoId" ON "MovimentacaoDiaria" ("BancoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_MovimentacaoDiaria_TipoCartaoId" ON "MovimentacaoDiaria" ("TipoCartaoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_MovimentacaoDiaria_TipoMovimentacaoId" ON "MovimentacaoDiaria" ("TipoMovimentacaoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_MovimentacaoDiaria_UserId" ON "MovimentacaoDiaria" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_PessoaMovimentacao_CategoriaId" ON "PessoaMovimentacao" ("CategoriaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_PessoaMovimentacao_TipoMovimentacaoId" ON "PessoaMovimentacao" ("TipoMovimentacaoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_PessoaMovimentacao_UserId" ON "PessoaMovimentacao" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_TipoCartao_UserId" ON "TipoCartao" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_TipoMovimentacao_UserId" ON "TipoMovimentacao" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    CREATE INDEX "IX_Transactions_CategoryId" ON "Transactions" ("CategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261005172650_AplicaDecisoes') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261005172650_AplicaDecisoes', '8.0.8');
    END IF;
END $EF$;
COMMIT;



-- -------------------------------------------------------------------------------------
-- PARTE 3 — conferência: esperado 2 linhas (Baseline e AplicaDecisoes) e 37 FKs.
-- Para a conferência completa, rodar a "consulta de divergências" do roteiro (esperado: 0 linhas).
-- -------------------------------------------------------------------------------------
SELECT "MigrationId" FROM "__EFMigrationsHistory" ORDER BY 1;
SELECT count(*) AS fks FROM pg_constraint WHERE contype = 'f' AND connamespace = 'public'::regnamespace;