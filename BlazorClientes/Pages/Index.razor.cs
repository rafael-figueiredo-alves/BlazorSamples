using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.IO.IsolatedStorage;

namespace BlazorClientes.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] protected IAuthServices? Auth {  get; set; }
        protected string Conta { get; set; } = string.Empty;
        protected RenderFragment? ChildContent { get; set; }
        protected string TesteHTML { get; set; } = "<h1><b>Teste HTML</b></h1>";

        protected override async void OnInitialized()
        {
            Conta = await Auth!.GetRole();

            //Exemplo de conversão de valor string em código razor - Excelente para renderizar conteúdo de um Banco de dados ou string externa
            RenderFragment Fragment = (builder) =>
            {
                builder.AddMarkupContent(0, TesteHTML);
            };
            ChildContent = Fragment;
            //---------------------------------------------------------------------------------------------------------------------------------

            StateHasChanged();
        }
    }
}
