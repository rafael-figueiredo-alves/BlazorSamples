﻿using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;
using BlazorClientes.Shared.Enums;

namespace BlazorClientes.Services.Interfaces
{
    public interface IVendedores
    {
        Task<PageVendedores?> GetVendedores(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltroVendedor? FiltrarPor = null, string? Termo = null);
        Task<Vendedores?> InsertOrUpdateVendedor(Vendedores Vendedor);
        Task<bool> DeleteVendedor(Vendedores Vendedor);
        Task<List<Vendedores>?> GetAllVendedoresToPrint();
        Task<Vendedores?> GetVendedor(string Codigo, GetKind Kind = GetKind.PorCodigo);
    }
}
