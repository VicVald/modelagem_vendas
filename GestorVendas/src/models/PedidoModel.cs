namespace modelagem_vendas.src.models;

public class Pedido
{
    public int id { get; set; }
    public int cliente_id {get; set;}

    public decimal valor_total {get; set;}

    public DateTime created_at {get; set;}
}