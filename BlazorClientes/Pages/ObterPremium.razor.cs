using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages
{
    public class ObterPremiumBase : ComponentBase
    {
        [Inject] protected IParamService? param {  get; set; }
    }
}
