-- V2__SEED_DATA.sql
-- Seed script with loops and random values to populate the database

-- Clean existing data to ensure a fresh, full seed
DELETE FROM dbo.historico_estoque;
DELETE FROM dbo.itens_pedidos;
DELETE FROM dbo.pedidos;
DELETE FROM dbo.clientes;
DELETE FROM dbo.materiais_produtos;
DELETE FROM dbo.produtos;
DELETE FROM dbo.materiais;

-- Reset identity columns
DBCC CHECKIDENT ('dbo.clientes', RESEED, 0);
DBCC CHECKIDENT ('dbo.pedidos', RESEED, 0);
DBCC CHECKIDENT ('dbo.materiais', RESEED, 0);
DBCC CHECKIDENT ('dbo.produtos', RESEED, 0);
DBCC CHECKIDENT ('dbo.itens_pedidos', RESEED, 0);
DBCC CHECKIDENT ('dbo.materiais_produtos', RESEED, 0);
DBCC CHECKIDENT ('dbo.historico_estoque', RESEED, 0);
GO

-- 1. Seed Materiais (Ingredients)
INSERT INTO dbo.materiais (nome, quantidade, medida) VALUES
('Farinha de Trigo', 50000, 'g'),
('Açúcar', 30000, 'g'),
('Manteiga', 10000, 'g'),
('Ovos', 200, 'un'),
('Leite', 20000, 'ml'),
('Fermento', 1000, 'g'),
('Chocolate em Pó', 5000, 'g'),
('Sal', 2000, 'g'),
('Queijo Mussarela', 15000, 'g'),
('Presunto', 10000, 'g'),
('Molho de Tomate', 5000, 'ml'),
('Orégano', 500, 'g'),
('Calabresa', 8000, 'g'),
('Essência de Baunilha', 250, 'ml'),
('Cebola', 3000, 'g');
GO

-- 2. Seed Produtos
DECLARE @ProdCount INT = 1;
DECLARE @MinMaterialId INT = (SELECT MIN(id) FROM dbo.materiais);
DECLARE @MaxMaterialId INT = (SELECT MAX(id) FROM dbo.materiais);
DECLARE @MaterialDiff INT = @MaxMaterialId - @MinMaterialId + 1;

WHILE @ProdCount <= 15
BEGIN
    DECLARE @Valor DECIMAL(12,2) = CAST((RAND() * 45.0) + 5.0 AS DECIMAL(12,2));
    -- Pick a random material for the direct link or leave it NULL sometimes
    DECLARE @DirectMaterialId INT = NULL;
    IF RAND() > 0.3
    BEGIN
        SET @DirectMaterialId = @MinMaterialId + ABS(CHECKSUM(NEWID())) % @MaterialDiff;
    END

    INSERT INTO dbo.produtos (valor, materiais_id)
    VALUES (@Valor, @DirectMaterialId);

    DECLARE @NewProdutoId INT = SCOPE_IDENTITY();

    -- Add 1 to 4 random ingredients in materiais_produtos
    DECLARE @IngredCount INT = 1;
    DECLARE @NumIngredients INT = CAST((RAND() * 3) + 1 AS INT);
    DECLARE @LastMaterialId INT = -1;

    WHILE @IngredCount <= @NumIngredients
    BEGIN
        DECLARE @RandomMaterialId INT = @MinMaterialId + ABS(CHECKSUM(NEWID())) % @MaterialDiff;
        
        -- Avoid adding duplicate ingredients to the same product
        IF @RandomMaterialId <> @LastMaterialId AND NOT EXISTS (
            SELECT 1 FROM dbo.materiais_produtos 
            WHERE produtos_id = @NewProdutoId AND materiais_id = @RandomMaterialId
        )
        BEGIN
            INSERT INTO dbo.materiais_produtos (materiais_id, produtos_id)
            VALUES (@RandomMaterialId, @NewProdutoId);
            SET @IngredCount = @IngredCount + 1;
        END
        SET @LastMaterialId = @RandomMaterialId;
    END

    SET @ProdCount = @ProdCount + 1;
END
GO

-- 3. Seed Clientes
DECLARE @FirstNames TABLE (ID INT IDENTITY(1,1), Name VARCHAR(30));
DECLARE @LastNames TABLE (ID INT IDENTITY(1,1), Name VARCHAR(30));

INSERT INTO @FirstNames (Name) VALUES 
('Victor'), ('Ana'), ('Lucas'), ('Mariana'), ('Gabriel'), ('Beatriz'), 
('Felipe'), ('Juliana'), ('Mateus'), ('Larissa'), ('Rodrigo'), ('Camila'), 
('Diego'), ('Letícia'), ('Thiago'), ('Bruna'), ('Gustavo'), ('Isabela');

INSERT INTO @LastNames (Name) VALUES 
('Silva'), ('Santos'), ('Oliveira'), ('Souza'), ('Rodrigues'), ('Ferreira'), 
('Alves'), ('Pereira'), ('Lima'), ('Gomes'), ('Costa'), ('Ribeiro'), 
('Martins'), ('Carvalho'), ('Almeida'), ('Lopes'), ('Soares'), ('Vieira');

DECLARE @ClientCount INT = 1;
WHILE @ClientCount <= 25
BEGIN
    -- Evaluate random IDs before the SELECT statement to prevent multiple evaluations
    DECLARE @RandFirstId INT = 1 + ABS(CHECKSUM(NEWID())) % 18;
    DECLARE @RandLastId INT = 1 + ABS(CHECKSUM(NEWID())) % 18;

    DECLARE @FirstName VARCHAR(30) = (SELECT Name FROM @FirstNames WHERE ID = @RandFirstId);
    DECLARE @LastName VARCHAR(30) = (SELECT Name FROM @LastNames WHERE ID = @RandLastId);
    DECLARE @NomeCompleto VARCHAR(60) = @FirstName + ' ' + @LastName;

    -- Create a unique 11 digit CPF format string
    DECLARE @RawCpf VARCHAR(11) = RIGHT('0000000000' + CAST(100000000 + @ClientCount * 3333333 AS VARCHAR(11)), 11);

    -- Insert client using hashed CPF (since that's the pattern defined)
    INSERT INTO dbo.clientes (nome, cpf)
    VALUES (@NomeCompleto, dbo.fn_hash_cpf(@RawCpf));

    SET @ClientCount = @ClientCount + 1;
END
GO

-- 4. Seed Pedidos & Itens de Pedido
DECLARE @MinClientId INT = (SELECT MIN(id) FROM dbo.clientes);
DECLARE @MaxClientId INT = (SELECT MAX(id) FROM dbo.clientes);
DECLARE @ClientDiff INT = @MaxClientId - @MinClientId + 1;

DECLARE @MinProdutoId INT = (SELECT MIN(id) FROM dbo.produtos);
DECLARE @MaxProdutoId INT = (SELECT MAX(id) FROM dbo.produtos);
DECLARE @ProdutoDiff INT = @MaxProdutoId - @MinProdutoId + 1;

DECLARE @PedidoIndex INT = 1;
WHILE @PedidoIndex <= 80
BEGIN
    DECLARE @RandClientId INT = @MinClientId + ABS(CHECKSUM(NEWID())) % @ClientDiff;
    -- Create orders spread over the last 30 days
    DECLARE @RandDaysAgo INT = ABS(CHECKSUM(NEWID())) % 30;
    DECLARE @OrderDate DATETIME = DATEADD(DAY, -@RandDaysAgo, GETDATE());

    -- Initial insert with 0 value
    INSERT INTO dbo.pedidos (cliente_id, valor_total, created_at)
    VALUES (@RandClientId, 0.00, @OrderDate);

    DECLARE @NewPedidoId INT = SCOPE_IDENTITY();

    -- Insert 1 to 5 random products for this order
    DECLARE @ItemCount INT = 1;
    DECLARE @NumItems INT = CAST((RAND() * 4) + 1 AS INT);
    DECLARE @PedidoValorTotal DECIMAL(12,2) = 0.00;

    WHILE @ItemCount <= @NumItems
    BEGIN
        DECLARE @RandProdutoId INT = @MinProdutoId + ABS(CHECKSUM(NEWID())) % @ProdutoDiff;
        DECLARE @ProdValor DECIMAL(12,2) = (SELECT valor FROM dbo.produtos WHERE id = @RandProdutoId);

        INSERT INTO dbo.itens_pedidos (pedido_id, produto_id, created_at)
        VALUES (@NewPedidoId, @RandProdutoId, @OrderDate);

        SET @PedidoValorTotal = @PedidoValorTotal + @ProdValor;
        SET @ItemCount = @ItemCount + 1;
    END

    -- Update the order's total value
    UPDATE dbo.pedidos
    SET valor_total = @PedidoValorTotal
    WHERE id = @NewPedidoId;

    SET @PedidoIndex = @PedidoIndex + 1;
END
GO

-- 5. Seed Historico Estoque
DECLARE @MinMatId INT = (SELECT MIN(id) FROM dbo.materiais);
DECLARE @MaxMatId INT = (SELECT MAX(id) FROM dbo.materiais);
DECLARE @MatDiff INT = @MaxMatId - @MinMatId + 1;

DECLARE @StockIndex INT = 1;
WHILE @StockIndex <= 40
BEGIN
    DECLARE @TargetMaterialId INT = @MinMatId + ABS(CHECKSUM(NEWID())) % @MatDiff;
    DECLARE @TipoMovimentacao VARCHAR(50) = CASE WHEN RAND() > 0.4 THEN 'Entrada' ELSE 'Saída' END;
    DECLARE @QuantMovimentada INT = CAST((RAND() * 500) + 50 AS INT);

    INSERT INTO dbo.historico_estoque (quantidade, tipo, materiais_id, created_at)
    VALUES (CAST(@QuantMovimentada AS VARCHAR(50)), @TipoMovimentacao, @TargetMaterialId, DATEADD(DAY, -CAST(RAND() * 20 AS INT), GETDATE()));

    SET @StockIndex = @StockIndex + 1;
END
GO
