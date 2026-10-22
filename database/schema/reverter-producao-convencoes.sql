-- [DB] P3 — Convenções: desfaz as 5 migrations de aplicar-producao-convencoes.sql e volta a 20261014004442_IndicesConsulta.
-- Gerado por dotnet ef migrations script --idempotent; as transações do EF foram unidas numa só.
-- Depois de rodar: tráfego da API de volta para a revisão anterior.
START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE bancos DROP CONSTRAINT fk_bancos_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE categorias DROP CONSTRAINT fk_categorias_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE contas DROP CONSTRAINT fk_contas_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT fk_contas_vencimento_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT fk_extratos_bancarios_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_extrato_bancario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_pessoa_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT fk_itens_lista_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE listas DROP CONSTRAINT fk_listas_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT fk_movimentacoes_diarias_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT fk_movimentacoes_diarias_tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT fk_pessoas_movimentacao_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE tipos_cartao DROP CONSTRAINT fk_tipos_cartao_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT fk_tipos_movimentacao_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE transacoes DROP CONSTRAINT fk_transacoes_asp_net_users_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE bancos ADD CONSTRAINT fk_bancos_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE categorias ADD CONSTRAINT fk_categorias_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE contas ADD CONSTRAINT fk_contas_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT fk_contas_vencimento_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT fk_extratos_bancarios_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_extratos_bancarios_extrato_bancario_ FOREIGN KEY (extrato_bancario_id) REFERENCES extratos_bancarios (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_pessoas_movimentacao_pessoa_moviment FOREIGN KEY (pessoa_movimentacao_id) REFERENCES pessoas_movimentacao (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_tipos_movimentacao_tipo_movimentacao FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_extrato_bancario ADD CONSTRAINT fk_itens_extrato_bancario_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT fk_itens_lista_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE listas ADD CONSTRAINT fk_listas_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT fk_movimentacoes_diarias_tipos_movimentacao_tipo_movimentacao_ FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT fk_movimentacoes_diarias_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT fk_pessoas_movimentacao_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE tipos_cartao ADD CONSTRAINT fk_tipos_cartao_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT fk_tipos_movimentacao_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    ALTER TABLE transacoes ADD CONSTRAINT fk_transacoes_usuario_usuario_id FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20261021184850_NomesDeForeignKeys';
    END IF;
END $EF$;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_role_claims DROP CONSTRAINT fk_asp_net_role_claims_asp_net_roles_role_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_claims DROP CONSTRAINT fk_asp_net_user_claims_asp_net_users_user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_logins DROP CONSTRAINT fk_asp_net_user_logins_asp_net_users_user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles DROP CONSTRAINT fk_asp_net_user_roles_asp_net_roles_role_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles DROP CONSTRAINT fk_asp_net_user_roles_asp_net_users_user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_tokens DROP CONSTRAINT fk_asp_net_user_tokens_asp_net_users_user_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos DROP CONSTRAINT fk_bancos_tipos_cartao_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos DROP CONSTRAINT fk_bancos_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias DROP CONSTRAINT fk_categorias_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT fk_contas_categorias_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT fk_contas_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT fk_contas_vencimento_contas_conta_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT fk_contas_vencimento_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT fk_extratos_bancarios_bancos_banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT fk_extratos_bancarios_tipos_cartao_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT fk_extratos_bancarios_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_bancos_banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_categorias_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_extratos_bancarios_extrato_bancario_;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_pessoas_movimentacao_pessoa_moviment;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_tipos_cartao_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_tipos_movimentacao_tipo_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT fk_itens_extrato_bancario_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT fk_itens_lista_listas_lista_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT fk_itens_lista_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas DROP CONSTRAINT fk_listas_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT fk_movimentacoes_diarias_bancos_banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT fk_movimentacoes_diarias_tipos_cartao_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT fk_movimentacoes_diarias_tipos_movimentacao_tipo_movimentacao_;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT fk_movimentacoes_diarias_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT fk_pessoas_movimentacao_categorias_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT fk_pessoas_movimentacao_tipos_movimentacao_tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT fk_pessoas_movimentacao_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_cartao DROP CONSTRAINT fk_tipos_cartao_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT fk_tipos_movimentacao_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT fk_transacoes_categorias_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT fk_transacoes_contas_conta_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT fk_transacoes_usuario_usuario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT pk_transacoes;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_situacao_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT pk_tipos_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT tipos_movimentacao_valor_meta_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_cartao DROP CONSTRAINT pk_tipos_cartao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT pk_pessoas_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT pk_movimentacoes_diarias;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas DROP CONSTRAINT pk_listas;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas DROP CONSTRAINT listas_tipo_lista_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT pk_itens_lista;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_quantidade_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_situacao_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT pk_extratos_bancarios;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT pk_contas_vencimento;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT contas_vencimento_dia_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT pk_contas;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_ativo_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias DROP CONSTRAINT pk_categorias;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_ativo_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_natureza_operacao_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos DROP CONSTRAINT pk_bancos;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users DROP CONSTRAINT pk_asp_net_users;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_tokens DROP CONSTRAINT pk_asp_net_user_tokens;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles DROP CONSTRAINT pk_asp_net_user_roles;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_logins DROP CONSTRAINT pk_asp_net_user_logins;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_claims DROP CONSTRAINT pk_asp_net_user_claims;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_roles DROP CONSTRAINT pk_asp_net_roles;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_role_claims DROP CONSTRAINT pk_asp_net_role_claims;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT pk_itens_extrato_bancario;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT itens_extrato_bancario_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario DROP CONSTRAINT itens_extrato_bancario_tipo_lancamento_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_extrato_bancario RENAME TO extratos_bancarios_itens;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes RENAME COLUMN situacao TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_tipos_cartao_usuario_id_nome_tipo_cartao RENAME TO ux_tipos_cartao_usuario_id_nome_tipo_cartao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_pessoas_movimentacao_usuario_id_nome_pessoa RENAME TO ux_pessoas_movimentacao_usuario_id_nome_pessoa;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas RENAME COLUMN ativo TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista RENAME COLUMN situacao TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas RENAME COLUMN ativo TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias RENAME COLUMN ativo TO status;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_categorias_usuario_id_nome_subcategoria RENAME TO ux_categorias_usuario_id_nome_subcategoria;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_bancos_usuario_id_nome_banco RENAME TO ux_bancos_usuario_id_nome_banco;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users RENAME COLUMN perfil TO role;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users RENAME COLUMN data_criacao TO criado_em;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users RENAME COLUMN data_alteracao TO atualizado_em;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users RENAME COLUMN ativo TO status_ativo;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ux_itens_extrato_bancario_chave_importacao RENAME TO ux_extratos_bancarios_itens_chave_importacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_itens_extrato_bancario_usuario_id_numero_fatura RENAME TO ix_extratos_bancarios_itens_usuario_id_numero_fatura;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_itens_extrato_bancario_usuario_id_data_movimentacao RENAME TO ix_extratos_bancarios_itens_usuario_id_data_movimentacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_itens_extrato_bancario_tipo_movimentacao_id RENAME TO ix_extratos_bancarios_itens_tipo_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_itens_extrato_bancario_tipo_cartao_id RENAME TO ix_extratos_bancarios_itens_tipo_cartao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_itens_extrato_bancario_pessoa_movimentacao_id RENAME TO ix_extratos_bancarios_itens_pessoa_movimentacao_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_itens_extrato_bancario_extrato_bancario_id RENAME TO ix_extratos_bancarios_itens_extrato_bancario_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_itens_extrato_bancario_categoria_id RENAME TO ix_extratos_bancarios_itens_categoria_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER INDEX ix_itens_extrato_bancario_banco_id RENAME TO ix_extratos_bancarios_itens_banco_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT tipos_movimentacao_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_cartao ADD CONSTRAINT tipos_cartao_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT pessoas_movimentacao_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT extratos_bancarios_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT contas_vencimento_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos ADD CONSTRAINT bancos_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_users ADD CONSTRAINT asp_net_users_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_tokens ADD CONSTRAINT asp_net_user_tokens_pkey PRIMARY KEY (user_id, login_provider, name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles ADD CONSTRAINT asp_net_user_roles_pkey PRIMARY KEY (user_id, role_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_logins ADD CONSTRAINT asp_net_user_logins_pkey PRIMARY KEY (login_provider, provider_key);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_claims ADD CONSTRAINT asp_net_user_claims_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_roles ADD CONSTRAINT asp_net_roles_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_role_claims ADD CONSTRAINT asp_net_role_claims_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_pkey PRIMARY KEY (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_parcelas_check CHECK ("quantidade_parcelas" >= 1 AND "parcela_atual" BETWEEN 1 AND "quantidade_parcelas");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_status_check CHECK ("status" IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT tipos_movimentacao_valor_meta_check CHECK ("valor_meta" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_tipo_lista_check CHECK ("tipo_lista" IN (1, 2, 3));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_quantidade_check CHECK ("quantidade" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_status_check CHECK ("status" IN ('PENDENTE', 'AGUARDANDO', 'OK'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT contas_vencimento_dia_check CHECK ("dia" BETWEEN 1 AND 31);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_parcelas_check CHECK (NOT "eh_parcelado" OR ("quantidade_parcelas" >= 1 AND "parcela_atual" BETWEEN 0 AND "quantidade_parcelas"));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_status_check CHECK ("status" IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_natureza_operacao_check CHECK ("natureza_operacao" BETWEEN 0 AND 5);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_status_check CHECK ("status" IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_parcelas_check CHECK ("quantidade_parcelas" >= 1 AND "parcela_atual" BETWEEN 1 AND "quantidade_parcelas");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_tipo_lancamento_check CHECK ("tipo_lancamento" IN ('Entrada', 'Saída'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_role_claims ADD CONSTRAINT asp_net_role_claims_role_id_fkey FOREIGN KEY (role_id) REFERENCES asp_net_roles (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_claims ADD CONSTRAINT asp_net_user_claims_user_id_fkey FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_logins ADD CONSTRAINT asp_net_user_logins_user_id_fkey FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles ADD CONSTRAINT asp_net_user_roles_role_id_fkey FOREIGN KEY (role_id) REFERENCES asp_net_roles (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_roles ADD CONSTRAINT asp_net_user_roles_user_id_fkey FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE asp_net_user_tokens ADD CONSTRAINT asp_net_user_tokens_user_id_fkey FOREIGN KEY (user_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos ADD CONSTRAINT bancos_tipo_cartao_id_fkey FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE bancos ADD CONSTRAINT bancos_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE categorias ADD CONSTRAINT categorias_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_categoria_id_fkey FOREIGN KEY (categoria_id) REFERENCES categorias (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas ADD CONSTRAINT contas_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT contas_vencimento_conta_id_fkey FOREIGN KEY (conta_id) REFERENCES contas (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE contas_vencimento ADD CONSTRAINT contas_vencimento_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT extratos_bancarios_banco_id_fkey FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT extratos_bancarios_tipo_cartao_id_fkey FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios ADD CONSTRAINT extratos_bancarios_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_banco_id_fkey FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_categoria_id_fkey FOREIGN KEY (categoria_id) REFERENCES categorias (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_extrato_bancario_id_fkey FOREIGN KEY (extrato_bancario_id) REFERENCES extratos_bancarios (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_pessoa_movimentacao_id_fkey FOREIGN KEY (pessoa_movimentacao_id) REFERENCES pessoas_movimentacao (id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_tipo_cartao_id_fkey FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_tipo_movimentacao_id_fkey FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE extratos_bancarios_itens ADD CONSTRAINT extratos_bancarios_itens_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_lista_id_fkey FOREIGN KEY (lista_id) REFERENCES listas (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE itens_lista ADD CONSTRAINT itens_lista_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE listas ADD CONSTRAINT listas_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_banco_id_fkey FOREIGN KEY (banco_id) REFERENCES bancos (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_tipo_cartao_id_fkey FOREIGN KEY (tipo_cartao_id) REFERENCES tipos_cartao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_tipo_movimentacao_id_fkey FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE movimentacoes_diarias ADD CONSTRAINT movimentacoes_diarias_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT pessoas_movimentacao_categoria_id_fkey FOREIGN KEY (categoria_id) REFERENCES categorias (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT pessoas_movimentacao_tipo_movimentacao_id_fkey FOREIGN KEY (tipo_movimentacao_id) REFERENCES tipos_movimentacao (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE pessoas_movimentacao ADD CONSTRAINT pessoas_movimentacao_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_cartao ADD CONSTRAINT tipos_cartao_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE tipos_movimentacao ADD CONSTRAINT tipos_movimentacao_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_categoria_id_fkey FOREIGN KEY (categoria_id) REFERENCES categorias (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_conta_id_fkey FOREIGN KEY (conta_id) REFERENCES contas (id) ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    ALTER TABLE transacoes ADD CONSTRAINT transacoes_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES asp_net_users (id) ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20261021182221_ConvencoesDaSpec';
    END IF;
END $EF$;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE tipos_movimentacao ALTER COLUMN nome_tipo_movimentacao TYPE character varying(80);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE tipos_cartao ALTER COLUMN nome_tipo_cartao TYPE character varying(50);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE movimentacoes_diarias ALTER COLUMN descricao TYPE character varying(300);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE listas ALTER COLUMN nome TYPE character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE extratos_bancarios_itens ALTER COLUMN descricao TYPE character varying(300);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE extratos_bancarios ALTER COLUMN situacao TYPE character varying(30);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE extratos_bancarios ALTER COLUMN nome_arquivo_origem TYPE character varying(255);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE extratos_bancarios ALTER COLUMN nome_arquivo TYPE character varying(255);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE contas ALTER COLUMN data_operacao TYPE character varying(10);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE categorias ALTER COLUMN subcategoria TYPE character varying(80);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE categorias ALTER COLUMN nome TYPE character varying(80);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE bancos ALTER COLUMN numero_conta TYPE character varying(30);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE bancos ALTER COLUMN nome_banco TYPE character varying(80);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE bancos ALTER COLUMN agencia TYPE character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE asp_net_users ALTER COLUMN phone_number TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    ALTER TABLE asp_net_users ALTER COLUMN nome_usuario TYPE character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20261021174951_AjusteLimitesDeTexto';
    END IF;
END $EF$;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_role_claims DROP CONSTRAINT asp_net_role_claims_role_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_claims DROP CONSTRAINT asp_net_user_claims_user_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_logins DROP CONSTRAINT asp_net_user_logins_user_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_roles DROP CONSTRAINT asp_net_user_roles_role_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_roles DROP CONSTRAINT asp_net_user_roles_user_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_tokens DROP CONSTRAINT asp_net_user_tokens_user_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos DROP CONSTRAINT bancos_tipo_cartao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos DROP CONSTRAINT bancos_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_categoria_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT contas_vencimento_conta_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT contas_vencimento_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT extratos_bancarios_banco_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT extratos_bancarios_tipo_cartao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT extratos_bancarios_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_banco_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_categoria_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_extrato_bancario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_pessoa_movimentacao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_tipo_cartao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_tipo_movimentacao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_lista_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas DROP CONSTRAINT listas_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_banco_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_tipo_cartao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_tipo_movimentacao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT pessoas_movimentacao_categoria_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT pessoas_movimentacao_tipo_movimentacao_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT pessoas_movimentacao_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao DROP CONSTRAINT tipos_cartao_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT tipos_movimentacao_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_categoria_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_conta_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_usuario_id_fkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes DROP CONSTRAINT transacoes_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT tipos_movimentacao_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao DROP CONSTRAINT tipos_movimentacao_valor_meta_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao DROP CONSTRAINT tipos_cartao_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    DROP INDEX ux_tipos_cartao_sistema_nome_tipo_cartao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao DROP CONSTRAINT pessoas_movimentacao_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias DROP CONSTRAINT movimentacoes_diarias_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas DROP CONSTRAINT listas_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas DROP CONSTRAINT listas_tipo_lista_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_quantidade_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista DROP CONSTRAINT itens_lista_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    DROP INDEX ux_extratos_bancarios_itens_chave_importacao;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens DROP CONSTRAINT extratos_bancarios_itens_tipo_lancamento_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios DROP CONSTRAINT extratos_bancarios_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT contas_vencimento_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento DROP CONSTRAINT contas_vencimento_dia_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_parcelas_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas DROP CONSTRAINT contas_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_natureza_operacao_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias DROP CONSTRAINT categorias_status_check;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos DROP CONSTRAINT bancos_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users DROP CONSTRAINT asp_net_users_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_tokens DROP CONSTRAINT asp_net_user_tokens_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_roles DROP CONSTRAINT asp_net_user_roles_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_logins DROP CONSTRAINT asp_net_user_logins_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_claims DROP CONSTRAINT asp_net_user_claims_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_roles DROP CONSTRAINT asp_net_roles_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_role_claims DROP CONSTRAINT asp_net_role_claims_pkey;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE transacoes RENAME TO "Transactions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_movimentacao RENAME TO "TipoMovimentacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE tipos_cartao RENAME TO "TipoCartao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE pessoas_movimentacao RENAME TO "PessoaMovimentacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE movimentacoes_diarias RENAME TO "MovimentacaoDiaria";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE listas RENAME TO "Lista";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE itens_lista RENAME TO "ItemLista";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios_itens RENAME TO "ExtratoBancarioItem";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE extratos_bancarios RENAME TO "ExtratoBancario";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas_vencimento RENAME TO "ContaVencimento";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE contas RENAME TO "Accounts";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE categorias RENAME TO "Categories";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE bancos RENAME TO "Banco";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_users RENAME TO "AspNetUsers";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_tokens RENAME TO "AspNetUserTokens";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_roles RENAME TO "AspNetUserRoles";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_logins RENAME TO "AspNetUserLogins";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_user_claims RENAME TO "AspNetUserClaims";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_roles RENAME TO "AspNetRoles";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE asp_net_role_claims RENAME TO "AspNetRoleClaims";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN status TO "Status";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN observacao TO "Observacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN quantidade_parcelas TO "QuantidadeParcelas";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN parcela_atual TO "ParcelaAtual";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN eh_parcelado TO "EhParcelado";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN data_criacao TO "DataCriacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN valor TO "Value";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN nome TO "Name";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN data TO "Date";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN conta_id TO "IdAccount";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" RENAME COLUMN categoria_id TO "CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_transacoes_usuario_id_data RENAME TO "IX_Transactions_UserId_Date";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_transacoes_conta_id RENAME TO "IX_Transactions_IdAccount";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_transacoes_categoria_id RENAME TO "IX_Transactions_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" RENAME COLUMN descricao TO "Descricao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" RENAME COLUMN valor_meta TO "ValorMeta";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" RENAME COLUMN nome_tipo_movimentacao TO "NomeTipoMovimentacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_tipos_movimentacao_usuario_id RENAME TO "IX_TipoMovimentacao_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" RENAME COLUMN descricao TO "Descricao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" RENAME COLUMN nome_tipo_cartao TO "NomeTipoCartao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" RENAME COLUMN eh_credito TO "EhCredito";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ux_tipos_cartao_usuario_id_nome_tipo_cartao RENAME TO "UX_TipoCartao_UserId_NomeTipoCartao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" RENAME COLUMN tipo_movimentacao_id TO "TipoMovimentacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" RENAME COLUMN nome_pessoa TO "NomePessoa";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" RENAME COLUMN categoria_id TO "CategoriaId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ux_pessoas_movimentacao_usuario_id_nome_pessoa RENAME TO "UX_PessoaMovimentacao_UserId_NomePessoa";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_pessoas_movimentacao_tipo_movimentacao_id RENAME TO "IX_PessoaMovimentacao_TipoMovimentacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_pessoas_movimentacao_categoria_id RENAME TO "IX_PessoaMovimentacao_CategoriaId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN valor TO "Valor";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN identificador TO "Identificador";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN descricao TO "Descricao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN tipo_movimentacao_id TO "TipoMovimentacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN tipo_lancamento TO "TipoLancamento";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN tipo_cartao_id TO "TipoCartaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN nome_pessoa_transacao TO "NomePessoaTransacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN data_movimentacao TO "DataMovimentacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN banco_id TO "BancoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_movimentacoes_diarias_usuario_id RENAME TO "IX_MovimentacaoDiaria_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_movimentacoes_diarias_tipo_movimentacao_id RENAME TO "IX_MovimentacaoDiaria_TipoMovimentacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_movimentacoes_diarias_tipo_cartao_id RENAME TO "IX_MovimentacaoDiaria_TipoCartaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_movimentacoes_diarias_banco_id RENAME TO "IX_MovimentacaoDiaria_BancoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" RENAME COLUMN status TO "Status";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" RENAME COLUMN nome TO "Nome";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" RENAME COLUMN tipo_lista TO "TipoLista";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_listas_usuario_id RENAME TO "IX_Lista_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN valor TO "Valor";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN status TO "Status";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN quantidade TO "Quantidade";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN observacao TO "Observacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN descricao TO "Descricao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN lista_id TO "ListaId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN horario_tarefa TO "HorarioTarefa";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN data_tarefa TO "DataTarefa";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_itens_lista_usuario_id RENAME TO "IX_ItemLista_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_itens_lista_lista_id RENAME TO "IX_ItemLista_ListaId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN valor TO "Valor";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN observacao TO "Observacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN identificador TO "Identificador";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN descricao TO "Descricao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN tipo_movimentacao_id TO "TipoMovimentacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN tipo_lancamento TO "TipoLancamento";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN tipo_cartao_id TO "TipoCartaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN quantidade_parcelas TO "QuantidadeParcelas";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN pessoa_movimentacao_id TO "PessoaMovimentacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN parcela_atual TO "ParcelaAtual";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN numero_fatura TO "NumeroFatura";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN nome_pessoa_transacao TO "NomePessoaTransacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN grupo_parcelamento TO "GrupoParcelamento";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN extrato_bancario_id TO "ExtratoBancarioId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN eh_parcelado TO "EhParcelado";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN data_movimentacao TO "DataMovimentacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN data_criacao TO "DataCriacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN chave_importacao TO "ChaveImportacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN chave_descricao TO "ChaveDescricao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN categoria_id TO "CategoriaId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN banco_id TO "BancoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_itens_usuario_id_numero_fatura RENAME TO "IX_ExtratoBancarioItem_UserId_NumeroFatura";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_itens_usuario_id_data_movimentacao RENAME TO "IX_ExtratoBancarioItem_UserId_DataMovimentacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_itens_tipo_movimentacao_id RENAME TO "IX_ExtratoBancarioItem_TipoMovimentacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_itens_tipo_cartao_id RENAME TO "IX_ExtratoBancarioItem_TipoCartaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_itens_pessoa_movimentacao_id RENAME TO "IX_ExtratoBancarioItem_PessoaMovimentacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_itens_extrato_bancario_id RENAME TO idx_extratobancarioitem_extrato;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_itens_categoria_id RENAME TO "IX_ExtratoBancarioItem_CategoriaId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_itens_banco_id RENAME TO "IX_ExtratoBancarioItem_BancoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN situacao TO "Situacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN valor_total TO "ValorTotal";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN tipo_cartao_id TO "TipoCartaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN quantidade_lancamentos TO "QuantidadeLancamentos";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN nome_arquivo_origem TO "NomeArquivoOrigem";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN nome_arquivo TO "NomeArquivo";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN lote_importacao_id TO "LoteImportacaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN data_inicio_periodo TO "DataInicioPeriodo";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN data_importacao TO "DataImportacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN data_fim_periodo TO "DataFimPeriodo";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN banco_id TO "BancoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_usuario_id RENAME TO "IX_ExtratoBancario_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_tipo_cartao_id RENAME TO "IX_ExtratoBancario_TipoCartaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_extratos_bancarios_banco_id RENAME TO "IX_ExtratoBancario_BancoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" RENAME COLUMN dia TO "Dia";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" RENAME COLUMN conta_id TO "ContaId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_contas_vencimento_usuario_id RENAME TO "IX_ContaVencimento_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_contas_vencimento_conta_id RENAME TO "IX_ContaVencimento_ContaId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN status TO "Status";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN quantidade_parcelas TO "QuantidadeParcelas";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN parcela_atual TO "ParcelaAtual";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN eh_parcelado TO "EhParcelado";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN data_operacao TO "DataOperacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN valor TO "Value";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN nome TO "Name";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" RENAME COLUMN categoria_id TO "CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_contas_usuario_id RENAME TO "IX_Accounts_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_contas_categoria_id RENAME TO "IX_Accounts_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" RENAME COLUMN status TO "Status";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" RENAME COLUMN natureza_operacao TO "NaturezaOperacao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" RENAME COLUMN subcategoria TO "SubCategory";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" RENAME COLUMN nome TO "Name";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ux_categorias_usuario_id_nome_subcategoria RENAME TO "UX_Categories_UserId_Name_SubCategory";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME COLUMN ativo TO "Ativo";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME COLUMN agencia TO "Agencia";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME COLUMN tipo_cartao_id TO "TipoCartaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME COLUMN saldo_inicial TO "SaldoInicial";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME COLUMN numero_conta TO "NumeroConta";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME COLUMN nome_banco TO "NomeBanco";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME COLUMN data_alteracao TO "DataAlteracao";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" RENAME COLUMN usuario_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ux_bancos_usuario_id_nome_banco RENAME TO "UX_Banco_UserId_NomeBanco";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_bancos_tipo_cartao_id RENAME TO "IX_Banco_TipoCartaoId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN role TO "Role";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN email TO "Email";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN user_name TO "UserName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN two_factor_enabled TO "TwoFactorEnabled";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN status_ativo TO "StatusAtivo";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN security_stamp TO "SecurityStamp";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN phone_number_confirmed TO "PhoneNumberConfirmed";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN phone_number TO "PhoneNumber";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN password_hash TO "PasswordHash";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN normalized_user_name TO "NormalizedUserName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN normalized_email TO "NormalizedEmail";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN nome_usuario TO "NomeUsuario";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN lockout_end TO "LockoutEnd";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN lockout_enabled TO "LockoutEnabled";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN email_confirmed TO "EmailConfirmed";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN criado_em TO "CriadoEm";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN concurrency_stamp TO "ConcurrencyStamp";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN atualizado_em TO "AtualizadoEm";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" RENAME COLUMN access_failed_count TO "AccessFailedCount";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX user_name_index RENAME TO "UserNameIndex";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX email_index RENAME TO "EmailIndex";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserTokens" RENAME COLUMN value TO "Value";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserTokens" RENAME COLUMN name TO "Name";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserTokens" RENAME COLUMN login_provider TO "LoginProvider";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserTokens" RENAME COLUMN user_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserRoles" RENAME COLUMN role_id TO "RoleId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserRoles" RENAME COLUMN user_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_asp_net_user_roles_role_id RENAME TO "IX_AspNetUserRoles_RoleId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserLogins" RENAME COLUMN user_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserLogins" RENAME COLUMN provider_display_name TO "ProviderDisplayName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserLogins" RENAME COLUMN provider_key TO "ProviderKey";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserLogins" RENAME COLUMN login_provider TO "LoginProvider";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_asp_net_user_logins_user_id RENAME TO "IX_AspNetUserLogins_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserClaims" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserClaims" RENAME COLUMN user_id TO "UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserClaims" RENAME COLUMN claim_value TO "ClaimValue";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserClaims" RENAME COLUMN claim_type TO "ClaimType";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_asp_net_user_claims_user_id RENAME TO "IX_AspNetUserClaims_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoles" RENAME COLUMN name TO "Name";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoles" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoles" RENAME COLUMN normalized_name TO "NormalizedName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoles" RENAME COLUMN concurrency_stamp TO "ConcurrencyStamp";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX role_name_index RENAME TO "RoleNameIndex";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoleClaims" RENAME COLUMN id TO "Id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoleClaims" RENAME COLUMN role_id TO "RoleId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoleClaims" RENAME COLUMN claim_value TO "ClaimValue";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoleClaims" RENAME COLUMN claim_type TO "ClaimType";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER INDEX ix_asp_net_role_claims_role_id RENAME TO "IX_AspNetRoleClaims_RoleId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT "PK_Transactions" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" ADD CONSTRAINT tipomovimentacao_pkey PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" ADD CONSTRAINT tipocartao_pkey PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" ADD CONSTRAINT "PessoaMovimentacao_pkey" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" ADD CONSTRAINT movimentacaodiaria_pkey PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" ADD CONSTRAINT lista_pkey PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" ADD CONSTRAINT itemlista_pkey PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT "ExtratoBancarioItem_pkey" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" ADD CONSTRAINT "ExtratoBancario_pkey" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" ADD CONSTRAINT "ContaVencimento_pkey" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" ADD CONSTRAINT "PK_Accounts" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" ADD CONSTRAINT "PK_Categories" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" ADD CONSTRAINT banco_pkey PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUsers" ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserTokens" ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserRoles" ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserLogins" ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserClaims" ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoles" ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoleClaims" ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT transactions_parcelas_check CHECK ("QuantidadeParcelas" >= 1 AND "ParcelaAtual" BETWEEN 1 AND "QuantidadeParcelas");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT transactions_status_check CHECK ("Status" IN ('PENDENTE', 'AGUARDANDO', 'PAGO NO PRAZO', 'PAGO ATRASADO'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" ADD CONSTRAINT tipomovimentacao_valormeta_check CHECK ("ValorMeta" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    CREATE UNIQUE INDEX "UX_TipoCartao_Sistema_NomeTipoCartao" ON "TipoCartao" ("NomeTipoCartao") WHERE "UserId" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" ADD CONSTRAINT lista_tipolista_check CHECK ("TipoLista" IN (1, 2, 3));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" ADD CONSTRAINT itemlista_quantidade_check CHECK ("Quantidade" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" ADD CONSTRAINT itemlista_status_check CHECK ("Status" IN ('PENDENTE', 'AGUARDANDO', 'OK'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    CREATE UNIQUE INDEX "UX_ExtratoBancarioItem_ChaveImportacao" ON "ExtratoBancarioItem" ("UserId", "BancoId", "ChaveImportacao") WHERE "ChaveImportacao" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_parcelas_check CHECK ("QuantidadeParcelas" >= 1 AND "ParcelaAtual" BETWEEN 1 AND "QuantidadeParcelas");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_tipolancamento_check CHECK ("TipoLancamento" IN ('Entrada', 'Saída'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" ADD CONSTRAINT contavencimento_dia_check CHECK ("Dia" BETWEEN 1 AND 31);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" ADD CONSTRAINT accounts_parcelas_check CHECK (NOT "EhParcelado" OR ("QuantidadeParcelas" >= 1 AND "ParcelaAtual" BETWEEN 0 AND "QuantidadeParcelas"));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" ADD CONSTRAINT accounts_status_check CHECK ("Status" IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" ADD CONSTRAINT categories_naturezaoperacao_check CHECK ("NaturezaOperacao" BETWEEN 0 AND 5);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" ADD CONSTRAINT categories_status_check CHECK ("Status" IN (0, 1));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" ADD CONSTRAINT "FK_Accounts_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Accounts" ADD CONSTRAINT "FK_Accounts_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetRoleClaims" ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserClaims" ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserLogins" ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserRoles" ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserRoles" ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "AspNetUserTokens" ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" ADD CONSTRAINT "FK_Banco_AspNetUsers_UerId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Banco" ADD CONSTRAINT fk_banco_tipocartao FOREIGN KEY ("TipoCartaoId") REFERENCES "TipoCartao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Categories" ADD CONSTRAINT "FK_Categories_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" ADD CONSTRAINT "ContaVencimento_ContaId_fkey" FOREIGN KEY ("ContaId") REFERENCES "Accounts" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ContaVencimento" ADD CONSTRAINT contavencimento_user_fkey FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" ADD CONSTRAINT "FK_ExtratoBancario_TipoCartao_TipoCartaoId" FOREIGN KEY ("TipoCartaoId") REFERENCES "TipoCartao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" ADD CONSTRAINT extratobancario_bancoid_fkey FOREIGN KEY ("BancoId") REFERENCES "Banco" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancario" ADD CONSTRAINT extratobancario_user_fkey FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT "FK_ExtratoBancarioItem_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT "extratoBancarioItem_bancoId_fkey" FOREIGN KEY ("CategoriaId") REFERENCES "Categories" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_bancoid_fkey FOREIGN KEY ("BancoId") REFERENCES "Banco" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_extrato_fkey FOREIGN KEY ("ExtratoBancarioId") REFERENCES "ExtratoBancario" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_pessoamovimentacaoid_fkey FOREIGN KEY ("PessoaMovimentacaoId") REFERENCES "PessoaMovimentacao" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_tipocartaoid_fkey FOREIGN KEY ("TipoCartaoId") REFERENCES "TipoCartao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ExtratoBancarioItem" ADD CONSTRAINT extratobancarioitem_tipomovimentacaoid_fkey FOREIGN KEY ("TipoMovimentacaoId") REFERENCES "TipoMovimentacao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" ADD CONSTRAINT "FK_ItemLista_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "ItemLista" ADD CONSTRAINT itemlista_listaid_fkey FOREIGN KEY ("ListaId") REFERENCES "Lista" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Lista" ADD CONSTRAINT "FK_Lista_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" ADD CONSTRAINT "FK_MovimentacaoDiaria_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" ADD CONSTRAINT movimentacaodiaria_bancoid_fkey FOREIGN KEY ("BancoId") REFERENCES "Banco" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" ADD CONSTRAINT movimentacaodiaria_tipocartaoid_fkey FOREIGN KEY ("TipoCartaoId") REFERENCES "TipoCartao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "MovimentacaoDiaria" ADD CONSTRAINT movimentacaodiaria_tipomovimentacaoid_fkey FOREIGN KEY ("TipoMovimentacaoId") REFERENCES "TipoMovimentacao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" ADD CONSTRAINT "FK_PessoaMovimentacao_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" ADD CONSTRAINT "FK_PessoaMovimentacao_Categories_CategoriaId" FOREIGN KEY ("CategoriaId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "PessoaMovimentacao" ADD CONSTRAINT "FK_PessoaMovimentacao_TipoMovimentacao_TipoMovimentacaoId" FOREIGN KEY ("TipoMovimentacaoId") REFERENCES "TipoMovimentacao" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoCartao" ADD CONSTRAINT fk_tipocartao_aspnetusers FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "TipoMovimentacao" ADD CONSTRAINT fk_tipomovimentacao_aspnetusers FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT "FK_Transactions_Accounts_IdAccount" FOREIGN KEY ("IdAccount") REFERENCES "Accounts" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT "FK_Transactions_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    ALTER TABLE "Transactions" ADD CONSTRAINT "FK_Transactions_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20261021132335_ConvencoesDeNomes';
    END IF;
END $EF$;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Transactions" ALTER COLUMN "Status" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Transactions" ALTER COLUMN "Observacao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Transactions" ALTER COLUMN "Name" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "TipoMovimentacao" ALTER COLUMN "NomeTipoMovimentacao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "TipoMovimentacao" ALTER COLUMN "Descricao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "TipoCartao" ALTER COLUMN "NomeTipoCartao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "TipoCartao" ALTER COLUMN "Descricao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "PessoaMovimentacao" ALTER COLUMN "NomePessoa" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "MovimentacaoDiaria" ALTER COLUMN "TipoLancamento" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "MovimentacaoDiaria" ALTER COLUMN "NomePessoaTransacao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "MovimentacaoDiaria" ALTER COLUMN "Identificador" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "MovimentacaoDiaria" ALTER COLUMN "Descricao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Lista" ALTER COLUMN "Nome" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ItemLista" ALTER COLUMN "Status" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ItemLista" ALTER COLUMN "Observacao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "TipoLancamento" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "Observacao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "NomePessoaTransacao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "Identificador" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancarioItem" ALTER COLUMN "Descricao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancario" ALTER COLUMN "NomeArquivoOrigem" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "ExtratoBancario" ALTER COLUMN "NomeArquivo" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Categories" ALTER COLUMN "SubCategory" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Categories" ALTER COLUMN "Name" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Banco" ALTER COLUMN "NumeroConta" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Banco" ALTER COLUMN "NomeBanco" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Banco" ALTER COLUMN "Agencia" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "Role" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "NomeUsuario" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Accounts" ALTER COLUMN "Name" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    ALTER TABLE "Accounts" ALTER COLUMN "DataOperacao" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261020135241_LimitesDeTexto') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20261020135241_LimitesDeTexto';
    END IF;
END $EF$;
COMMIT;

