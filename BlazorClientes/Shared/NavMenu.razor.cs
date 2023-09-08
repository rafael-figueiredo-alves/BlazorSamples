using Microsoft.AspNetCore.Components;
using BlazorClientes.Entities;

namespace BlazorClientes.Shared
{
    public class NavMenuBase : ComponentBase
    {
        [Parameter] public EventCallback ChangeTheme { get; set; }
        [Parameter] public bool IsDark { get; set; }

        [CascadingParameter]
        protected UITheming? Theme { get; set; }
    }
}
