using BlazorClientes.Shared.Entities;
using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Shared.Components
{
    public class BtnTrocaTemaBase : ComponentBase
    {
        [Parameter] public string ExtraClass { get; set; } = string.Empty;

        [Parameter] public EventCallback ChangeTheme { get; set; }
        [CascadingParameter] public UITheming? Theme { get; set; }
        [Parameter] public bool IsDark { get; set; }

        protected void TrocarTema()
        {
            _ = ChangeTheme.InvokeAsync();
            StateHasChanged();
        }
    }
}
