CREATE OR ALTER PROCEDURE prc_add_order
    @cliente_id INT,
    @valor_total DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.pedidos(cliente_id, valor_total)
    VALUES (@cliente_id, @valor_total);
    SELECT SCOPE_IDENTITY() AS id;
END;
GO

CREATE OR ALTER PROCEDURE prc_update_order_total
    @id INT,
    @valor_total DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.pedidos
    SET valor_total = @valor_total
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_remove_order
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.pedidos
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_get_order
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.pedidos
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_list_orders_by_client
    @cliente_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.pedidos
    WHERE cliente_id = @cliente_id
    ORDER BY created_at DESC;
END;
GO

CREATE OR ALTER PROCEDURE prc_get_sales_summary
    @start_date DATETIME,
    @end_date DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        CONVERT(DATE, created_at) AS data,
        COUNT(*) AS total_pedidos,
        SUM(valor_total) AS total_vendas
    FROM dbo.pedidos
    WHERE created_at BETWEEN @start_date AND @end_date
    GROUP BY CONVERT(DATE, created_at)
    ORDER BY data;
END;
GO
