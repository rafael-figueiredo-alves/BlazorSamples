using BlazorClientes.Shared.Entities;

namespace BlazorClientes.Services.Interfaces
{
    public interface IClientes
    {
        Task<List<Clientes>> GetClientes(int? Pagina, int? QtdRegistrosPorPagina, FiltrosCliente? FiltrarPor, string? Termo);
        Task<Clientes> GetCliente(string ID);
        Task<Clientes> InsertOrUpdateCliente(Clientes cliente);
        Task<bool> DeleteCliente(string ID);
    }
}
