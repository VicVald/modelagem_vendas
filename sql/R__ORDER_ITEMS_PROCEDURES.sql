CREATE OR ALTER PROCEDURE prc_add_order_item
    @pedido_id INT,
    @produto_id INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.itens_pedidos(pedido_id, produto_id)
    VALUES (@pedido_id, @produto_id);
    SELECT SCOPE_IDENTITY() AS id;
END;
GO

CREATE OR ALTER PROCEDURE prc_remove_order_item
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.itens_pedidos
    WHERE id = @id;
END;
GO

CREATE OR ALTER PROCEDURE prc_list_order_items
    @pedido_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ip.*, p.valor, p.materiais_id
    FROM dbo.itens_pedidos ip
    JOIN dbo.produtos p ON p.id = ip.produto_id
    WHERE ip.pedido_id = @pedido_id;
END;
GO

CREATE OR ALTER PROCEDURE prc_get_order_details
    @pedido_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        o.id AS pedido_id,
        o.cliente_id,
        c.nome AS cliente_nome,
        o.valor_total,
        o.created_at AS pedido_data,
        ip.id AS item_id,
        p.id AS produto_id,
        p.valor AS produto_valor,
        p.materiais_id
    FROM dbo.pedidos o
    JOIN dbo.clientes c ON c.id = o.cliente_id
    JOIN dbo.itens_pedidos ip ON ip.pedido_id = o.id
    JOIN dbo.produtos p ON p.id = ip.produto_id
    WHERE o.id = @pedido_id;
END;
GO
