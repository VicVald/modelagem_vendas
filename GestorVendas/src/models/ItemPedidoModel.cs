namespace modelagem_vendas.src.models;

public class ItemPedido
{
    public int id { get; set; }
    public int pedido_id { get; set; }
    public int produto_id { get; set; }
    public DateTime created_at { get; set; }
}