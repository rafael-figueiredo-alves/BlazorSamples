using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Components;
using Microsoft.AspNetCore.Components.Web;
using Blazored.Toast.Services;

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
        protected Clientes? SelectedCliente { get; set; }
        protected Produtos? SelectedProduto { get; set; }
        protected Vendedores? SelectedVendedor { get; set; }
        #endregion

        protected ChooseDlg? ChooseCliente { get; set; }
        protected ChooseDlg? ChooseProduto { get; set; }
        protected ChooseDlg? ChooseVendedor { get; set; }

        protected string? ClienteID {  get; set; }
        protected string? VendedorID { get; set; }
        protected string? ProdutoID { get; set; }

        protected int Quantidade { get; set; } = 1;
        protected decimal ValorUni { get; set; } = (decimal)0.00;

        protected decimal ValorTotalPedido { get; set; } = (decimal)0.00;
        protected decimal ValorComissao { get; set; } = (decimal)0.00;

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
           // await Orders!.InsertOrUpdatePedido(Pedido!);
        }

        protected void GetCliente(object? Value)
        {
            if(Value != null)
            {
                SelectedCliente = (Clientes)Value;
            }
        }

        protected void GetProduto(object? Value)
        {
            if (Value != null)
            {
                SelectedProduto = (Produtos)Value;
                ValorUni = SelectedProduto.Valor == null ? (decimal)0.00 : (decimal)SelectedProduto.Valor;
            }
        }

        protected void RemoveItem(ItensPedido Value)
        {
            Pedido.Itens!.Remove(Value);
            CalcularPedido();
        }

        protected void GetVendedor(object? Value)
        {
            if (Value != null)
            {
                SelectedVendedor = (Vendedores)Value;
            }
        }

        protected async Task OnKeyUpCliente(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                if (!string.IsNullOrEmpty(ClienteID))
                {
                    Clientes? result = await Clientes!.GetCliente(ClienteID);
                    if (result != null)
                    {
                        try
                        {
                            SelectedCliente = result;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    } else
                    {
                        ChooseCliente!.Exibe(ChooseType.Customers);
                    }
                }
                else
                {
                    ChooseCliente!.Exibe(ChooseType.Customers);
                }
            }
        }

        protected void AddItem()
        {
            if(SelectedProduto != null)
            {
                Pedido.Itens!.Add(new ItensPedido(Pedido.idPedido, SelectedProduto.idProduto, SelectedProduto.Produto, Quantidade, ValorUni, 0));
                SelectedProduto = null;
                Quantidade = 1;
                ValorUni = (decimal)0.00;
                ProdutoID = null;
                CalcularPedido();
            }
        }

        protected void CalcularPedido()
        {
            if(Pedido.Itens!.Any())
            {
                ValorTotalPedido = decimal.Zero;
                ValorComissao = decimal.Zero;
                foreach (var item in Pedido.Itens!)
                {
                    ValorTotalPedido += item.Valor;
                }

                if(SelectedVendedor != null)
                {
                    if(SelectedVendedor.pComissao > 0)
                    {
                        ValorComissao = (ValorTotalPedido * SelectedVendedor.pComissao) / 100;
                    }
                }
            }
            else
            {
                ValorComissao = (decimal)0.00;
                ValorTotalPedido = (decimal)0.00;
            }
        }

        protected async Task OnKeyUpProduto(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                if (ProdutoID != null)
                {
                    Produtos? result = await Produtos!.GetProduto(ProdutoID);
                    if (result != null)
                    {
                        SelectedProduto = result;
                        ValorUni = SelectedProduto.Valor == null ? (decimal)0.00 : (decimal)SelectedProduto.Valor;
                    }
                    else
                    {
                        ChooseProduto!.Exibe(ChooseType.Products);
                    }
                }
            }
        }

        protected async Task OnKeyUpVendedor(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                if (VendedorID != null)
                {
                    Vendedores? result = await Vendedores!.GetVendedor(VendedorID);
                    if (result != null)
                    {
                        SelectedVendedor = result;
                    }
                    else
                    {
                        ChooseVendedor!.Exibe(ChooseType.Salespeople);
                    }
                }
            }
        }
    }
}
