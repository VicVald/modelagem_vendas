using modelagem_vendas.src.models;

namespace modelagem_vendas.src.repositories.interfaces;

public interface IClienteRepository
{
    Task<int> AddClientAsync(string nome, string cpf);
    Task<IEnumerable<Cliente>> GetAllClientesAsync();
    Task<Cliente?> GetClienteByIdAsync(int id);
    Task<Cliente?> GetClienteByCpfAsync(string cpf);
    Task<bool> UpdateClienteAsync(int id, string nome);
    Task<bool> DeleteClienteAsync(int id);
}
