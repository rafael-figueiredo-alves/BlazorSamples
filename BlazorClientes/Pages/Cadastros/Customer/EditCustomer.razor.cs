using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages.Cadastros.Customer
{
    public class EditCustomerBase : ComponentBase
    {
        [Inject] protected IParamService? ParamService { get; set; }
    }
}
