using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using modelagem_vendas.src.models;
using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public ClienteRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("DefaultConnection connection string not found.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<int> AddClientAsync(string nome, string cpf)
    {
        using var db = CreateConnection();
        var parameters = new { nome, cpf };
        return await db.QuerySingleAsync<int>("prc_add_client", parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
    {
        using var db = CreateConnection();
        return await db.QueryAsync<Cliente>("SELECT id, nome, '[PROTEGIDO]' AS cpf, created_at FROM dbo.clientes");
    }

    public async Task<Cliente?> GetClienteByIdAsync(int id)
    {
        using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Cliente>("prc_get_client", new { id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Cliente?> GetClienteByCpfAsync(string cpf)
    {
        using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Cliente>(
            "SELECT id, nome, '[PROTEGIDO]' AS cpf, created_at FROM dbo.clientes WHERE cpf = dbo.fn_hash_cpf(@cpf)", 
            new { cpf }
        );
    }

    public async Task<bool> UpdateClienteAsync(int id, string nome)
    {
        using var db = CreateConnection();
        var affected = await db.ExecuteAsync("prc_update_client", new { id, nome }, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }

    public async Task<bool> DeleteClienteAsync(int id)
    {
        using var db = CreateConnection();
        var affected = await db.ExecuteAsync("prc_remove_client", new { id }, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }
}
