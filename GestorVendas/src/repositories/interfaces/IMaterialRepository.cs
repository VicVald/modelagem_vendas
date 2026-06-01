using modelagem_vendas.src.models;

namespace modelagem_vendas.src.repositories.interfaces;

public interface IMaterialRepository
{
    Task<int> AddMaterialAsync(string nome, int quantidade, string medida);
    Task<IEnumerable<Material>> GetAllMateriaisAsync();
    Task<Material?> GetMaterialByIdAsync(int id);
    Task<bool> UpdateMaterialAsync(int id, string nome, int quantidade, string medida);
    Task<bool> DeleteMaterialAsync(int id);
    Task<IEnumerable<Material>> GetLowStockMaterialsAsync(int threshold);
    Task<bool> AdjustMaterialStockAsync(int materialId, int delta, string tipo);
}
