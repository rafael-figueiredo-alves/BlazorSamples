using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Components;
using Microsoft.AspNetCore.Components.Web;
using Blazored.Toast.Services;
using BlazorClientes.Shared.Enums;

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
        [Inject] protected IToastService? Toast { get; set; }
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

        #region Reference Variables
        protected ChooseDlg? ChooseCliente { get; set; }
        protected ChooseDlg? ChooseProduto { get; set; }
        protected ChooseDlg? ChooseVendedor { get; set; }
        #endregion

        #region Helper Variables
        protected string? ClienteID {  get; set; }
        protected string? VendedorID { get; set; }
        protected string? ProdutoID { get; set; }

        protected int Quantidade { get; set; } = 1;
        protected decimal ValorUni { get; set; } = (decimal)0.00;

        protected decimal ValorTotalPedido { get; set; } = (decimal)0.00;
        protected decimal ValorComissao { get; set; } = (decimal)0.00;
        protected int Desconto { get; set; } = 0;
        #endregion

        protected bool ShouldSubmit { get; set; } = false;

        #region Methods
        protected override void OnInitialized()
        {
            TituloPagina = "Novo Pedido";
        }

        protected void GoBack()
        {
            Nav!.NavigateTo("orders");
        }

        protected async Task SubmitButtonClick()
        {
            ShouldSubmit = true;
            await SubmitOrder();
        }

        #region Methods related to submit order
        protected void PreencherDadosPedido()
        {
            if(SelectedCliente != null)
            {
                Pedido.idCliente = SelectedCliente.idCliente;
                Pedido.Cliente   = SelectedCliente.Nome;
            }
            else
            {
                throw new Exception("Não é possível emitir um pedido sem ter um cliente associado ao mesmo.");
            }

            if (SelectedVendedor != null)
            {
                Pedido.idVendedor = SelectedVendedor.idVendedor;
                Pedido.Vendedor = SelectedVendedor.Vendedor;
            }
            else
            {
                throw new Exception("Não é possível emitir um pedido sem ter um vendedor associado ao mesmo.");
            }

            if (Pedido.DataEmissao > Pedido.DataEntrega)
            {
                throw new Exception("Não é possível emitir um pedido com uma data de entrega inferior a data de sua emissão.");
            }

            if (ValorTotalPedido > 0)
            {
                Pedido.ValorTotal = ValorTotalPedido;
            }
            else
            {
                throw new Exception("Não é possível emitir um pedido com valor financeiro igual a ZERO.");
            }

            Pedido.vComissao = ValorComissao;
            Pedido.Status = StatusPedido.Emitido.ToString();
        }

        protected async Task SubmitOrder()
        {
            if(ShouldSubmit)
            {
                try
                {
                    PreencherDadosPedido();
                    await Orders!.InsertOrUpdatePedido(Pedido!);
                }
                catch (Exception ex)
                {
                    Toast!.ShowWarning(ex.Message);
                    ShouldSubmit = false;
                }
            }
        }
        #endregion

        #region Methods to get objects(Customers, Salespeople, Products)
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

        protected void GetVendedor(object? Value)
        {
            if (Value != null)
            {
                SelectedVendedor = (Vendedores)Value;
                CalcularPedido();
            }
        }
        #endregion

        #region Methods related to Itens
        protected void RemoveItem(ItensPedido Value)
        {
            Pedido.Itens!.Remove(Value);
            CalcularPedido();
        }

        protected void AddItem()
        {
            if (SelectedProduto != null)
            {
                Pedido.Itens!.Add(new ItensPedido(Pedido.idPedido, SelectedProduto.idProduto, SelectedProduto.Descricao, Quantidade, ValorUni, Desconto));
                SelectedProduto = null;
                Quantidade = 1;
                Desconto = 0;
                ValorUni = (decimal)0.00;
                ProdutoID = null;
                CalcularPedido();
            }
        }
        #endregion

        #region Methods related to Keyboard
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
                        CalcularPedido();
                    }
                    else
                    {
                        ChooseVendedor!.Exibe(ChooseType.Salespeople);
                    }
                }
            }
        }
        #endregion

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

                decimal vTotalSemDesconto = decimal.Zero;
                foreach (var item in Pedido.Itens!)
                {
                    vTotalSemDesconto += item.ValorUnitario * item.Quantidade;
                }


                if (SelectedVendedor != null)
                {
                    if(SelectedVendedor.pComissao > 0)
                    {
                        ValorComissao = (vTotalSemDesconto * SelectedVendedor.pComissao) / 100;
                    }
                }
            }
            else
            {
                ValorComissao = (decimal)0.00;
                ValorTotalPedido = (decimal)0.00;
            }
        }
        #endregion
    }
}
