CREATE OR ALTER PROCEDURE prc_add_material
    @nome VARCHAR(100),
    @quantidade INT,
    @medida VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.materiais(nome, quantidade, medida)
    VALUES (@nome, @quantidade, @medida);
    SELECT SCOPE_IDENTITY() AS id;
END;
GO

CREATE OR ALTER PROCEDURE prc_update_material
    @id INT,
    @nome VARCHAR(100),
    @quantidade INT,
    @medida VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.materiais
    SET nome = @nome,
        quantidade = @quantidade,
        medida = @medida
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_remove_material
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.materiais
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_get_material
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.materiais
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_list_low_stock_materials
    @threshold INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.materiais
    WHERE quantidade <= @threshold
    ORDER BY quantidade ASC;
END;
GO

CREATE OR ALTER PROCEDURE prc_adjust_material_stock
    @material_id INT,
    @delta INT,
    @tipo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.materiais
    SET quantidade = quantidade + @delta
    WHERE id = @material_id;

    INSERT INTO dbo.historico_estoque(quantidade, tipo, materiais_id)
    VALUES (CONVERT(VARCHAR(50), @delta), @tipo, @material_id);
END;
GO
