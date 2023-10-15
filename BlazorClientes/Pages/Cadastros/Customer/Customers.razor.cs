using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages.Cadastros
{
    public class CustomersBase :  ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        #region Injeções de Dependência
        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        #endregion

        protected List<Clientes> Lista { get; set; } = new();

        protected override void OnInitialized()
        {
            Lista.Add(new Clientes("Rafael Alves", "Rua A, 16", "(11) 2309-0123", "(11) 95044-5876", "teacherdesk6@gmail.com"));
            Lista.Add(new Clientes("Jailza Alves", "Rua B, 16", "(11) 2309-0123", "(11) 95044-5876", "teacherdesk6@gmail.com"));
            Lista.Add(new Clientes("Rafaela Alves", "Rua c, 16", "(11) 2309-0123", "(11) 95044-5876", "teacherdesk6@gmail.com"));
            Lista.Add(new Clientes("Davi Alves", "Rua Francisco, 15", "(11) 2309-0123", "(11) 95044-5876", "teacherdesk6@gmail.com"));
        }
        
        protected void UpdateCliente(Clientes cliente)
        {
            ParamService!.setParam(cliente);
            NavigationManager!.NavigateTo("editcustomer");
        }
        protected void DeleteCliente(string? idCliente)
        {

        }
    }
}
