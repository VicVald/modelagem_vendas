using System.Collections.Generic;
using System.Threading.Tasks;

namespace modelagem_vendas.src.repositories.interfaces;

public interface IDashboardRepository
{
    Task<IEnumerable<ProdutoQuantidadeDto>> GetMelhoresProdutosQuantidadeAsync();
    Task<IEnumerable<ProdutoValorDto>> GetMelhoresProdutosValorAsync();
    Task<IEnumerable<IngredienteMaisUtilizadoDto>> GetIngredientesMaisUtilizadosAsync();
}

public class ProdutoQuantidadeDto
{
    public int produto_id { get; set; }
    public decimal produto_valor { get; set; }
    public int total_vendas { get; set; }
}

public class ProdutoValorDto
{
    public int produto_id { get; set; }
    public decimal produto_valor { get; set; }
    public int total_vendas { get; set; }
    public decimal valor_total_recebido { get; set; }
}

public class IngredienteMaisUtilizadoDto
{
    public int material_id { get; set; }
    public string material_nome { get; set; } = string.Empty;
    public string material_medida { get; set; } = string.Empty;
    public int total_utilizacoes { get; set; }
}
