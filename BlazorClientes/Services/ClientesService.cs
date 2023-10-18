using BlazorClientes.Auth;
using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using BlazorClientes.Shared.Entities.PageResults;

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

        public async Task<PageClientes?> GetClientes(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltrosCliente? FiltrarPor = null, string? Termo = null)
        {
            try
            {
                var httpResponse = await Http!.GetAsync("api/v1/Clientes", HttpCompletionOption.ResponseContentRead);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<List<Clientes>?>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    PageClientes? PageResult = new();
                    var teste = httpResponse.Content.Headers.ToString();
                    PageResult.Clientes = jsonResult;
                    PageResult.Pagina = Pagina;
                    PageResult.TotalPaginas = Convert.ToInt32(httpResponse.Headers.GetValues("TotalPages").First());
                    PageResult.TotalRecords = Convert.ToInt32(httpResponse.Headers.GetValues("TotalRecords").First());
                    PageResult.Endpoint = Http!.BaseAddress!.ToString() + "api/v1/Clientes";
                    

                    return PageResult;
                }
                else
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new Exception(jsonResult!.Info);
                }
            }
            catch (Exception ex)
            {
                var Teste = ex;
                var Teste1 = ex.Message;
                throw new Exception("Ocorreu um erro inesperado! Tente novamente.");
            }
        }

        public Task<Clientes> InsertOrUpdateCliente(Clientes cliente)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
