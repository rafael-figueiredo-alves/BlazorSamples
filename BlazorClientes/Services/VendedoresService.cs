using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text.Json;

namespace BlazorClientes.Services
{
    public class VendedoresService : IVendedores
    {
        #region Private Variables
        private readonly HttpClient? Http;
        private readonly NavigationManager Nav;
        private readonly IToastService Toast;
        private readonly IUserData UserData;
        #endregion

        #region Constructor
        public VendedoresService(HttpClient? http, IUserData _UserData, NavigationManager nav, IToastService toast)
        {
            Http = http;
            UserData = _UserData;
            Nav = nav;
            Toast = toast;
        }
        #endregion

        #region Methods
        public async Task<bool> DeleteVendedor(Vendedores Vendedor)
        {
            Http!.DefaultRequestHeaders.Remove("If-Match");

            string Endpoint = "api/v1/Vendedores/" + Vendedor.idVendedor;

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                Http!.DefaultRequestHeaders.Remove("If-Match");
                Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", Vendedor.ETag);

                var httpResponse = await Http!.DeleteAsync(Endpoint);

                if (httpResponse.IsSuccessStatusCode)
                {
                    Toast!.ShowSuccess("Vendedor foi removido com sucesso!");
                    return true;
                }
                else if ((httpResponse.StatusCode == HttpStatusCode.BadRequest) || (httpResponse.StatusCode == HttpStatusCode.PreconditionFailed))
                {
                    Toast!.ShowWarning("Não foi possível remover vendedor informado pois registro não foi encontrado ou ele não corresponde com o registrado na base de dados.");
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

        public async Task<PageVendedores?> GetVendedores(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltroVendedor? FiltrarPor = null, string? Termo = null)
        {
            Http!.DefaultRequestHeaders.Remove("If-None-Match");

            string Endpoint = "api/v1/Vendedores?Pagina=" + Pagina.ToString() + "&QtdRegistrosPorPagina=" + QtdRegistrosPorPagina.ToString();

            if ((FiltrarPor != null) && (!string.IsNullOrEmpty(Termo)))
            {
                Endpoint += "&FiltrarPor=" + FiltrarPor + "&Termo=" + Termo;
            }

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                if (UserData.UserDB().Vendedores != null)
                {
                    if (UserData!.UserDB().Vendedores!.Where(x => x.Endpoint == Endpoint).Any())
                    {
                        var ETag = UserData!.UserDB().Vendedores!.Where(x => x.Endpoint == Endpoint).First().ETag;
                        Http!.DefaultRequestHeaders.Remove("If-None-Match");
                        Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", ETag);
                    }
                }
                else
                {
                    UserData.UserDB().Vendedores = new();
                }

                var httpResponse = await Http!.GetAsync(Endpoint, HttpCompletionOption.ResponseHeadersRead);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<List<VendedoresDTO>?>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    List<Vendedores>? ListaVendedores = null;

                    if (jsonResult != null)
                    {
                        ListaVendedores = new();
                        foreach (var vendedor in jsonResult)
                        {
                            ListaVendedores.Add(new Vendedores(vendedor.Vendedor, vendedor.pComissao, vendedor.ETag, vendedor.idVendedor));
                        }
                    }

                    PageVendedores? PageResult = new();

                    if (ListaVendedores != null)
                    {
                        PageResult.Vendedores = new();
                        foreach (var vendedor in ListaVendedores)
                        {
                            PageResult.Vendedores.Add(new VendedoresDTO(vendedor.Vendedor, vendedor.pComissao, vendedor.ETag, vendedor.idVendedor));
                        }
                    }

                    PageResult.Pagina = Pagina;
                    var TotalPages = httpResponse.Headers.GetValues("TotalPages").First() ?? "0";
                    var TotalRecords = httpResponse.Headers.GetValues("TotalRecords").First() ?? "0";
                    PageResult.TotalPaginas = Convert.ToInt32(TotalPages);
                    PageResult.TotalRecords = Convert.ToInt32(TotalRecords);
                    PageResult.ETag = httpResponse.Headers.GetValues("ETag").First();
                    PageResult.Endpoint = Endpoint;

                    if (UserData!.UserDB().Vendedores!.Where(x => x.Endpoint == Endpoint).Any())
                    {
                        UserData!.UserDB().Vendedores!.Remove(UserData!.UserDB().Vendedores!.Where(x => x.Endpoint == Endpoint).First());
                    }

                    UserData!.UserDB().Vendedores!.Add(PageResult);
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

        public Task<Vendedores> InsertOrUpdateVendedor(Vendedores Vendedor)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
