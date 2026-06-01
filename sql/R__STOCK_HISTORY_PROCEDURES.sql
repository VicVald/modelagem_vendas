CREATE OR ALTER PROCEDURE prc_add_stock_history
    @material_id INT,
    @quantidade VARCHAR(50),
    @tipo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.historico_estoque(quantidade, tipo, materiais_id)
    VALUES (@quantidade, @tipo, @material_id);
END;
GO

CREATE OR ALTER PROCEDURE prc_list_stock_history
    @material_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.historico_estoque
    WHERE materiais_id = @material_id
    ORDER BY created_at DESC;
END;
GO

CREATE OR ALTER PROCEDURE prc_get_stock_balance
    @material_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT quantidade FROM dbo.materiais
    WHERE id = @material_id;
END;
GO

CREATE OR ALTER PROCEDURE prc_get_stock_movement_report
    @start_date DATETIME,
    @end_date DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        he.materiais_id,
        m.nome AS material_nome,
        he.tipo,
        he.quantidade,
        he.created_at
    FROM dbo.historico_estoque he
    JOIN dbo.materiais m ON m.id = he.materiais_id
    WHERE he.created_at BETWEEN @start_date AND @end_date
    ORDER BY he.created_at DESC;
END;
GO
