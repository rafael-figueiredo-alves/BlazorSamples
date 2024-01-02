using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;

namespace BlazorClientes.Services.Interfaces
{
    public interface IClientes
    {
        Task<PageClientes?> GetClientes(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltrosCliente? FiltrarPor = null, string? Termo = null);
        Task<Clientes?> InsertOrUpdateCliente(Clientes cliente);
        Task<bool> DeleteCliente(Clientes Cliente);
        Task<List<Clientes>?> GetAllClientesToPrint();
        Task<Clientes?> GetCliente(string Codigo);
    }
}
