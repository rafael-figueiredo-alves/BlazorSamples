using BlazorClientes.Services;
using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Components;
using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages.Pedidos
{
    public class ViewOrderBase : ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        [Inject] protected IProdutos? ProdutoService {  get; set; }
        [Inject] protected IPedidos? PedidoService { get; set; }
        [Inject] protected IClientes? ClienteService { get; set; }
        [Inject] protected IVendedores? VendedorService { get; set; }
        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? Nav { get; set; }
        protected BlazorClientes.Shared.Entities.Pedidos? Pedido { get; set; }
        protected Clientes? Cliente { get; set; } = new();
        protected Vendedores? Vendedor {  get; set; } = new();
        protected Produtos? Produtos { get; set; } = new();
        protected RenderFragment? TituloPagina { get; set; }
        protected ConfirmDlg? MsgDelete { get; set; }
        protected ConfirmDlg? MsgCancelaPedido { get; set; }
        protected ConfirmDlg? MsgEntregaPedido { get; set; }


        protected RenderFragment RenderizarTitulo(string? Titulo)
        {
            return (builder) =>
            {
                builder.AddMarkupContent(0, Titulo);
            };
        }

        protected override async Task OnInitializedAsync()
        {
            Pedido = (BlazorClientes.Shared.Entities.Pedidos?)ParamService!.GetParam();

            if (Pedido != null)
            {
                TituloPagina = RenderizarTitulo("Pedido ID <strong>" + Pedido.idPedido + "</strong>" );
                Cliente = await ClienteService!.GetCliente(Pedido.idCliente!, Shared.Enums.GetKind.PorID);
                Pedido.Cliente = Cliente!.Nome;
                Vendedor = await VendedorService!.GetVendedor(Pedido.idVendedor!, Shared.Enums.GetKind.PorID);
                Pedido.Vendedor = Vendedor!.Vendedor;

                foreach(var Item in Pedido.Itens!)
                {
                    var Produto = await ProdutoService!.GetProduto(Item.idProduto!, Shared.Enums.GetKind.PorID);
                    Item.Descricao = Produto!.Descricao;
                }
            }
        }

        protected void GoBack()
        {
            Nav!.NavigateTo("orders");
        }

        protected async Task ConfirmPedidoEntregue()
        {
            MsgEntregaPedido!.Oculta();
            await PedidoService!.SetStatusPedido(Pedido!, Shared.Enums.StatusPedido.Entregue);
            Pedido!.Status = Shared.Enums.StatusPedido.Entregue.ToString();
            StateHasChanged();
        }

        protected void EntregarPedido()
        {
            MsgEntregaPedido!.Exibe();
        }

        protected void CancelarPedido()
        {
            MsgCancelaPedido!.Exibe();
        }

        protected async Task ConfirmPedidoCancelado()
        {
            MsgCancelaPedido!.Oculta();
            await PedidoService!.SetStatusPedido(Pedido!, Shared.Enums.StatusPedido.Cancelado);
            Pedido!.Status = Shared.Enums.StatusPedido.Cancelado.ToString();
            StateHasChanged();
        }

        protected void DeletePedido()
        {
            MsgDelete!.Exibe();
        }

        protected async Task ConfirmDeleteCliente()
        {
            MsgDelete!.Oculta();

            bool Result = await PedidoService!.DeletePedido(Pedido!);

            GoBack();
        }
    }
}
