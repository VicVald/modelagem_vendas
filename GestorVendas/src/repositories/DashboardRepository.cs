using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DashboardRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("DefaultConnection connection string not found.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<ProdutoQuantidadeDto>> GetMelhoresProdutosQuantidadeAsync()
    {
        using var db = CreateConnection();
        return await db.QueryAsync<ProdutoQuantidadeDto>(
            "SELECT produto_id, produto_valor, total_vendas FROM dbo.vw_melhores_produtos_quantidade ORDER BY total_vendas DESC"
        );
    }

    public async Task<IEnumerable<ProdutoValorDto>> GetMelhoresProdutosValorAsync()
    {
        using var db = CreateConnection();
        return await db.QueryAsync<ProdutoValorDto>(
            "SELECT produto_id, produto_valor, total_vendas, valor_total_recebido FROM dbo.vw_melhores_produtos_valor ORDER BY valor_total_recebido DESC"
        );
    }

    public async Task<IEnumerable<IngredienteMaisUtilizadoDto>> GetIngredientesMaisUtilizadosAsync()
    {
        using var db = CreateConnection();
        return await db.QueryAsync<IngredienteMaisUtilizadoDto>(
            "SELECT material_id, material_nome, material_medida, total_utilizacoes FROM dbo.vw_ingredientes_mais_utilizados ORDER BY total_utilizacoes DESC"
        );
    }
}
