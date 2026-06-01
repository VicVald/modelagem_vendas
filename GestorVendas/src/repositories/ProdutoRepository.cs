using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using modelagem_vendas.src.models;
using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public ProdutoRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("DefaultConnection connection string not found.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<int> AddProdutoAsync(decimal valor, int? materiaisId)
    {
        using var db = CreateConnection();
        var parameters = new { valor, materiais_id = materiaisId };
        return await db.QuerySingleAsync<int>("prc_add_product", parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Produto>> GetAllProdutosAsync()
    {
        using var db = CreateConnection();
        return await db.QueryAsync<Produto>("SELECT id, valor, materiais_id FROM dbo.produtos");
    }

    public async Task<Produto?> GetProdutoByIdAsync(int id)
    {
        using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Produto>("prc_get_product", new { id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateProdutoAsync(int id, decimal valor, int? materiaisId)
    {
        using var db = CreateConnection();
        var affected = await db.ExecuteAsync("prc_update_product", new { id, valor, materiais_id = materiaisId }, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }

    public async Task<bool> DeleteProdutoAsync(int id)
    {
        using var db = CreateConnection();
        var affected = await db.ExecuteAsync("prc_remove_product", new { id }, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }

    public async Task LinkMaterialProdutoAsync(int materialId, int produtoId)
    {
        using var db = CreateConnection();
        var parameters = new { materiais_id = materialId, produtos_id = produtoId };
        await db.ExecuteAsync("prc_link_material_product", parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task UnlinkMaterialProdutoAsync(int relationId)
    {
        using var db = CreateConnection();
        await db.ExecuteAsync("prc_unlink_material_product", new { id = relationId }, commandType: CommandType.StoredProcedure);
    }

    public async Task RemoveMaterialProdutoAsync(int produtoId, int materialId)
    {
        using var db = CreateConnection();
        await db.ExecuteAsync("DELETE FROM dbo.materiais_produtos WHERE produtos_id = @produtoId AND materiais_id = @materialId", new { produtoId, materialId });
    }

    public async Task<IEnumerable<Material>> GetMateriaisByProdutoAsync(int produtoId)
    {
        using var db = CreateConnection();
        return await db.QueryAsync<Material>("prc_list_product_materials", new { produto_id = produtoId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Produto>> GetProductsByMaterialAsync(int materialId)
    {
        using var db = CreateConnection();
        return await db.QueryAsync<Produto>("prc_list_products_by_material", new { material_id = materialId }, commandType: CommandType.StoredProcedure);
    }
}
