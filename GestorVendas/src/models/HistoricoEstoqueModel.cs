namespace modelagem_vendas.src.models;

public class HistoricoEstoque
{
    public int id { get; set; }
    public string quantidade { get; set; }
    public string tipo { get; set; }
    public DateTime created_at { get; set; }
    public int materiais_id { get; set; }
}