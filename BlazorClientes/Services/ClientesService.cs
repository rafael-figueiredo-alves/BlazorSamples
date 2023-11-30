using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;
using BlazorClientes.Shared.Utils;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace BlazorClientes.Services
{
    public class ClientesService : IClientes
    {
        #region Private Variables
        private readonly HttpClient? Http;
        private readonly NavigationManager Nav;
        private readonly IToastService Toast;
        private readonly IUserData UserData;
        #endregion

        #region Constructor
        public ClientesService(HttpClient? http, IUserData _UserData, NavigationManager nav, IToastService toast)
        {
            Http = http;
            UserData = _UserData;
            Nav = nav;
            Toast = toast;
        }
        #endregion

        #region Methods
        public async Task<bool> DeleteCliente(Clientes Cliente)
        {
            Http!.DefaultRequestHeaders.Remove("If-Match");

            string Endpoint = "api/v1/Clientes/" + Cliente.idCliente;

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                Http!.DefaultRequestHeaders.Remove("If-Match");
                Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", Cliente.ETag);

                var httpResponse = await Http!.DeleteAsync(Endpoint);

                if (httpResponse.IsSuccessStatusCode)
                {
                    Toast!.ShowSuccess("Cliente foi removido com sucesso!");
                    return true;
                }
                else if ((httpResponse.StatusCode == HttpStatusCode.BadRequest) || (httpResponse.StatusCode == HttpStatusCode.PreconditionFailed))
                {
                    Toast!.ShowWarning("Não foi possível remover cliente informado pois registro não foi encontrado ou ele não corresponde com o registrado na base de dados.");
                    return false;
                }
                else
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + jsonResult!.Info);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + ex.Message);
                return false;
            }
        }

        public Task<Clientes?> GetCliente(string ID)
        {
            throw new NotImplementedException();
        }

        public async Task<PageClientes?> GetClientes(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltrosCliente? FiltrarPor = null, string? Termo = null)
        {
            Http!.DefaultRequestHeaders.Remove("If-None-Match");

            string Endpoint = "api/v1/Clientes?Pagina=" + Pagina.ToString() + "&QtdRegistrosPorPagina=" + QtdRegistrosPorPagina.ToString();

            if((FiltrarPor != null) && (!string.IsNullOrEmpty(Termo)))
            {
                Endpoint += "&FiltrarPor=" + FiltrarPor + "&Termo=" + Termo;
            }

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                if(UserData.UserDB().Clientes != null)
                {
                    if (UserData!.UserDB().Clientes!.Where(x => x.Endpoint == Endpoint).Any())
                    {
                        var ETag = UserData!.UserDB().Clientes!.Where(x => x.Endpoint == Endpoint).First().ETag;
                        Http!.DefaultRequestHeaders.Remove("If-None-Match");
                        Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", ETag);
                    }
                }
                else
                {
                    UserData.UserDB().Clientes = new();
                }

                var httpResponse = await Http!.GetAsync(Endpoint, HttpCompletionOption.ResponseHeadersRead);

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
                            ListaClientes.Add(new Clientes(cliente.Nome!, cliente.Endereco!, cliente.Telefone!, cliente.Celular!, cliente.Email!, cliente.ETag, cliente.idCliente));
                        }
                    }

                    PageClientes? PageResult = new();
                    
                    if(ListaClientes != null)
                    {
                        PageResult.Clientes = new();
                        foreach (var cliente in ListaClientes)
                        {
                            PageResult.Clientes.Add(new ClientesDTO(cliente.Nome!, cliente.Endereco!, cliente.Telefone!, cliente.Celular!, cliente.Email!, cliente.ETag, cliente.idCliente));
                        }
                    }

                    PageResult.Pagina = Pagina;
                    var TotalPages = httpResponse.Headers.GetValues("TotalPages").First() ?? "0";
                    var TotalRecords = httpResponse.Headers.GetValues("TotalRecords").First() ?? "0";
                    PageResult.TotalPaginas = Convert.ToInt32(TotalPages);
                    PageResult.TotalRecords = Convert.ToInt32(TotalRecords);
                    PageResult.ETag = httpResponse.Headers.GetValues("ETag").First();
                    PageResult.Endpoint = Endpoint;

                    if (UserData!.UserDB().Clientes!.Where(x => x.Endpoint == Endpoint).Any())
                    {
                        UserData!.UserDB().Clientes!.Remove(UserData!.UserDB().Clientes!.Where(x => x.Endpoint == Endpoint).First());
                    }

                    UserData!.UserDB().Clientes!.Add(PageResult);
                    await UserData!.SaveData();

                    return PageResult;
                }
                else if (httpResponse.StatusCode == HttpStatusCode.NotModified)
                {
                    PageClientes? PageResult = UserData!.UserDB().Clientes!.Where(x => x.Endpoint == Endpoint).First();

                    PageResult.Pagina = Pagina;
                    var TotalPages = httpResponse.Headers.GetValues("TotalPages").First() ?? "0";
                    var TotalRecords = httpResponse.Headers.GetValues("TotalRecords").First() ?? "0";
                    PageResult.TotalPaginas = Convert.ToInt32(TotalPages);
                    PageResult.TotalRecords = Convert.ToInt32(TotalRecords);
                    PageResult.ETag = httpResponse.Headers.GetValues("ETag").First();
                    PageResult.Endpoint = Endpoint;

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
