using BlazorClientes.Auth;
using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Services
{
    public class ClientesService : IClientes
    {
        #region Private Variables
        private readonly HttpClient? Http;
        private readonly ILocalStorage? Ls;
        private readonly NavigationManager Nav;
        private readonly IToastService Toast;
        #endregion

        #region Constructor
        public ClientesService(HttpClient? http, ILocalStorage? ls, NavigationManager nav, IToastService toast)
        {
            Http = http;
            Ls = ls;
            Nav = nav;
            Toast = toast;
        }
        #endregion

        #region Methods
        public Task<bool> DeleteCliente(string ID)
        {
            throw new NotImplementedException();
        }

        public Task<Clientes> GetCliente(string ID)
        {
            throw new NotImplementedException();
        }

        public Task<List<Clientes>> GetClientes(int? Pagina, int? QtdRegistrosPorPagina, FiltrosCliente? FiltrarPor, string? Termo)
        {
            throw new NotImplementedException();
        }

        public Task<Clientes> InsertOrUpdateCliente(Clientes cliente)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
