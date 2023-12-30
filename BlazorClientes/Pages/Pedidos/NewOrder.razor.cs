using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Components;

namespace BlazorClientes.Pages.Pedidos
{
    public class NewOrderBase : ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        #region Injeções de Dependência
        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? Nav { get; set; }
        [Inject] protected IPedidos? Orders { get; set; }
        [Inject] protected IProdutos? Produtos { get; set; }
        [Inject] protected IClientes? Clientes { get; set; }
        [Inject] protected IVendedores? Vendedores { get; set; }
        #endregion

        #region Variables
        protected Shared.Entities.Pedidos Pedido { get; set; } = new();
        protected string TituloPagina { get; set; } = string.Empty;
        protected List<Clientes>? ListClientes {  get; set; }
        protected Clientes? SelectedCliente { get; set; }
        protected List<Produtos>? ListProdutos { get; set; }
        protected Produtos? SelectedProduto { get; set; }
        protected List<Vendedores>? ListVendedores { get; set; }
        protected Vendedores? SelectedVendedor { get; set; }
        #endregion

        protected ChooseDlg? ChooseCliente { get; set; }

        protected override void OnInitialized()
        {
            TituloPagina = "Novo Pedido";
        }

        protected void GoBack()
        {
            Nav!.NavigateTo("orders");
        }

        protected async Task SubmitOrder()
        {
            await Orders!.InsertOrUpdatePedido(Pedido!);
        }
    }
}
