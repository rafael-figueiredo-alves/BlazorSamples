using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.IO.IsolatedStorage;

namespace BlazorClientes.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] protected IAuthServices? Auth {  get; set; }
        protected string Conta { get; set; } = string.Empty;

        protected override async void OnInitialized()
        {
            Conta = await Auth!.GetRole();
            StateHasChanged();
        }
    }
}
