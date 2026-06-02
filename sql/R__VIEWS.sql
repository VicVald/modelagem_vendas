CREATE OR ALTER VIEW dbo.vw_melhores_produtos_quantidade AS
SELECT 
    p.id AS produto_id,
    p.valor AS produto_valor,
    COUNT(ip.id) AS total_vendas
FROM dbo.produtos p
JOIN dbo.itens_pedidos ip ON p.id = ip.produto_id
GROUP BY p.id, p.valor;
GO

CREATE OR ALTER VIEW dbo.vw_melhores_produtos_valor AS
SELECT 
    p.id AS produto_id,
    p.valor AS produto_valor,
    COUNT(ip.id) AS total_vendas,
    SUM(p.valor) AS valor_total_recebido
FROM dbo.produtos p
JOIN dbo.itens_pedidos ip ON p.id = ip.produto_id
GROUP BY p.id, p.valor;
GO

CREATE OR ALTER VIEW dbo.vw_ingredientes_mais_utilizados AS
WITH RelacaoProdutoMaterial AS (
    -- Relacionamento direto
    SELECT id AS produto_id, materiais_id AS material_id
    FROM dbo.produtos
    WHERE materiais_id IS NOT NULL
    UNION
    -- Relacionamento muitos-para-muitos
    SELECT produtos_id AS produto_id, materiais_id AS material_id
    FROM dbo.materiais_produtos
)
SELECT 
    m.id AS material_id,
    m.nome AS material_nome,
    m.medida AS material_medida,
    COUNT(ip.id) AS total_utilizacoes
FROM dbo.materiais m
JOIN RelacaoProdutoMaterial rpm ON m.id = rpm.material_id
JOIN dbo.itens_pedidos ip ON rpm.produto_id = ip.produto_id
GROUP BY m.id, m.nome, m.medida;
GO
