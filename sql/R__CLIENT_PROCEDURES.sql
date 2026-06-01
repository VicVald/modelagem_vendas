CREATE OR ALTER PROCEDURE prc_add_client
    @nome VARCHAR(60),
    @cpf VARCHAR(60)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.clientes(nome, cpf)
    VALUES (@nome, dbo.fn_hash_cpf(@cpf));
    SELECT SCOPE_IDENTITY() AS id;
END;
GO

CREATE OR ALTER PROCEDURE prc_update_client
    @id INT,
    @nome VARCHAR(60)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.clientes
    SET nome = @nome
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_remove_client
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.clientes
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_get_client
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.clientes
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_list_client_orders
    @cliente_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.pedidos
    WHERE cliente_id = @cliente_id
    ORDER BY created_at DESC;
END;
GO
