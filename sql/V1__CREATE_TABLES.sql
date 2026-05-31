CREATE TABLE clientes (
    id INTEGER PRIMARY KEY IDENTITY(1,1),
    nome VARCHAR(60),
    cpf VARCHAR(60) UNIQUE NOT NULL,
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE pedidos (
    id INTEGER PRIMARY KEY IDENTITY(1,1),
    cliente_id INTEGER NOT NULL,
    valor_total DECIMAL(12,2),
    created_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT fk_clientes_pedidos FOREIGN KEY (cliente_id) REFERENCES clientes(id)
);

CREATE TABLE materiais (
    id INTEGER PRIMARY KEY IDENTITY(1,1),
    nome VARCHAR(100) NOT NULL,
    quantidade INTEGER NOT NULL,
    medida VARCHAR(20),
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE produtos (
    id INTEGER PRIMARY KEY IDENTITY(1,1),
    valor DECIMAL(12,2) NOT NULL,
    materiais_id INTEGER,

    CONSTRAINT fk_produtos_materiais FOREIGN KEY (materiais_id) REFERENCES materiais(id)
);

CREATE TABLE itens_pedidos (
    id INTEGER PRIMARY KEY IDENTITY(1,1),
    pedido_id INTEGER NOT NULL,
    produto_id INTEGER NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT fk_pedidos_itens_pedidos FOREIGN KEY (pedido_id) REFERENCES pedidos(id),
    CONSTRAINT fk_produtos_itens_pedidos FOREIGN KEY (produto_id) REFERENCES produtos(id)
);



CREATE TABLE materiais_produtos (
    id INTEGER PRIMARY KEY IDENTITY(1,1),
    materiais_id INTEGER NOT NULL,
    produtos_id INTEGER NOT NULL,

    CONSTRAINT fk_materiais_produtos_materiais FOREIGN KEY (materiais_id) REFERENCES materiais(id),
    CONSTRAINT fk_materiais_produtos_produtos FOREIGN KEY (produtos_id) REFERENCES produtos(id)
);

CREATE TABLE historico_estoque (
    id INTEGER PRIMARY KEY IDENTITY(1,1),
    quantidade VARCHAR(50) NOT NULL,
    tipo VARCHAR(50) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    materiais_id INTEGER NOT NULL,

    CONSTRAINT fk_historico_materiais FOREIGN KEY (materiais_id) REFERENCES materiais(id)
);