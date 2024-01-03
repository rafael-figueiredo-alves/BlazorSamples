using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
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

        public async Task<Vendedores?> GetVendedor(string Codigo)
        {
            Http!.DefaultRequestHeaders.Remove("If-None-Match");

            string Endpoint = "api/v1/Vendedores/" + Codigo;

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

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
                            ListaVendedores.Add(new Vendedores(vendedor.Vendedor, vendedor.pComissao, vendedor.ETag, vendedor.Codigo, vendedor.idVendedor));
                        }
                    }

                    return jsonResult == null ? null : ListaVendedores!.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro inesperado! Tente novamente. Detalhes: " + ex.Message);
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
                            ListaVendedores.Add(new Vendedores(vendedor.Vendedor, vendedor.pComissao, vendedor.ETag, vendedor.Codigo, vendedor.idVendedor));
                        }
                    }

                    PageVendedores? PageResult = new();

                    if (ListaVendedores != null)
                    {
                        PageResult.Vendedores = new();
                        foreach (var vendedor in ListaVendedores)
                        {
                            PageResult.Vendedores.Add(new VendedoresDTO(vendedor.Vendedor, vendedor.pComissao, vendedor.ETag, vendedor.Codigo, vendedor.idVendedor));
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
                    PageVendedores? PageResult = UserData!.UserDB().Vendedores!.Where(x => x.Endpoint == Endpoint).First();

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

        public async Task<Vendedores?> InsertOrUpdateVendedor(Vendedores Vendedor)
        {
            if (Vendedor.isNewRecord)
            {
                Http!.DefaultRequestHeaders.Remove("If-Match");

                string Endpoint = "api/v1/Vendedores";

                try
                {
                    Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                    var httpResponse = await Http!.PostAsJsonAsync(Endpoint, Vendedor);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        Toast!.ShowSuccess("Novo vendedor cadastrado com sucesso!");
                        Nav!.NavigateTo("salespeople");
                        return await httpResponse.Content.ReadFromJsonAsync<Vendedores>();
                    }
                    else
                    {
                        var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                        var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + jsonResult!.Info);
                        Nav!.NavigateTo("salespeople");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + ex.Message);
                    Nav!.NavigateTo("salespeople");
                    return null;
                }
            }
            else
            {
                Http!.DefaultRequestHeaders.Remove("If-Match");

                string Endpoint = "api/v1/Vendedores/" + Vendedor.idVendedor;

                try
                {
                    Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                    Http!.DefaultRequestHeaders.Remove("If-Match");
                    Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", Vendedor.ETag);

                    var httpResponse = await Http!.PutAsJsonAsync(Endpoint, Vendedor);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        Toast!.ShowSuccess("Alterações das informações do vendedor foram salvas com sucesso!");
                        Nav!.NavigateTo("salespeople");
                        return await httpResponse.Content.ReadFromJsonAsync<Vendedores>();
                    }
                    else if ((httpResponse.StatusCode == HttpStatusCode.BadRequest) || (httpResponse.StatusCode == HttpStatusCode.PreconditionFailed))
                    {
                        Toast!.ShowWarning("Não foi possível salvar as alterações do vendedor informado pois registro não foi encontrado ou ele não corresponde com o registrado na base de dados.");
                        Nav!.NavigateTo("salespeople");
                        return null;
                    }
                    else
                    {
                        var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                        var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + jsonResult!.Info);
                        Nav!.NavigateTo("salespeople");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + ex.Message);
                    Nav!.NavigateTo("salespeople");
                    return null;
                }
            }
        }

        public async Task<List<Vendedores>?> GetAllVendedoresToPrint()
        {
            Http!.DefaultRequestHeaders.Remove("If-None-Match");

            string Endpoint = "api/v1/Vendedores/print";

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

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
                            ListaVendedores.Add(new Vendedores(vendedor.Vendedor!, vendedor.pComissao!, vendedor.ETag!, vendedor.Codigo, vendedor.idVendedor!));
                        }
                    }

                    return ListaVendedores;
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
        #endregion
    }
}
