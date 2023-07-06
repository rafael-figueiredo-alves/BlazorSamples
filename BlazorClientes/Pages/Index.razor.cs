using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;
using System.IO.IsolatedStorage;

namespace BlazorClientes.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        public ILocalStorage? fStorage { get; set; }

        public string? Nome;
        protected override void OnInitialized()
        {
            CarregarValores();
        }

        protected async void CarregarValores();
        { 
            Task t = await fStorage.SetValue("Teste", "Rafael Alves"); 
        }
    }
}
