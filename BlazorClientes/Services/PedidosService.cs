using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities.PageResults;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Enums;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text.Json;

namespace BlazorClientes.Services
{
    public class PedidosService : IPedidos
    {
        #region Private Variables
        private readonly HttpClient? Http;
        private readonly NavigationManager Nav;
        private readonly IToastService Toast;
        private readonly IUserData UserData;
        #endregion

        #region Constructor
        public PedidosService(HttpClient? http, IUserData _UserData, NavigationManager nav, IToastService toast)
        {
            Http = http;
            UserData = _UserData;
            Nav = nav;
            Toast = toast;
        }
        #endregion

        public async Task<PagePedidos?> GetPedidos(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltrosPedido? FiltrarPor = null, string? Termo = null)
        {
            Http!.DefaultRequestHeaders.Remove("If-None-Match");

            string Endpoint = "api/v1/Pedidos?Pagina=" + Pagina.ToString() + "&QtdRegistrosPorPagina=" + QtdRegistrosPorPagina.ToString();

            if ((FiltrarPor != null) && (!string.IsNullOrEmpty(Termo)))
            {
                Endpoint += "&FiltrarPor=" + FiltrarPor + "&Termo1=" + Termo;
            }

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                if (UserData.UserDB().Pedidos != null)
                {
                    if (UserData!.UserDB().Pedidos!.Where(x => x.Endpoint == Endpoint).Any())
                    {
                        var ETag = UserData!.UserDB().Pedidos!.Where(x => x.Endpoint == Endpoint).First().ETag;
                        Http!.DefaultRequestHeaders.Remove("If-None-Match");
                        Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", ETag);
                    }
                }
                else
                {
                    UserData.UserDB().Pedidos = new();
                }

                var httpResponse = await Http!.GetAsync(Endpoint, HttpCompletionOption.ResponseHeadersRead);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<List<PedidosDTO>?>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    List<Pedidos>? ListaPedidos = null;

                    if (jsonResult != null)
                    {
                        ListaPedidos = new();
                        foreach (var pedido in jsonResult)
                        {
                            ListaPedidos.Add(new Pedidos(pedido.idCliente!, pedido.idVendedor!, pedido.vComissao, pedido.pComissao, pedido.ValorTotal, pedido.DataEmissao, pedido.DataEntrega, pedido.Status, pedido.idPedido));
                        }
                    }

                    PagePedidos? PageResult = new();

                    if (ListaPedidos != null)
                    {
                        PageResult.Pedidos = new();
                        foreach (var pedido in ListaPedidos)
                        {
                            PageResult.Pedidos.Add(new PedidosDTO(pedido.idCliente!, pedido.idVendedor!, pedido.vComissao, pedido.pComissao, pedido.ValorTotal, pedido.DataEmissao, pedido.DataEntrega, pedido.Status, pedido.idPedido)));
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

        public async Task<PagePedidos?> GetPedidosPorPeriodo(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltrosPedido? FiltrarPor = null, string? De = null, string? Ate = null)
        {
            throw new NotImplementedException();
        }

        public async Task<Pedidos?> InsertOrUpdatePedido(Pedidos Pedido)
        {
            throw new NotImplementedException();
        }

        public async Task<Pedidos?> SetStatusPedido(int? Pedido, StatusPedido? Status)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeletePedido(Pedidos Pedido)
        {
            Http!.DefaultRequestHeaders.Remove("If-Match");

            string Endpoint = "api/v1/Pedidos/" + Pedido.idPedido;

            try
            {
                Http!.DefaultRequestHeaders.Add("Access-Control-Allow-Headers", "*");

                Http!.DefaultRequestHeaders.Remove("If-Match");
                Http!.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", Pedido.ETag);

                var httpResponse = await Http!.DeleteAsync(Endpoint);

                if (httpResponse.IsSuccessStatusCode)
                {
                    Toast!.ShowSuccess("Pedido foi removido com sucesso!");
                    return true;
                }
                else if ((httpResponse.StatusCode == HttpStatusCode.BadRequest) || (httpResponse.StatusCode == HttpStatusCode.PreconditionFailed))
                {
                    Toast!.ShowWarning("Não foi possível remover pedido informado pois registro não foi encontrado ou ele não corresponde com o registrado na base de dados.");
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
    }
}
