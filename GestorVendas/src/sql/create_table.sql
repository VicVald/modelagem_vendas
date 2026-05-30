
CREATE TABLE clientes
(
    id UUID PRIMARY KEY,

    nome VARCHAR(100) NOT NULL,

    email VARCHAR(200) UNIQUE NOT NULL,

    cpf_hash TEXT NOT NULL,

    data_cadastro TIMESTAMP NOT NULL
);