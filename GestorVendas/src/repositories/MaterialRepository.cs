using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using modelagem_vendas.src.models;
using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public MaterialRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("DefaultConnection connection string not found.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<int> AddMaterialAsync(string nome, int quantidade, string medida)
    {
        using var db = CreateConnection();
        var parameters = new { nome, quantidade, medida };
        return await db.QuerySingleAsync<int>("prc_add_material", parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Material>> GetAllMateriaisAsync()
    {
        using var db = CreateConnection();
        return await db.QueryAsync<Material>("SELECT id, nome, quantidade, medida FROM dbo.materiais");
    }

    public async Task<Material?> GetMaterialByIdAsync(int id)
    {
        using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Material>("prc_get_material", new { id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateMaterialAsync(int id, string nome, int quantidade, string medida)
    {
        using var db = CreateConnection();
        var affected = await db.ExecuteAsync("prc_update_material", new { id, nome, quantidade, medida }, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }

    public async Task<bool> DeleteMaterialAsync(int id)
    {
        using var db = CreateConnection();
        var affected = await db.ExecuteAsync("prc_remove_material", new { id }, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }

    public async Task<IEnumerable<Material>> GetLowStockMaterialsAsync(int threshold)
    {
        using var db = CreateConnection();
        return await db.QueryAsync<Material>("prc_list_low_stock_materials", new { threshold }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> AdjustMaterialStockAsync(int materialId, int delta, string tipo)
    {
        using var db = CreateConnection();
        var affected = await db.ExecuteAsync("prc_adjust_material_stock", new { material_id = materialId, delta, tipo }, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }
}
