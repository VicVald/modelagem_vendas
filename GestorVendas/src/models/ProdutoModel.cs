namespace modelagem_vendas.src.models;

public class Produto
{
    public int id { get; set; }
    public decimal valor { get; set; }
    public int materiais_id { get; set; }
}