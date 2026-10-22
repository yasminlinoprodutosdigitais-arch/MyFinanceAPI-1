-- [DB] P3 — Convenções: LimitesDeTexto, ConvencoesDeNomes, AjusteLimitesDeTexto, ConvencoesDaSpec e NomesDeForeignKeys,
-- a partir de 20261014004442_IndicesConsulta (último aplicado em produção). Gerado por dotnet ef migrations script --idempotent;
-- as 5 transações do EF foram unidas numa só: aplica tudo ou nada. Rollback: reverter-producao-convencoes.sql.
-- Antes: branch de restauração no Neon. A trava DB-P3-04 aborta (sem truncar) se algum texto passar do limite.
START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN

                    DO $$
                    DECLARE
                        r record;
                        maior int;
                        excedentes text := '';
                    BEGIN
                        FOR r IN
                            SELECT * FROM (VALUES
                            ('Accounts', 'DataOperacao', 10),
                            ('Accounts', 'Name', 120),
                            ('AspNetUsers', 'NomeUsuario', 100),
                            ('AspNetUsers', 'Role', 30),
                            ('Banco', 'Agencia', 20),
                            ('Banco', 'NomeBanco', 80),
                            ('Banco', 'NumeroConta', 30),
                            ('Categories', 'Name', 80),
                            ('Categories', 'SubCategory', 80),
                            ('ExtratoBancario', 'NomeArquivo', 255),
                            ('ExtratoBancario', 'NomeArquivoOrigem', 255),
                            ('ExtratoBancarioItem', 'Descricao', 300),
                            ('ExtratoBancarioItem', 'Identificador', 100),
                            ('ExtratoBancarioItem', 'NomePessoaTransacao', 120),
                            ('ExtratoBancarioItem', 'Observacao', 500),
                            ('ExtratoBancarioItem', 'TipoLancamento', 20),
                            ('ItemLista', 'Observacao', 500),
                            ('ItemLista', 'Status', 20),
                            ('Lista', 'Nome', 100),
                            ('MovimentacaoDiaria', 'Descricao', 300),
                            ('MovimentacaoDiaria', 'Identificador', 100),
                            ('MovimentacaoDiaria', 'NomePessoaTransacao', 120),
                            ('MovimentacaoDiaria', 'TipoLancamento', 20),
                            ('PessoaMovimentacao', 'NomePessoa', 120),
                            ('TipoCartao', 'Descricao', 200),
                            ('TipoCartao', 'NomeTipoCartao', 50),
                            ('TipoMovimentacao', 'Descricao', 200),
                            ('TipoMovimentacao', 'NomeTipoMovimentacao', 80),
                            ('Transactions', 'Name', 120),
                            ('Transactions', 'Observacao', 500),
                            ('Transactions', 'Status', 20)
                            ) AS t(tabela, coluna, limite)
                        LOOP
                            EXECUTE format('SELECT max(length(%I)) FROM %I', r.coluna, r.tabela) INTO maior;
                            IF maior > r.limite THEN
                                excedentes := excedentes || format('%s.%s: maior valor %s, limite %s; ', r.tabela, r.coluna, maior, r.limite);
                            END IF;
                        END LOOP;

                        IF excedentes <> '' THEN
                            RAISE EXCEPTION 'DB-P3-04: dados acima do limite -> %', excedentes;
                        END IF;
                    END $$;
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Transactions" ALTER COLUMN "Status" TYPE character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Transactions" ALTER COLUMN "Observacao" TYPE character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Transactions" ALTER COLUMN "Name" TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "TipoMovimentacao" ALTER COLUMN "NomeTipoMovimentacao" TYPE character varying(80);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "TipoMovimentacao" ALTER COLUMN "Descricao" TYPE character varying(200);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "TipoCartao" ALTER COLUMN "NomeTipoCartao" TYPE character varying(50);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "TipoCartao" ALTER COLUMN "Descricao" TYPE character varying(200);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "PessoaMovimentacao" ALTER COLUMN "NomePessoa" TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "MovimentacaoDiaria" ALTER COLUMN "TipoLancamento" TYPE character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "MovimentacaoDiaria" ALTER COLUMN "NomePessoaTransacao" TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "MovimentacaoDiaria" ALTER COLUMN "Identificador" TYPE character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "MovimentacaoDiaria" ALTER COLUMN "Descricao" TYPE character varying(300);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Lista" ALTER COLUMN "Nome" TYPE character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ItemLista" ALTER COLUMN "Status" TYPE character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ItemLista" ALTER COLUMN "Observacao" TYPE character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "TipoLancamento" TYPE character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "Observacao" TYPE character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "NomePessoaTransacao" TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "Identificador" TYPE character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "Descricao" TYPE character varying(300);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancario" ALTER COLUMN "NomeArquivoOrigem" TYPE character varying(255);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancario" ALTER COLUMN "NomeArquivo" TYPE character varying(255);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Categories" ALTER COLUMN "SubCategory" TYPE character varying(80);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Categories" ALTER COLUMN "Name" TYPE character varying(80);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Banco" ALTER COLUMN "NumeroConta" TYPE character varying(30);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Banco" ALTER COLUMN "NomeBanco" TYPE character varying(80);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Banco" ALTER COLUMN "Agencia" TYPE character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "Role" TYPE character varying(30);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "NomeUsuario" TYPE character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Accounts" ALTER COLUMN "Name" TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Accounts" ALTER COLUMN "DataOperacao" TYPE character varying(10);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261020135241_LimitesDeTexto', '8.0.8');
    END IF;
END $EF$;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" DROP CONSTRAINT "FK_Accounts_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" DROP CONSTRAINT "FK_Accounts_Categories_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoleClaims" DROP CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserClaims" DROP CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserLogins" DROP CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserRoles" DROP CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserRoles" DROP CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserTokens" DROP CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" DROP CONSTRAINT "FK_Banco_AspNetUsers_UerId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" DROP CONSTRAINT fk_banco_tipocartao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" DROP CONSTRAINT "FK_Categories_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" DROP CONSTRAINT "ContaVencimento_ContaId_fkey";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" DROP CONSTRAINT contavencimento_user_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" DROP CONSTRAINT "FK_ExtratoBancario_TipoCartao_TipoCartaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" DROP CONSTRAINT extratobancario_bancoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" DROP CONSTRAINT extratobancario_user_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT "FK_ExtratoBancarioItem_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT "extratoBancarioItem_bancoId_fkey";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_bancoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_extrato_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_pessoamovimentacaoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_tipocartaoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_tipomovimentacaoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" DROP CONSTRAINT "FK_ItemLista_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" DROP CONSTRAINT itemlista_listaid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" DROP CONSTRAINT "FK_Lista_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" DROP CONSTRAINT "FK_MovimentacaoDiaria_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" DROP CONSTRAINT movimentacaodiaria_bancoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" DROP CONSTRAINT movimentacaodiaria_tipocartaoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" DROP CONSTRAINT movimentacaodiaria_tipomovimentacaoid_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" DROP CONSTRAINT "FK_PessoaMovimentacao_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" DROP CONSTRAINT "FK_PessoaMovimentacao_Categories_CategoriaId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" DROP CONSTRAINT "FK_PessoaMovimentacao_TipoMovimentacao_TipoMovimentacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" DROP CONSTRAINT fk_tipocartao_aspnetusers;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" DROP CONSTRAINT fk_tipomovimentacao_aspnetusers;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" DROP CONSTRAINT "FK_Transactions_Accounts_IdAccount";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" DROP CONSTRAINT "FK_Transactions_AspNetUsers_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" DROP CONSTRAINT "FK_Transactions_Categories_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" DROP CONSTRAINT "PK_Transactions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" DROP CONSTRAINT transactions_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" DROP CONSTRAINT transactions_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" DROP CONSTRAINT tipomovimentacao_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" DROP CONSTRAINT tipomovimentacao_valormeta_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" DROP CONSTRAINT tipocartao_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    DROP INDEX "UX_TipoCartao_Sistema_NomeTipoCartao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" DROP CONSTRAINT "PessoaMovimentacao_pkey";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" DROP CONSTRAINT movimentacaodiaria_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" DROP CONSTRAINT lista_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" DROP CONSTRAINT lista_tipolista_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" DROP CONSTRAINT itemlista_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" DROP CONSTRAINT itemlista_quantidade_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" DROP CONSTRAINT itemlista_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT "ExtratoBancarioItem_pkey";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    DROP INDEX "UX_ExtratoBancarioItem_ChaveImportacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" DROP CONSTRAINT extratobancarioitem_tipolancamento_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" DROP CONSTRAINT "ExtratoBancario_pkey";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" DROP CONSTRAINT "ContaVencimento_pkey";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" DROP CONSTRAINT contavencimento_dia_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" DROP CONSTRAINT "PK_Categories";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" DROP CONSTRAINT categories_naturezaoperacao_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" DROP CONSTRAINT categories_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" DROP CONSTRAINT banco_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserTokens" DROP CONSTRAINT "PK_AspNetUserTokens";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" DROP CONSTRAINT "PK_AspNetUsers";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserRoles" DROP CONSTRAINT "PK_AspNetUserRoles";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserLogins" DROP CONSTRAINT "PK_AspNetUserLogins";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserClaims" DROP CONSTRAINT "PK_AspNetUserClaims";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoles" DROP CONSTRAINT "PK_AspNetRoles";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoleClaims" DROP CONSTRAINT "PK_AspNetRoleClaims";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" DROP CONSTRAINT "PK_Accounts";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" DROP CONSTRAINT accounts_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" DROP CONSTRAINT accounts_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME TO transacoes;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" RENAME TO tipos_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" RENAME TO tipos_cartao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" RENAME TO pessoas_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME TO movimentacoes_diarias;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" RENAME TO listas;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME TO itens_lista;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME TO extratos_bancarios_itens;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME TO extratos_bancarios;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" RENAME TO contas_vencimento;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" RENAME TO categorias;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME TO bancos;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserTokens" RENAME TO asp_net_user_tokens;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME TO asp_net_users;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserRoles" RENAME TO asp_net_user_roles;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserLogins" RENAME TO asp_net_user_logins;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserClaims" RENAME TO asp_net_user_claims;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoles" RENAME TO asp_net_roles;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoleClaims" RENAME TO asp_net_role_claims;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME TO contas;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "Status" TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "Observacao" TO observacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "QuantidadeParcelas" TO quantidade_parcelas;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "ParcelaAtual" TO parcela_atual;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "EhParcelado" TO eh_parcelado;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "DataCriacao" TO data_criacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "Value" TO valor;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "Name" TO nome;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "IdAccount" TO conta_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "Date" TO data;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME COLUMN "CategoryId" TO categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_Transactions_UserId_Date" RENAME TO ix_transacoes_usuario_id_data;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_Transactions_IdAccount" RENAME TO ix_transacoes_conta_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_Transactions_CategoryId" RENAME TO ix_transacoes_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao RENAME COLUMN "Descricao" TO descricao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao RENAME COLUMN "ValorMeta" TO valor_meta;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao RENAME COLUMN "NomeTipoMovimentacao" TO nome_tipo_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_TipoMovimentacao_UserId" RENAME TO ix_tipos_movimentacao_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao RENAME COLUMN "Descricao" TO descricao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao RENAME COLUMN "NomeTipoCartao" TO nome_tipo_cartao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao RENAME COLUMN "EhCredito" TO eh_credito;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "UX_TipoCartao_UserId_NomeTipoCartao" RENAME TO ux_tipos_cartao_usuario_id_nome_tipo_cartao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao RENAME COLUMN "TipoMovimentacaoId" TO tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao RENAME COLUMN "NomePessoa" TO nome_pessoa;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao RENAME COLUMN "CategoriaId" TO categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "UX_PessoaMovimentacao_UserId_NomePessoa" RENAME TO ux_pessoas_movimentacao_usuario_id_nome_pessoa;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_PessoaMovimentacao_TipoMovimentacaoId" RENAME TO ix_pessoas_movimentacao_tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_PessoaMovimentacao_CategoriaId" RENAME TO ix_pessoas_movimentacao_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "Valor" TO valor;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "Identificador" TO identificador;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "Descricao" TO descricao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "TipoMovimentacaoId" TO tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "TipoLancamento" TO tipo_lancamento;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "TipoCartaoId" TO tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "NomePessoaTransacao" TO nome_pessoa_transacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "DataMovimentacao" TO data_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "BancoId" TO banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_MovimentacaoDiaria_UserId" RENAME TO ix_movimentacoes_diarias_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_MovimentacaoDiaria_TipoMovimentacaoId" RENAME TO ix_movimentacoes_diarias_tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_MovimentacaoDiaria_TipoCartaoId" RENAME TO ix_movimentacoes_diarias_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_MovimentacaoDiaria_BancoId" RENAME TO ix_movimentacoes_diarias_banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas RENAME COLUMN "Status" TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas RENAME COLUMN "Nome" TO nome;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas RENAME COLUMN "TipoLista" TO tipo_lista;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_Lista_UserId" RENAME TO ix_listas_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "Valor" TO valor;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "Status" TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "Quantidade" TO quantidade;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "Observacao" TO observacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "Descricao" TO descricao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "ListaId" TO lista_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "HorarioTarefa" TO horario_tarefa;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "DataTarefa" TO data_tarefa;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ItemLista_UserId" RENAME TO ix_itens_lista_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ItemLista_ListaId" RENAME TO ix_itens_lista_lista_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "Valor" TO valor;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "Observacao" TO observacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "Identificador" TO identificador;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "Descricao" TO descricao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "TipoMovimentacaoId" TO tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "TipoLancamento" TO tipo_lancamento;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "TipoCartaoId" TO tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "QuantidadeParcelas" TO quantidade_parcelas;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "PessoaMovimentacaoId" TO pessoa_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "ParcelaAtual" TO parcela_atual;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "NumeroFatura" TO numero_fatura;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "NomePessoaTransacao" TO nome_pessoa_transacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "GrupoParcelamento" TO grupo_parcelamento;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "ExtratoBancarioId" TO extrato_bancario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "EhParcelado" TO eh_parcelado;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "DataMovimentacao" TO data_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "DataCriacao" TO data_criacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "ChaveImportacao" TO chave_importacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "ChaveDescricao" TO chave_descricao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "CategoriaId" TO categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "BancoId" TO banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancarioItem_UserId_NumeroFatura" RENAME TO ix_extratos_bancarios_itens_usuario_id_numero_fatura;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancarioItem_UserId_DataMovimentacao" RENAME TO ix_extratos_bancarios_itens_usuario_id_data_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancarioItem_TipoMovimentacaoId" RENAME TO ix_extratos_bancarios_itens_tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancarioItem_TipoCartaoId" RENAME TO ix_extratos_bancarios_itens_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancarioItem_PessoaMovimentacaoId" RENAME TO ix_extratos_bancarios_itens_pessoa_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancarioItem_CategoriaId" RENAME TO ix_extratos_bancarios_itens_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancarioItem_BancoId" RENAME TO ix_extratos_bancarios_itens_banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX idx_extratobancarioitem_extrato RENAME TO ix_extratos_bancarios_itens_extrato_bancario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "Situacao" TO situacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "ValorTotal" TO valor_total;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "TipoCartaoId" TO tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "QuantidadeLancamentos" TO quantidade_lancamentos;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "NomeArquivoOrigem" TO nome_arquivo_origem;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "NomeArquivo" TO nome_arquivo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "LoteImportacaoId" TO lote_importacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "DataInicioPeriodo" TO data_inicio_periodo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "DataImportacao" TO data_importacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "DataFimPeriodo" TO data_fim_periodo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "BancoId" TO banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancario_UserId" RENAME TO ix_extratos_bancarios_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancario_TipoCartaoId" RENAME TO ix_extratos_bancarios_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ExtratoBancario_BancoId" RENAME TO ix_extratos_bancarios_banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento RENAME COLUMN "Dia" TO dia;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento RENAME COLUMN "ContaId" TO conta_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ContaVencimento_UserId" RENAME TO ix_contas_vencimento_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_ContaVencimento_ContaId" RENAME TO ix_contas_vencimento_conta_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias RENAME COLUMN "Status" TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias RENAME COLUMN "NaturezaOperacao" TO natureza_operacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias RENAME COLUMN "SubCategory" TO subcategoria;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias RENAME COLUMN "Name" TO nome;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "UX_Categories_UserId_Name_SubCategory" RENAME TO ux_categorias_usuario_id_nome_subcategoria;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME COLUMN "Ativo" TO ativo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME COLUMN "Agencia" TO agencia;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME COLUMN "TipoCartaoId" TO tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME COLUMN "SaldoInicial" TO saldo_inicial;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME COLUMN "NumeroConta" TO numero_conta;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME COLUMN "NomeBanco" TO nome_banco;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "UX_Banco_UserId_NomeBanco" RENAME TO ux_bancos_usuario_id_nome_banco;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_Banco_TipoCartaoId" RENAME TO ix_bancos_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_tokens RENAME COLUMN "Value" TO value;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_tokens RENAME COLUMN "Name" TO name;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_tokens RENAME COLUMN "LoginProvider" TO login_provider;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_tokens RENAME COLUMN "UserId" TO user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "Role" TO role;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "Email" TO email;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "UserName" TO user_name;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "TwoFactorEnabled" TO two_factor_enabled;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "StatusAtivo" TO status_ativo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "SecurityStamp" TO security_stamp;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "PhoneNumberConfirmed" TO phone_number_confirmed;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "PhoneNumber" TO phone_number;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "PasswordHash" TO password_hash;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "NormalizedUserName" TO normalized_user_name;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "NormalizedEmail" TO normalized_email;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "NomeUsuario" TO nome_usuario;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "LockoutEnd" TO lockout_end;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "LockoutEnabled" TO lockout_enabled;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "EmailConfirmed" TO email_confirmed;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "CriadoEm" TO criado_em;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "ConcurrencyStamp" TO concurrency_stamp;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "AtualizadoEm" TO atualizado_em;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME COLUMN "AccessFailedCount" TO access_failed_count;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "UserNameIndex" RENAME TO user_name_index;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "EmailIndex" RENAME TO email_index;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_roles RENAME COLUMN "RoleId" TO role_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_roles RENAME COLUMN "UserId" TO user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_AspNetUserRoles_RoleId" RENAME TO ix_asp_net_user_roles_role_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_logins RENAME COLUMN "UserId" TO user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_logins RENAME COLUMN "ProviderDisplayName" TO provider_display_name;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_logins RENAME COLUMN "ProviderKey" TO provider_key;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_logins RENAME COLUMN "LoginProvider" TO login_provider;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_AspNetUserLogins_UserId" RENAME TO ix_asp_net_user_logins_user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_claims RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_claims RENAME COLUMN "UserId" TO user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_claims RENAME COLUMN "ClaimValue" TO claim_value;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_claims RENAME COLUMN "ClaimType" TO claim_type;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_AspNetUserClaims_UserId" RENAME TO ix_asp_net_user_claims_user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_roles RENAME COLUMN "Name" TO name;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_roles RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_roles RENAME COLUMN "NormalizedName" TO normalized_name;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_roles RENAME COLUMN "ConcurrencyStamp" TO concurrency_stamp;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "RoleNameIndex" RENAME TO role_name_index;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_role_claims RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_role_claims RENAME COLUMN "RoleId" TO role_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_role_claims RENAME COLUMN "ClaimValue" TO claim_value;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_role_claims RENAME COLUMN "ClaimType" TO claim_type;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_AspNetRoleClaims_RoleId" RENAME TO ix_asp_net_role_claims_role_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "Status" TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "Id" TO id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "QuantidadeParcelas" TO quantidade_parcelas;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "ParcelaAtual" TO parcela_atual;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "EhParcelado" TO eh_parcelado;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "DataOperacao" TO data_operacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "DataAlteracao" TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "Value" TO valor;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "UserId" TO usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "Name" TO nome;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME COLUMN "CategoryId" TO categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_Accounts_UserId" RENAME TO ix_contas_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX "IX_Accounts_CategoryId" RENAME TO ix_contas_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT tipos_movimentacao_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao ADD CONSTRAINT tipos_cartao_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT pessoas_movimentacao_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT extratos_bancarios_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT contas_vencimento_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos ADD CONSTRAINT bancos_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_tokens ADD CONSTRAINT asp_net_user_tokens_pkey PRIMARY KEY (user_id, login_provider, name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users ADD CONSTRAINT asp_net_users_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_roles ADD CONSTRAINT asp_net_user_roles_pkey PRIMARY KEY (user_id, role_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_logins ADD CONSTRAINT asp_net_user_logins_pkey PRIMARY KEY (login_provider, provider_key);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_claims ADD CONSTRAINT asp_net_user_claims_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_roles ADD CONSTRAINT asp_net_roles_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_role_claims ADD CONSTRAINT asp_net_role_claims_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_parcelas_check CHECK ("quantidade_parcelas" >= 1 AND "parcela_atual" BETWEEN 1 AND "quantidade_parcelas");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_status_check CHECK ("status" IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT tipos_movimentacao_valor_meta_check CHECK ("valor_meta" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    CREATE UNIQUE INDEX ux_tipos_cartao_sistema_nome_tipo_cartao ON tipos_cartao (nome_tipo_cartao) WHERE "usuario_id" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_tipo_lista_check CHECK ("tipo_lista" IN (1, 2, 3));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_quantidade_check CHECK ("quantidade" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_status_check CHECK ("status" IN ('PENDENTE', 'AGUARDANDO', 'OK'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    CREATE UNIQUE INDEX ux_extratos_bancarios_itens_chave_importacao ON extratos_bancarios_itens (usuario_id, banco_id, chave_importacao) WHERE "chave_importacao" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_parcelas_check CHECK ("quantidade_parcelas" >= 1 AND "parcela_atual" BETWEEN 1 AND "quantidade_parcelas");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_tipo_lancamento_check CHECK ("tipo_lancamento" IN ('Entrada', 'Saída'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT contas_vencimento_dia_check CHECK ("dia" BETWEEN 1 AND 31);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_natureza_operacao_check CHECK ("natureza_operacao" BETWEEN 0 AND 5);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_status_check CHECK ("status" IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_parcelas_check CHECK (NOT "eh_parcelado" OR ("quantidade_parcelas" >= 1 AND "parcela_atual" BETWEEN 0 AND "quantidade_parcelas"));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_status_check CHECK ("status" IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_role_claims ADD CONSTRAINT asp_net_role_claims_role_id_fkey FOREIGN KEY (role_id) REFERENCES asp_net_roles (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_claims ADD CONSTRAINT asp_net_user_claims_user_id_fkey FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_logins ADD CONSTRAINT asp_net_user_logins_user_id_fkey FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_roles ADD CONSTRAINT asp_net_user_roles_role_id_fkey FOREIGN KEY (role_id) REFERENCES asp_net_roles (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_roles ADD CONSTRAINT asp_net_user_roles_user_id_fkey FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_tokens ADD CONSTRAINT asp_net_user_tokens_user_id_fkey FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos ADD CONSTRAINT bancos_tipo_cartao_id_fkey FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos ADD CONSTRAINT bancos_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_categoria_id_fkey FOREIGN KEY (categoria_id) REFERENCES categorias (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT contas_vencimento_conta_id_fkey FOREIGN KEY (conta_id) REFERENCES contas (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT contas_vencimento_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT extratos_bancarios_banco_id_fkey FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT extratos_bancarios_tipo_cartao_id_fkey FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT extratos_bancarios_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_banco_id_fkey FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_categoria_id_fkey FOREIGN KEY (categoria_id) REFERENCES categorias (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_extrato_bancario_id_fkey FOREIGN KEY (extrato_bancario_id) REFERENCES extratos_bancarios (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_pessoa_movimentacao_id_fkey FOREIGN KEY (pessoa_movimentacao_id) REFERENCES pessoas_movimentacao (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_tipo_cartao_id_fkey FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_tipo_movimentacao_id_fkey FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_lista_id_fkey FOREIGN KEY (lista_id) REFERENCES listas (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_banco_id_fkey FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_tipo_cartao_id_fkey FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_tipo_movimentacao_id_fkey FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT pessoas_movimentacao_categoria_id_fkey FOREIGN KEY (categoria_id) REFERENCES categorias (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT pessoas_movimentacao_tipo_movimentacao_id_fkey FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT pessoas_movimentacao_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao ADD CONSTRAINT tipos_cartao_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT tipos_movimentacao_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_categoria_id_fkey FOREIGN KEY (categoria_id) REFERENCES categorias (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_conta_id_fkey FOREIGN KEY (conta_id) REFERENCES contas (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261021132335_ConvencoesDeNomes', '8.0.8');
    END IF;
END $EF$;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN

                    DO $$
                    DECLARE
                        r record;
                        maior int;
                        excedentes text := '';
                    BEGIN
                        FOR r IN
                            SELECT * FROM (VALUES
                            ('asp_net_users', 'phone_number', 20),
                            ('bancos', 'agencia', 10),
                            ('bancos', 'numero_conta', 20),
                            ('extratos_bancarios', 'situacao', 20),
                            ('extratos_bancarios_itens', 'descricao', 200),
                            ('movimentacoes_diarias', 'descricao', 200),
                            ('tipos_movimentacao', 'nome_tipo_movimentacao', 60)
                            ) AS t(tabela, coluna, limite)
                        LOOP
                            EXECUTE format('SELECT max(length(%I)) FROM %I', r.coluna, r.tabela) INTO maior;
                            IF maior > r.limite THEN
                                excedentes := excedentes || format('%s.%s: maior valor %s, limite %s; ', r.tabela, r.coluna, maior, r.limite);
                            END IF;
                        END LOOP;

                        IF excedentes <> '' THEN
                            RAISE EXCEPTION 'DB-P3-04: dados acima do limite -> %', excedentes;
                        END IF;
                    END $$;
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE tipos_movimentacao ALTER COLUMN nome_tipo_movimentacao TYPE character varying(60);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE tipos_cartao ALTER COLUMN nome_tipo_cartao TYPE character varying(60);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE movimentacoes_diarias ALTER COLUMN descricao TYPE character varying(200);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE listas ALTER COLUMN nome TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE extratos_bancarios_itens ALTER COLUMN descricao TYPE character varying(200);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE extratos_bancarios ALTER COLUMN situacao TYPE character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE extratos_bancarios ALTER COLUMN nome_arquivo_origem TYPE character varying(260);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE extratos_bancarios ALTER COLUMN nome_arquivo TYPE character varying(260);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE contas ALTER COLUMN data_operacao TYPE character varying(30);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE categorias ALTER COLUMN subcategoria TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE categorias ALTER COLUMN nome TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE bancos ALTER COLUMN numero_conta TYPE character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE bancos ALTER COLUMN nome_banco TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE bancos ALTER COLUMN agencia TYPE character varying(10);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE asp_net_users ALTER COLUMN phone_number TYPE character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE asp_net_users ALTER COLUMN nome_usuario TYPE character varying(120);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261021174951_AjusteLimitesDeTexto', '8.0.8');
    END IF;
END $EF$;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_role_claims DROP CONSTRAINT asp_net_role_claims_role_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_claims DROP CONSTRAINT asp_net_user_claims_user_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_logins DROP CONSTRAINT asp_net_user_logins_user_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles DROP CONSTRAINT asp_net_user_roles_role_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles DROP CONSTRAINT asp_net_user_roles_user_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_tokens DROP CONSTRAINT asp_net_user_tokens_user_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos DROP CONSTRAINT bancos_tipo_cartao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos DROP CONSTRAINT bancos_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_categoria_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT contas_vencimento_conta_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT contas_vencimento_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT extratos_bancarios_banco_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT extratos_bancarios_tipo_cartao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT extratos_bancarios_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_banco_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_categoria_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_extrato_bancario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_pessoa_movimentacao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_tipo_cartao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_tipo_movimentacao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_lista_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas DROP CONSTRAINT listas_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_banco_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_tipo_cartao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_tipo_movimentacao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT pessoas_movimentacao_categoria_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT pessoas_movimentacao_tipo_movimentacao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT pessoas_movimentacao_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_cartao DROP CONSTRAINT tipos_cartao_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT tipos_movimentacao_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_categoria_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_conta_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT tipos_movimentacao_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT tipos_movimentacao_valor_meta_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_cartao DROP CONSTRAINT tipos_cartao_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT pessoas_movimentacao_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas DROP CONSTRAINT listas_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas DROP CONSTRAINT listas_tipo_lista_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_quantidade_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT extratos_bancarios_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT contas_vencimento_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT contas_vencimento_dia_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_natureza_operacao_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos DROP CONSTRAINT bancos_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users DROP CONSTRAINT asp_net_users_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_tokens DROP CONSTRAINT asp_net_user_tokens_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles DROP CONSTRAINT asp_net_user_roles_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_logins DROP CONSTRAINT asp_net_user_logins_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_claims DROP CONSTRAINT asp_net_user_claims_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_roles DROP CONSTRAINT asp_net_roles_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_role_claims DROP CONSTRAINT asp_net_role_claims_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_tipo_lancamento_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens RENAME TO itens_extrato_bancario;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes RENAME COLUMN status TO situacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ux_tipos_cartao_usuario_id_nome_tipo_cartao RENAME TO ix_tipos_cartao_usuario_id_nome_tipo_cartao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ux_pessoas_movimentacao_usuario_id_nome_pessoa RENAME TO ix_pessoas_movimentacao_usuario_id_nome_pessoa;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas RENAME COLUMN status TO ativo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista RENAME COLUMN status TO situacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas RENAME COLUMN status TO ativo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias RENAME COLUMN status TO ativo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ux_categorias_usuario_id_nome_subcategoria RENAME TO ix_categorias_usuario_id_nome_subcategoria;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ux_bancos_usuario_id_nome_banco RENAME TO ix_bancos_usuario_id_nome_banco;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users RENAME COLUMN status_ativo TO ativo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users RENAME COLUMN role TO perfil;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users RENAME COLUMN criado_em TO data_criacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users RENAME COLUMN atualizado_em TO data_alteracao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ux_extratos_bancarios_itens_chave_importacao RENAME TO ux_itens_extrato_bancario_chave_importacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_extratos_bancarios_itens_usuario_id_numero_fatura RENAME TO ix_itens_extrato_bancario_usuario_id_numero_fatura;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_extratos_bancarios_itens_usuario_id_data_movimentacao RENAME TO ix_itens_extrato_bancario_usuario_id_data_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_extratos_bancarios_itens_tipo_movimentacao_id RENAME TO ix_itens_extrato_bancario_tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_extratos_bancarios_itens_tipo_cartao_id RENAME TO ix_itens_extrato_bancario_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_extratos_bancarios_itens_pessoa_movimentacao_id RENAME TO ix_itens_extrato_bancario_pessoa_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_extratos_bancarios_itens_extrato_bancario_id RENAME TO ix_itens_extrato_bancario_extrato_bancario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_extratos_bancarios_itens_categoria_id RENAME TO ix_itens_extrato_bancario_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_extratos_bancarios_itens_banco_id RENAME TO ix_itens_extrato_bancario_banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT pk_transacoes PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT pk_tipos_movimentacao PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_cartao ADD CONSTRAINT pk_tipos_cartao PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT pk_pessoas_movimentacao PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT pk_movimentacoes_diarias PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas ADD CONSTRAINT pk_listas PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT pk_itens_lista PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT pk_extratos_bancarios PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT pk_contas_vencimento PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT pk_contas PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias ADD CONSTRAINT pk_categorias PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos ADD CONSTRAINT pk_bancos PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users ADD CONSTRAINT pk_asp_net_users PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_tokens ADD CONSTRAINT pk_asp_net_user_tokens PRIMARY KEY (user_id, login_provider, name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles ADD CONSTRAINT pk_asp_net_user_roles PRIMARY KEY (user_id, role_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_logins ADD CONSTRAINT pk_asp_net_user_logins PRIMARY KEY (login_provider, provider_key);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_claims ADD CONSTRAINT pk_asp_net_user_claims PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_roles ADD CONSTRAINT pk_asp_net_roles PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_role_claims ADD CONSTRAINT pk_asp_net_role_claims PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT pk_itens_extrato_bancario PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_parcelas_check CHECK (quantidade_parcelas >= 1 AND parcela_atual BETWEEN 1 AND quantidade_parcelas);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_situacao_check CHECK (situacao IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT tipos_movimentacao_valor_meta_check CHECK (valor_meta >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_tipo_lista_check CHECK (tipo_lista IN (1, 2, 3));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_quantidade_check CHECK (quantidade >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_situacao_check CHECK (situacao IN ('PENDENTE', 'AGUARDANDO', 'OK'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT contas_vencimento_dia_check CHECK (dia BETWEEN 1 AND 31);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_ativo_check CHECK (ativo IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_parcelas_check CHECK (NOT eh_parcelado OR (quantidade_parcelas >= 1 AND parcela_atual BETWEEN 0 AND quantidade_parcelas));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_ativo_check CHECK (ativo IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_natureza_operacao_check CHECK (natureza_operacao BETWEEN 0 AND 5);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT itens_extrato_bancario_parcelas_check CHECK (quantidade_parcelas >= 1 AND parcela_atual BETWEEN 1 AND quantidade_parcelas);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT itens_extrato_bancario_tipo_lancamento_check CHECK (tipo_lancamento IN ('Entrada', 'Saída'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_role_claims ADD CONSTRAINT fk_asp_net_role_claims_asp_net_roles_role_id FOREIGN KEY (role_id) REFERENCES asp_net_roles (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_claims ADD CONSTRAINT fk_asp_net_user_claims_asp_net_users_user_id FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_logins ADD CONSTRAINT fk_asp_net_user_logins_asp_net_users_user_id FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles ADD CONSTRAINT fk_asp_net_user_roles_asp_net_roles_role_id FOREIGN KEY (role_id) REFERENCES asp_net_roles (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles ADD CONSTRAINT fk_asp_net_user_roles_asp_net_users_user_id FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_tokens ADD CONSTRAINT fk_asp_net_user_tokens_asp_net_users_user_id FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos ADD CONSTRAINT fk_bancos_tipos_cartao_tipo_cartao_id FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos ADD CONSTRAINT fk_bancos_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias ADD CONSTRAINT fk_categorias_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT fk_contas_categorias_categoria_id FOREIGN KEY (categoria_id) REFERENCES categorias (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT fk_contas_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT fk_contas_vencimento_contas_conta_id FOREIGN KEY (conta_id) REFERENCES contas (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT fk_contas_vencimento_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT fk_extratos_bancarios_bancos_banco_id FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT fk_extratos_bancarios_tipos_cartao_tipo_cartao_id FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT fk_extratos_bancarios_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_bancos_banco_id FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_categorias_categoria_id FOREIGN KEY (categoria_id) REFERENCES categorias (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_extratos_bancarios_extrato_bancario_ FOREIGN KEY (extrato_bancario_id) REFERENCES extratos_bancarios (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_pessoas_movimentacao_pessoa_moviment FOREIGN KEY (pessoa_movimentacao_id) REFERENCES pessoas_movimentacao (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_tipos_cartao_tipo_cartao_id FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_tipos_movimentacao_tipo_movimentacao FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT fk_itens_lista_listas_lista_id FOREIGN KEY (lista_id) REFERENCES listas (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT fk_itens_lista_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas ADD CONSTRAINT fk_listas_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT fk_movimentacoes_diarias_bancos_banco_id FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT fk_movimentacoes_diarias_tipos_cartao_tipo_cartao_id FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT fk_movimentacoes_diarias_tipos_movimentacao_tipo_movimentacao_ FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT fk_movimentacoes_diarias_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT fk_pessoas_movimentacao_categorias_categoria_id FOREIGN KEY (categoria_id) REFERENCES categorias (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT fk_pessoas_movimentacao_tipos_movimentacao_tipo_movimentacao_id FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT fk_pessoas_movimentacao_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_cartao ADD CONSTRAINT fk_tipos_cartao_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT fk_tipos_movimentacao_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT fk_transacoes_categorias_categoria_id FOREIGN KEY (categoria_id) REFERENCES categorias (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT fk_transacoes_contas_conta_id FOREIGN KEY (conta_id) REFERENCES contas (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT fk_transacoes_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261021182221_ConvencoesDaSpec', '8.0.8');
    END IF;
END $EF$;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE bancos DROP CONSTRAINT fk_bancos_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE categorias DROP CONSTRAINT fk_categorias_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE contas DROP CONSTRAINT fk_contas_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT fk_contas_vencimento_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT fk_extratos_bancarios_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_extratos_bancarios_extrato_bancario_;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_pessoas_movimentacao_pessoa_moviment;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_tipos_movimentacao_tipo_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT fk_itens_lista_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE listas DROP CONSTRAINT fk_listas_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT fk_movimentacoes_diarias_tipos_movimentacao_tipo_movimentacao_;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT fk_movimentacoes_diarias_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT fk_pessoas_movimentacao_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE tipos_cartao DROP CONSTRAINT fk_tipos_cartao_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT fk_tipos_movimentacao_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE transacoes DROP CONSTRAINT fk_transacoes_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE bancos ADD CONSTRAINT fk_bancos_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE categorias ADD CONSTRAINT fk_categorias_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE contas ADD CONSTRAINT fk_contas_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT fk_contas_vencimento_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT fk_extratos_bancarios_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_extrato_bancario_id FOREIGN KEY (extrato_bancario_id) REFERENCES extratos_bancarios (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_pessoa_movimentacao_id FOREIGN KEY (pessoa_movimentacao_id) REFERENCES pessoas_movimentacao (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_tipo_movimentacao_id FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT fk_itens_lista_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE listas ADD CONSTRAINT fk_listas_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT fk_movimentacoes_diarias_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT fk_movimentacoes_diarias_tipo_movimentacao_id FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT fk_pessoas_movimentacao_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE tipos_cartao ADD CONSTRAINT fk_tipos_cartao_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT fk_tipos_movimentacao_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE transacoes ADD CONSTRAINT fk_transacoes_asp_net_users_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261021184850_NomesDeForeignKeys', '8.0.8');
    END IF;
END $EF$;
COMMIT;

