-- =====================================================================================
-- [DB] P1 subprojeto 3 — Cartão de crédito e tipos padrão do sistema — PRODUÇÃO (branch "development")
-- Card: [DB] P1 — Modelagem (13) · checklist "Plano — Subprojeto 3 (Cartão de crédito)"
-- Testado no branch "dev" em 13/10/2026 (migrations 20261013174830_TipoCartaoEhCredito e
-- 20261013181037_TipoCartaoPadraoSistema).
--
-- PRÉ-REQUISITOS, nesta ordem: aplicar-producao-2026-10-05.sql, aplicar-producao-integridade.sql,
-- aplicar-producao-dominios.sql, aplicar-producao-datas.sql.
-- Publicar a API da branch db-p1-modelagem JUNTO: depois deste script a API antiga quebra ao ler
-- tipos de cartão (UserId passa a ser nulo nos padrão do sistema).
-- ANTES DE TUDO: branch de restauração no Neon. As cópias dos tipos padrão de cada usuário são apagadas.
--
-- PASSO 0 — CONFERÊNCIA (só leitura). Mostra o que vai acontecer com cada tipo: o de menor Id de
-- cada nome padrão fica (vira do sistema); os demais com o mesmo nome têm as referências movidas e
-- são apagados. Tipos com outros nomes não mudam.
-- SELECT t."Id", t."UserId", t."NomeTipoCartao",
--        CASE WHEN t."Id" = min(t."Id") OVER (PARTITION BY lower(trim(t."NomeTipoCartao"))) THEN 'FICA (sistema)' ELSE 'APAGADO (referências movidas)' END AS destino,
--        (SELECT count(*) FROM "Banco" b WHERE b."TipoCartaoId" = t."Id") AS bancos,
--        (SELECT count(*) FROM "ExtratoBancarioItem" i WHERE i."TipoCartaoId" = t."Id") AS itens
-- FROM "TipoCartao" t
-- WHERE lower(trim(t."NomeTipoCartao")) IN ('cartão de crédito', 'conta corrente', 'conta poupança')
-- ORDER BY lower(trim(t."NomeTipoCartao")), t."Id";
--
-- PASSO 1 — o script abaixo (gerado por dotnet ef migrations script --idempotent).
-- =====================================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013174830_TipoCartaoEhCredito') THEN
    ALTER TABLE "TipoCartao" ADD "EhCredito" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013174830_TipoCartaoEhCredito') THEN
    UPDATE "TipoCartao" SET "EhCredito" = true WHERE "NomeTipoCartao" ILIKE '%crédito%' OR "NomeTipoCartao" ILIKE '%credito%';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013174830_TipoCartaoEhCredito') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261013174830_TipoCartaoEhCredito', '8.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN
    ALTER TABLE "TipoCartao" ALTER COLUMN "UserId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN

    INSERT INTO "TipoCartao" ("NomeTipoCartao", "EhCredito", "UserId")
    SELECT p.nome, p.credito, NULL
    FROM (VALUES ('Cartão de crédito', true), ('Conta corrente', false), ('Conta Poupança', false)) AS p(nome, credito)
    WHERE NOT EXISTS (SELECT 1 FROM "TipoCartao" t WHERE lower(trim(t."NomeTipoCartao")) = lower(p.nome));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN

    WITH padrao(nome) AS (VALUES ('Cartão de crédito'), ('Conta corrente'), ('Conta Poupança')),
         sistema AS (SELECT p.nome, min(t."Id") AS id
                     FROM padrao p JOIN "TipoCartao" t ON lower(trim(t."NomeTipoCartao")) = lower(p.nome)
                     GROUP BY p.nome),
         copia AS (SELECT t."Id" AS antigo, s.id AS novo
                   FROM "TipoCartao" t JOIN sistema s ON lower(trim(t."NomeTipoCartao")) = lower(s.nome)
                   WHERE t."Id" <> s.id)
    UPDATE "Banco" x SET "TipoCartaoId" = c.novo FROM copia c WHERE x."TipoCartaoId" = c.antigo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN

    WITH padrao(nome) AS (VALUES ('Cartão de crédito'), ('Conta corrente'), ('Conta Poupança')),
         sistema AS (SELECT p.nome, min(t."Id") AS id
                     FROM padrao p JOIN "TipoCartao" t ON lower(trim(t."NomeTipoCartao")) = lower(p.nome)
                     GROUP BY p.nome),
         copia AS (SELECT t."Id" AS antigo, s.id AS novo
                   FROM "TipoCartao" t JOIN sistema s ON lower(trim(t."NomeTipoCartao")) = lower(s.nome)
                   WHERE t."Id" <> s.id)
    UPDATE "ExtratoBancario" x SET "TipoCartaoId" = c.novo FROM copia c WHERE x."TipoCartaoId" = c.antigo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN

    WITH padrao(nome) AS (VALUES ('Cartão de crédito'), ('Conta corrente'), ('Conta Poupança')),
         sistema AS (SELECT p.nome, min(t."Id") AS id
                     FROM padrao p JOIN "TipoCartao" t ON lower(trim(t."NomeTipoCartao")) = lower(p.nome)
                     GROUP BY p.nome),
         copia AS (SELECT t."Id" AS antigo, s.id AS novo
                   FROM "TipoCartao" t JOIN sistema s ON lower(trim(t."NomeTipoCartao")) = lower(s.nome)
                   WHERE t."Id" <> s.id)
    UPDATE "ExtratoBancarioItem" x SET "TipoCartaoId" = c.novo FROM copia c WHERE x."TipoCartaoId" = c.antigo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN

    WITH padrao(nome) AS (VALUES ('Cartão de crédito'), ('Conta corrente'), ('Conta Poupança')),
         sistema AS (SELECT p.nome, min(t."Id") AS id
                     FROM padrao p JOIN "TipoCartao" t ON lower(trim(t."NomeTipoCartao")) = lower(p.nome)
                     GROUP BY p.nome),
         copia AS (SELECT t."Id" AS antigo, s.id AS novo
                   FROM "TipoCartao" t JOIN sistema s ON lower(trim(t."NomeTipoCartao")) = lower(s.nome)
                   WHERE t."Id" <> s.id)
    UPDATE "MovimentacaoDiaria" x SET "TipoCartaoId" = c.novo FROM copia c WHERE x."TipoCartaoId" = c.antigo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN

    WITH padrao(nome) AS (VALUES ('Cartão de crédito'), ('Conta corrente'), ('Conta Poupança')),
         sistema AS (SELECT p.nome, min(t."Id") AS id
                     FROM padrao p JOIN "TipoCartao" t ON lower(trim(t."NomeTipoCartao")) = lower(p.nome)
                     GROUP BY p.nome),
         copia AS (SELECT t."Id" AS antigo, s.id AS novo
                   FROM "TipoCartao" t JOIN sistema s ON lower(trim(t."NomeTipoCartao")) = lower(s.nome)
                   WHERE t."Id" <> s.id)
    DELETE FROM "TipoCartao" t USING copia c WHERE t."Id" = c.antigo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN

    UPDATE "TipoCartao" t
    SET "UserId" = NULL, "NomeTipoCartao" = p.nome, "EhCredito" = p.credito
    FROM (VALUES ('Cartão de crédito', true), ('Conta corrente', false), ('Conta Poupança', false)) AS p(nome, credito)
    WHERE lower(trim(t."NomeTipoCartao")) = lower(p.nome);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN
    CREATE UNIQUE INDEX "UX_TipoCartao_Sistema_NomeTipoCartao" ON "TipoCartao" ("NomeTipoCartao") WHERE "UserId" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261013181037_TipoCartaoPadraoSistema') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261013181037_TipoCartaoPadraoSistema', '8.0.8');
    END IF;
END $EF$;
COMMIT;

