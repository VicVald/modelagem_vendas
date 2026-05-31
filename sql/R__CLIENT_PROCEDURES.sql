CREATE OR ALTER PROCEDURE prc_add_client
    @nome VARCHAR(60),
    @cpf VARCHAR(60)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO clientes(nome,cpf) VALUES (@nome, dbo.fn_hash_cpf(@cpf));
END;