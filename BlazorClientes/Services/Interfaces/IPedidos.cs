using BlazorClientes.Shared.Entities.PageResults;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Enums;

namespace BlazorClientes.Services.Interfaces
{
    public interface IPedidos
    {
        Task<PagePedidos?> GetPedidos(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltrosPedido? FiltrarPor = null, string? Termo = null);
        Task<PagePedidos?> GetPedidosPorPeriodo(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltrosPedido? FiltrarPor = null, string? De = null, string? Ate = null);
        Task<Pedidos?> InsertOrUpdatePedido(Pedidos Pedido);
        Task<Pedidos?> SetStatusPedido(Pedidos Pedido, StatusPedido? Status);
        Task<bool> DeletePedido(Pedidos Pedido);
    }
}
