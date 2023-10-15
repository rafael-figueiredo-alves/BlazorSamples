using BlazorClientes.Shared.Entities;

namespace BlazorClientes.Services.Interfaces
{
    public interface IVendedores
    {
        Task<List<Vendedores>> GetVendedores(int? Pagina, int? QtdRegistrosPorPagina, FiltroVendedor? FiltrarPor, string? TermoBusca);
        Task<Vendedores> GetVendedor(string ID);
        Task<Vendedores> InsertOrUpdateVendedor(Vendedores Vendedor);
        Task<bool> DeleteVendedor(string ID);
    }
}
