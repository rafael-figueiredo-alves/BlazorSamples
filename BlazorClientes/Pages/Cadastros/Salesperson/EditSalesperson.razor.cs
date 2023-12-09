using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages.Cadastros.Salesperson
{
    public class EditSalespersonBase : ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        #region Injeção de Dependência
        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? Nav { get; set; }
        [Inject] protected IVendedores? Vendedores { get; set; }
        protected Vendedores? Vendedor { get; set; }
        protected string TituloPagina { get; set; } = string.Empty;
        #endregion

        #region Métodos
        protected override void OnInitialized()
        {
            if (ParamService!.GetParam() == null)
            {
                Nav!.NavigateTo("salespeople");
            }
            else
            {
                Vendedor = (Vendedores)ParamService!.GetParam()!;
                if (Vendedor.isNewRecord)
                {
                    TituloPagina = "Novo Vendedor";
                }
                else
                {
                    TituloPagina = "Editando Vendedor";
                }
            }
        }

        protected void GoBack()
        {
            Nav!.NavigateTo("salespeople");
        }

        protected async Task SubmitVendedor()
        {
            await Vendedores!.InsertOrUpdateVendedor(Vendedor!);
        }
        #endregion
    }
}
