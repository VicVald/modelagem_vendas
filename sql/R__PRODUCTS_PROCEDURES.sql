CREATE OR ALTER PROCEDURE prc_add_product
    @valor DECIMAL(12,2),
    @materiais_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.produtos(valor, materiais_id)
    VALUES (@valor, @materiais_id);
END;
GO

CREATE OR ALTER PROCEDURE prc_update_product
    @id INT,
    @valor DECIMAL(12,2),
    @materiais_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.produtos
    SET valor = @valor,
        materiais_id = @materiais_id
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_remove_product
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.produtos
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_get_product
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.produtos
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_list_products_by_material
    @material_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.*
    FROM dbo.produtos p
    JOIN dbo.materiais_produtos mp ON mp.produtos_id = p.id
    WHERE mp.materiais_id = @material_id;
END;
GO
