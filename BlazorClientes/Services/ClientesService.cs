using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

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
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");
                var httpResponse = await Http!.GetAsync("api/v1/Clientes?Pagina=" + Pagina.ToString() + "&QtdRegistrosPorPagina=" + QtdRegistrosPorPagina.ToString(), HttpCompletionOption.ResponseHeadersRead);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<List<ClientesDTO>?>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    List<Clientes>? ListaClientes = null;

                    if (jsonResult != null)
                    {
                        ListaClientes = new();
                        foreach (var cliente in jsonResult)
                        {
                            ListaClientes.Add(new Clientes(cliente.Nome!, cliente.Endereco!, cliente.Telefone!, cliente.Celular!, cliente.Email!, cliente.idCliente));
                        }
                    }

                    PageClientes? PageResult = new();
                    
                    PageResult.Clientes = ListaClientes;
                    PageResult.Pagina = Pagina;
                    var TotalPages = httpResponse.Headers.GetValues("TotalPages").First() ?? "0";
                    var TotalRecords = httpResponse.Headers.GetValues("TotalRecords").First() ?? "0";
                    PageResult.TotalPaginas = Convert.ToInt32(TotalPages);
                    PageResult.TotalRecords = Convert.ToInt32(TotalRecords);
                    PageResult.ETag = httpResponse.Headers.GetValues("ETag").First();
                    PageResult.Endpoint = Http!.BaseAddress!.ToString() + "api/v1/Clientes";
                    

                    return PageResult;
                }
                else if (httpResponse.StatusCode.Equals(StatusCodes.Status304NotModified))
                {
                    PageClientes? PageResult = new();

                    PageResult.Pagina = Pagina;
                    var TotalPages = httpResponse.Headers.GetValues("TotalPages").First() ?? "0";
                    var TotalRecords = httpResponse.Headers.GetValues("TotalRecords").First() ?? "0";
                    PageResult.TotalPaginas = Convert.ToInt32(TotalPages);
                    PageResult.TotalRecords = Convert.ToInt32(TotalRecords);
                    PageResult.ETag = httpResponse.Headers.GetValues("ETag").First();
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
                throw new Exception("Ocorreu um erro inesperado! Tente novamente. Detalhes: " + ex.Message);
            }
        }

        public Task<Clientes> InsertOrUpdateCliente(Clientes cliente)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
