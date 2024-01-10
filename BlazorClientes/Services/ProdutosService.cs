using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities.PageResults;
using BlazorClientes.Shared.Entities;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using BlazorClientes.Shared.Enums;

namespace BlazorClientes.Services
{
    public class ProdutosService : IProdutos
    {
        #region Private Variables
        private readonly HttpClient? Http;
        private readonly NavigationManager Nav;
        private readonly IToastService Toast;
        private readonly IUserData UserData;
        #endregion

        #region Constructor
        public ProdutosService(HttpClient? http, IUserData _UserData, NavigationManager nav, IToastService toast)
        {
            Http = http;
            UserData = _UserData;
            Nav = nav;
            Toast = toast;
        }
        #endregion

        #region Methods
        public async Task<bool> DeleteProduto(Produtos Produto)
        {
            Http!.DefaultRequestHeaders.Remove("If-Match");

            string Endpoint = "api/v1/Produtos/" + Produto.idProduto;

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                Http!.DefaultRequestHeaders.Remove("If-Match");
                Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", Produto.ETag);

                var httpResponse = await Http!.DeleteAsync(Endpoint);

                if (httpResponse.IsSuccessStatusCode)
                {
                    Toast!.ShowSuccess("Produto foi removido com sucesso!");
                    return true;
                }
                else if ((httpResponse.StatusCode == HttpStatusCode.BadRequest) || (httpResponse.StatusCode == HttpStatusCode.PreconditionFailed))
                {
                    Toast!.ShowWarning("Não foi possível remover produto informado pois registro não foi encontrado ou ele não corresponde com o registrado na base de dados.");
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

        public async Task<Produtos?> GetProduto(string Codigo, GetKind Kind = GetKind.PorCodigo)
        {
            Http!.DefaultRequestHeaders.Remove("If-None-Match");

            string Endpoint = "api/v1/Produtos/" + Codigo + "?Kind=" + Kind;

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                var httpResponse = await Http!.GetAsync(Endpoint, HttpCompletionOption.ResponseHeadersRead);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<ProdutosDTO?>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    List<Produtos>? ListaProdutos = null;

                    if (jsonResult != null)
                    {
                        ListaProdutos = new();
                        ListaProdutos.Add(new Produtos(jsonResult.Produto, jsonResult.Descricao, jsonResult.Valor, jsonResult.Barcode, jsonResult.ETag, jsonResult.idProduto));
                    }

                    return jsonResult == null ? null : ListaProdutos!.FirstOrDefault();
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

        public async Task<PageProdutos?> GetProdutos(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltroProdutos? FiltrarPor = null, string? Termo = null)
        {
            Http!.DefaultRequestHeaders.Remove("If-None-Match");

            string Endpoint = "api/v1/Produtos?Pagina=" + Pagina.ToString() + "&QtdRegistrosPorPagina=" + QtdRegistrosPorPagina.ToString();

            if ((FiltrarPor != null) && (!string.IsNullOrEmpty(Termo)))
            {
                Endpoint += "&FiltrarPor=" + FiltrarPor + "&Termo=" + Termo;
            }

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                if (UserData.UserDB().Produtos != null)
                {
                    if (UserData!.UserDB().Produtos!.Where(x => x.Endpoint == Endpoint).Any())
                    {
                        var ETag = UserData!.UserDB().Produtos!.Where(x => x.Endpoint == Endpoint).First().ETag;
                        Http!.DefaultRequestHeaders.Remove("If-None-Match");
                        Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", ETag);
                    }
                }
                else
                {
                    UserData.UserDB().Produtos = new();
                }

                var httpResponse = await Http!.GetAsync(Endpoint, HttpCompletionOption.ResponseHeadersRead);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<List<ProdutosDTO>?>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    List<Produtos>? ListaProdutos = null;

                    if (jsonResult != null)
                    {
                        ListaProdutos = new();
                        foreach (var produto in jsonResult)
                        {
                            ListaProdutos.Add(new Produtos(produto.Produto, produto.Descricao, produto.Valor, produto.Barcode, produto.ETag, produto.idProduto));
                        }
                    }

                    PageProdutos? PageResult = new();

                    if (ListaProdutos != null)
                    {
                        PageResult.Produtos = new();
                        foreach (var produto in ListaProdutos)
                        {
                            PageResult.Produtos.Add(new ProdutosDTO(produto.Produto, produto.Descricao, produto.Valor, produto.Barcode, produto.ETag, produto.idProduto));
                        }
                    }

                    PageResult.Pagina = Pagina;
                    var TotalPages = httpResponse.Headers.GetValues("TotalPages").First() ?? "0";
                    var TotalRecords = httpResponse.Headers.GetValues("TotalRecords").First() ?? "0";
                    PageResult.TotalPaginas = Convert.ToInt32(TotalPages);
                    PageResult.TotalRecords = Convert.ToInt32(TotalRecords);
                    PageResult.ETag = httpResponse.Headers.GetValues("ETag").First();
                    PageResult.Endpoint = Endpoint;

                    if (UserData!.UserDB().Produtos!.Where(x => x.Endpoint == Endpoint).Any())
                    {
                        UserData!.UserDB().Produtos!.Remove(UserData!.UserDB().Produtos!.Where(x => x.Endpoint == Endpoint).First());
                    }

                    UserData!.UserDB().Produtos!.Add(PageResult);
                    await UserData!.SaveData();

                    return PageResult;
                }
                else if (httpResponse.StatusCode == HttpStatusCode.NotModified)
                {
                    PageProdutos? PageResult = UserData!.UserDB().Produtos!.Where(x => x.Endpoint == Endpoint).First();

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

        public async Task<Produtos?> InsertOrUpdateProduto(Produtos Produto)
        {
            if (Produto.isNewRecord)
            {
                Http!.DefaultRequestHeaders.Remove("If-Match");

                string Endpoint = "api/v1/Produtos";

                try
                {
                    Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                    var httpResponse = await Http!.PostAsJsonAsync(Endpoint, Produto);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        Toast!.ShowSuccess("Novo produto cadastrado com sucesso!");
                        Nav!.NavigateTo("products");
                        return await httpResponse.Content.ReadFromJsonAsync<Produtos>();
                    }
                    else
                    {
                        var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                        var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + jsonResult!.Info);
                        Nav!.NavigateTo("products");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + ex.Message);
                    Nav!.NavigateTo("products");
                    return null;
                }
            }
            else
            {
                Http!.DefaultRequestHeaders.Remove("If-Match");

                string Endpoint = "api/v1/Produtos/" + Produto.idProduto;

                try
                {
                    Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                    Http!.DefaultRequestHeaders.Remove("If-Match");
                    Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", Produto.ETag);

                    var httpResponse = await Http!.PutAsJsonAsync(Endpoint, Produto);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        Toast!.ShowSuccess("Alterações das informações do produto foram salvas com sucesso!");
                        Nav!.NavigateTo("products");
                        return await httpResponse.Content.ReadFromJsonAsync<Produtos>();
                    }
                    else if ((httpResponse.StatusCode == HttpStatusCode.BadRequest) || (httpResponse.StatusCode == HttpStatusCode.PreconditionFailed))
                    {
                        Toast!.ShowWarning("Não foi possível salvar as alterações do produto informado pois registro não foi encontrado ou ele não corresponde com o registrado na base de dados.");
                        Nav!.NavigateTo("products");
                        return null;
                    }
                    else
                    {
                        var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                        var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + jsonResult!.Info);
                        Nav!.NavigateTo("products");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Toast!.ShowError("Ocorreu um erro inesperado! Informações: " + ex.Message);
                    Nav!.NavigateTo("products");
                    return null;
                }
            }
        }

        public async Task<List<Produtos>?> GetAllProdutosToPrint()
        {
            Http!.DefaultRequestHeaders.Remove("If-None-Match");

            string Endpoint = "api/v1/Produtos/print";

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                var httpResponse = await Http!.GetAsync(Endpoint, HttpCompletionOption.ResponseHeadersRead);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<List<ProdutosDTO>?>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    List<Produtos>? ListaProdutos = null;

                    if (jsonResult != null)
                    {
                        ListaProdutos = new();
                        foreach (var produto in jsonResult)
                        {
                            ListaProdutos.Add(new Produtos(produto.Produto, produto.Descricao, produto.Valor, produto.Barcode, produto.ETag, produto.idProduto));
                        }
                    }

                    return ListaProdutos;
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
