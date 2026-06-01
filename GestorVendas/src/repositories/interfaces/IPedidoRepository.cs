using modelagem_vendas.src.models;

namespace modelagem_vendas.src.repositories.interfaces;

public interface IPedidoRepository
{
    Task<int> CreateOrderAsync(int clienteId, List<int> produtoIds);
    Task<Pedido?> GetOrderByIdAsync(int id);
    Task<IEnumerable<Pedido>> ListOrdersByClientAsync(int clienteId);
    Task<bool> DeleteOrderAsync(int id);
    Task<IEnumerable<OrderDetailDto>> GetOrderDetailsAsync(int id);
    Task<IEnumerable<SalesSummaryDto>> GetSalesSummaryAsync(DateTime startDate, DateTime endDate);
}

public class OrderDetailDto
{
    public int pedido_id { get; set; }
    public int cliente_id { get; set; }
    public string cliente_nome { get; set; }
    public decimal valor_total { get; set; }
    public DateTime pedido_data { get; set; }
    public int item_id { get; set; }
    public int produto_id { get; set; }
    public decimal produto_valor { get; set; }
    public int? materiais_id { get; set; }
}

public class SalesSummaryDto
{
    public DateTime data { get; set; }
    public int total_pedidos { get; set; }
    public decimal total_vendas { get; set; }
}
