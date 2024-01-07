using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Entities;
using Microsoft.JSInterop;
using BlazorClientes.Shared.Components;
using BlazorClientes.Shared.Entities.PageResults;

namespace BlazorClientes.Pages.Pedidos
{
    public class OrdersBase : ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        #region Injeções de Dependência
        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        [Inject] protected IPedidos? pedidos { get; set; }
        [Inject] protected IJSRuntime? JSRuntime { get; set; }
        #endregion

        #region Variables
        protected List<BlazorClientes.Shared.Entities.Pedidos>? Lista { get; set; } = new();
        protected Dictionary<string, int>? Filtros { get; set; } = null;
        protected (string? Filtro, int? FiltroIndice) FiltroSelecionado { get; set; }
        protected int PaginaAtual { get; set; } = 1;
        protected int QuantidadeTotalPaginas { get; set; } = 1;
        protected int ItensPorPagina { get; set; } = 10;
        protected int TotalDeRegistros { get; set; } = 0;
        protected string InfoPaginasERegistros { get; set; } = string.Empty;
        protected ConfirmDlg? MsgDelete { get; set; }
        protected ConfirmDlg? MsgCancelaPedido { get; set; }
        protected ConfirmDlg? MsgEntregaPedido { get; set; }
        protected BlazorClientes.Shared.Entities.Pedidos? SelectedPedidos { get; set; } = null;
        protected string? TermoBusca { get; set; } = null;
        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            Filtros = new()
            {
                { "Por Data Emissao", 0 },
                { "Por Data Entrega", 1 },
                { "Por Cliente", 3 },
                { "Por Vendedor", 5 },
                { "Por Status", 6 }
            };
            FiltroSelecionado = ("Por Status", 6);
            GetPage(PaginaAtual);
            await JSRuntime!.InvokeVoidAsync("AtivarToolTips");
        }

        protected void GetPageClick(int page)
        {
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(page, (FiltrosPedido)FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(page);
            }
        }

        protected void InsertPedido()
        {
            NavigationManager!.NavigateTo("neworder");
        }

        protected void UpdatePedido(BlazorClientes.Shared.Entities.Pedidos pedido)
        {
            //ParamService!.setParam(pedido);
            //NavigationManager!.NavigateTo("editcustomer");
        }

        protected async Task ConfirmPedidoEntregue()
        {
            MsgEntregaPedido!.Oculta();
            await pedidos!.SetStatusPedido(SelectedPedidos!, Shared.Enums.StatusPedido.Entregue);
            GetPage(PaginaAtual);
        }

        protected void EntregarPedido(BlazorClientes.Shared.Entities.Pedidos pedido)
        {
            SelectedPedidos = pedido;
            MsgEntregaPedido!.Exibe();
        }

        protected void CancelarPedido(BlazorClientes.Shared.Entities.Pedidos pedido)
        {
            SelectedPedidos = pedido;
            MsgCancelaPedido!.Exibe();
        }

        protected async Task ConfirmPedidoCancelado()
        {
            MsgCancelaPedido!.Oculta();
            await pedidos!.SetStatusPedido(SelectedPedidos!, Shared.Enums.StatusPedido.Cancelado);
            GetPage(PaginaAtual);
        }

        protected void DeletePedido(BlazorClientes.Shared.Entities.Pedidos pedido)
        {
            SelectedPedidos = pedido;
            MsgDelete!.Exibe();
        }

        protected async Task ConfirmDeleteCliente()
        {
            MsgDelete!.Oculta();

            bool Result = await pedidos!.DeletePedido(SelectedPedidos!);

            if (Result)
            {
                if (!string.IsNullOrEmpty(TermoBusca))
                {
                    GetPage(PaginaAtual, (FiltrosPedido)FiltroSelecionado.FiltroIndice!, TermoBusca);
                }
                else
                {
                    GetPage(PaginaAtual);
                }
            }
        }

        protected void PrintPedidos()
        {
            //Implementar no futuro
        }

        protected void OnChangeQtdItensPorPagina(ChangeEventArgs args)
        {
            ItensPorPagina = Convert.ToInt32(args.Value);
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(PaginaAtual, (FiltrosPedido)FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(PaginaAtual);
            }
        }

        protected async void GetPage(int Page, FiltrosPedido? Filtro = null, string? Termo = null)
        {
        GetPageBeginning:

            PagePedidos? Pagina = await pedidos!.GetPedidos(Page, ItensPorPagina, Filtro, Termo);

            if (Pagina!.Pedidos != null)
            {
                Lista = new();
                foreach (var pedido in Pagina!.Pedidos)
                {
                    Lista.Add(new BlazorClientes.Shared.Entities.Pedidos(pedido.idCliente!, pedido.idVendedor!, pedido.vComissao, pedido.pComissao, pedido.ValorTotal, pedido.DataEmissao, pedido.DataEntrega, pedido.Status, pedido.ETag, pedido.Itens, pedido.idPedido));
                }
            }
            else
            {
                Lista = null;
            }

            PaginaAtual = Page;
            QuantidadeTotalPaginas = (int)Pagina!.TotalPaginas!;
            TotalDeRegistros = (int)Pagina!.TotalRecords!;
            InfoPaginasERegistros = (TotalDeRegistros == 0 ? "Nenhum pedido encontrado" : (TotalDeRegistros == 1 ? $"{TotalDeRegistros} pedido encontrado" : $"{TotalDeRegistros} pedidos encontrados"));

            if ((PaginaAtual > 1) && ((Lista == null) || (!Lista!.Any())))
            {
                Page--;
                goto GetPageBeginning;
            }

            StateHasChanged();
        }

        protected void SearchClick((string? Termo, int? Filtro) args)
        {
            TermoBusca = null;
            FiltroSelecionado = (Filtros!.Where(x => x.Value == args.Filtro).FirstOrDefault().Key, args.Filtro);

            if (args.Filtro != null)
            {
                GetPage(PaginaAtual, (FiltrosPedido)args.Filtro, args.Termo);
                TermoBusca = args.Termo;
            }
        }
        #endregion
    }
}
