# Comandos Úteis

Este arquivo armazena comandos úteis para interagir com o ambiente de desenvolvimento.

## Acessar o Banco de Dados com Docker

```bash
docker exec -it gestorvendas_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SuaSenhaSuperForte123!" -C
```

## Ver Logs do Banco de Dados

```bash
docker logs -f gestorvendas_sqlserver
```

## Acessar o container do banco via bash

```bash
docker exec -it gestorvendas_sqlserver bash
```

## Executar Reparo do Flyway (Alinhamento de Metadados)

```bash
docker compose run --rm flyway repair
```

## Executar Migrações do Flyway Manualmente

```bash
docker compose run --rm flyway migrate
```

