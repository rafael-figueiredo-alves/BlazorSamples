using BlazorClientes.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazorClientes.Shared.Components
{
    public class OffcanvasBase : ComponentBase
    {
        [CascadingParameter] protected UITheming? Theme { get; set; }
        [Inject] protected IJSRuntime? Js { get; set; }

        public async void testar()
        {
            await Js!.InvokeVoidAsync("TesteJS", "#home-collapse");
        }
    }
}
