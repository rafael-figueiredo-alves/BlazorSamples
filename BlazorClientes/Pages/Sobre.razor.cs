using BlazorClientes.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages
{
    public class SobreBase : ComponentBase
    {
        [CascadingParameter] protected UITheming? Theme { get; set; }
    }
}
