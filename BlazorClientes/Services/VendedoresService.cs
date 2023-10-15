using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Services
{
    public class VendedoresService : IVendedores
    {
        #region Private Variables
        private readonly HttpClient? Http;
        private readonly ILocalStorage? Ls;
        private readonly NavigationManager Nav;
        private readonly IToastService Toast;
        #endregion

        #region Constructor
        public VendedoresService(HttpClient? http, ILocalStorage? ls, NavigationManager nav, IToastService toast)
        {
            Http = http;
            Ls = ls;
            Nav = nav;
            Toast = toast;
        }
        #endregion

        #region Methods
        public Task<bool> DeleteVendedor(string ID)
        {
            throw new NotImplementedException();
        }

        public Task<Vendedores> GetVendedor(string ID)
        {
            throw new NotImplementedException();
        }

        public Task<List<Vendedores>> GetVendedores(int? Pagina, int? QtdRegistrosPorPagina, FiltroVendedor? FiltrarPor, string? TermoBusca)
        {
            throw new NotImplementedException();
        }

        public Task<Vendedores> InsertOrUpdateVendedor(Vendedores Vendedor)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
