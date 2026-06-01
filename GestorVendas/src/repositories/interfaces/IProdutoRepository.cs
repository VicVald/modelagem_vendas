using modelagem_vendas.src.models;

namespace modelagem_vendas.src.repositories.interfaces;

public interface IProdutoRepository
{
    Task<int> AddProdutoAsync(decimal valor, int? materiaisId);
    Task<IEnumerable<Produto>> GetAllProdutosAsync();
    Task<Produto?> GetProdutoByIdAsync(int id);
    Task<bool> UpdateProdutoAsync(int id, decimal valor, int? materiaisId);
    Task<bool> DeleteProdutoAsync(int id);
    Task LinkMaterialProdutoAsync(int materialId, int produtoId);
    Task UnlinkMaterialProdutoAsync(int relationId);
    Task RemoveMaterialProdutoAsync(int produtoId, int materialId);
    Task<IEnumerable<Material>> GetMateriaisByProdutoAsync(int produtoId);
    Task<IEnumerable<Produto>> GetProductsByMaterialAsync(int materialId);
}
