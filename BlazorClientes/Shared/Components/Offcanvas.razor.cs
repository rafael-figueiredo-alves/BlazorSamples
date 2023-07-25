using BlazorClientes.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Shared.Components
{
    public class OffcanvasBase : ComponentBase
    {
        [CascadingParameter] protected UITheming? Theme { get; set; }
    }
}
