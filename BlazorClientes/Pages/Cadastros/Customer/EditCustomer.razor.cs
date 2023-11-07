using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages.Cadastros.Customer
{
    public class EditCustomerBase : ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? Nav {  get; set; }

        protected Clientes? Cliente { get; set; }
        protected string TituloPagina { get; set; } = string.Empty;

        protected override void OnInitialized()
        {
            if(ParamService!.GetParam() == null)
            {
                Nav!.NavigateTo("customers");
            }
            else
            {
                Cliente = (Clientes)ParamService!.GetParam()!;
                if(Cliente.isNewRecord)
                {
                    TituloPagina = "Novo Cliente";
                }
                else
                {
                    TituloPagina = "Alterando Cliente";
                }
            }
        }

        protected void GoBack()
        {
            Nav!.NavigateTo("customers");
        }
    }
}
