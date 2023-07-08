using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;
using System.IO.IsolatedStorage;

namespace BlazorClientes.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        public ILocalStorage? fStorage { get; set; }

        public string? Name { get; set; } = "Lopes";
        protected override void OnInitialized()
        {
            CarregarValores();
        }

        public async void PegarNome()
        {
            Name = await fStorage!.GetValue("Teste");
            StateHasChanged();
        }

        public void LimparJson()
        {
            //await fStorage!.DeleteValue("Teste");
            throw new NotImplementedException("Teste");
        }

        protected async void CarregarValores()
        { 
            await fStorage!.SetValue("Teste", "Rafael Alves");
            PegarNome();
        }
    }
}
