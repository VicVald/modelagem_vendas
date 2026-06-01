using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using modelagem_vendas.src.models;
using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public PedidoRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("DefaultConnection connection string not found.");
    }

    private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<int> CreateOrderAsync(int clienteId, List<int> produtoIds)
    {
        if (produtoIds == null || !produtoIds.Any())
        {
            throw new ArgumentException("O pedido precisa conter pelo menos um produto.");
        }

        using var connection = CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            decimal valorTotal = 0;
            var productsDetails = new List<Produto>();

            // 1. Fetch details of all products in the list
            foreach (var prodId in produtoIds)
            {
                var product = await connection.QueryFirstOrDefaultAsync<Produto>(
                    "SELECT id, valor, materiais_id FROM dbo.produtos WHERE id = @id", 
                    new { id = prodId }, 
                    transaction: transaction
                );

                if (product == null)
                {
                    throw new KeyNotFoundException($"Produto com ID {prodId} não encontrado.");
                }

                productsDetails.Add(product);
                valorTotal += product.valor;
            }

            // 2. Create the order using prc_add_order
            var orderId = await connection.QuerySingleAsync<int>(
                "prc_add_order", 
                new { cliente_id = clienteId, valor_total = valorTotal }, 
                transaction: transaction, 
                commandType: CommandType.StoredProcedure
            );

            // 3. For each product, add order items and update stocks
            foreach (var product in productsDetails)
            {
                // Add order item via prc_add_order_item
                await connection.ExecuteAsync(
                    "prc_add_order_item", 
                    new { pedido_id = orderId, produto_id = product.id }, 
                    transaction: transaction, 
                    commandType: CommandType.StoredProcedure
                );

                // Collect materials associated with this product
                var uniqueMaterialIds = new HashSet<int>();

                // Direct material reference
                if (product.materiais_id > 0)
                {
                    uniqueMaterialIds.Add(product.materiais_id);
                }

                // Many-to-many materials reference
                var linkedMaterials = await connection.QueryAsync<Material>(
                    "prc_list_product_materials", 
                    new { produto_id = product.id }, 
                    transaction: transaction, 
                    commandType: CommandType.StoredProcedure
                );

                foreach (var mat in linkedMaterials)
                {
                    uniqueMaterialIds.Add(mat.id);
                }

                // Decrement stock for each material and register stock history
                foreach (var materialId in uniqueMaterialIds)
                {
                    await connection.ExecuteAsync(
                        "prc_adjust_material_stock", 
                        new { material_id = materialId, delta = -1, tipo = "SAIDA_PEDIDO" }, 
                        transaction: transaction, 
                        commandType: CommandType.StoredProcedure
                    );
                }
            }

            transaction.Commit();
            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Pedido?> GetOrderByIdAsync(int id)
    {
        using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Pedido>("prc_get_order", new { id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Pedido>> ListOrdersByClientAsync(int clienteId)
    {
        using var db = CreateConnection();
        return await db.QueryAsync<Pedido>("prc_list_orders_by_client", new { cliente_id = clienteId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        using var db = CreateConnection();
        var affected = await db.ExecuteAsync("prc_remove_order", new { id }, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }

    public async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsAsync(int id)
    {
        using var db = CreateConnection();
        return await db.QueryAsync<OrderDetailDto>("prc_get_order_details", new { pedido_id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<SalesSummaryDto>> GetSalesSummaryAsync(DateTime startDate, DateTime endDate)
    {
        using var db = CreateConnection();
        return await db.QueryAsync<SalesSummaryDto>(
            "prc_get_sales_summary", 
            new { start_date = startDate, end_date = endDate }, 
            commandType: CommandType.StoredProcedure
        );
    }
}
