CREATE OR ALTER PROCEDURE prc_link_material_product
    @materiais_id INT,
    @produtos_id INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.materiais_produtos(materiais_id, produtos_id)
    VALUES (@materiais_id, @produtos_id);
END;
GO

CREATE OR ALTER PROCEDURE prc_unlink_material_product
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.materiais_produtos
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_list_product_materials
    @produto_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.*
    FROM dbo.materiais m
    JOIN dbo.materiais_produtos mp ON mp.materiais_id = m.id
    WHERE mp.produtos_id = @produto_id;
END;
GO
