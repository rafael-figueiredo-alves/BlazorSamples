using Microsoft.AspNetCore.Components;
using BlazorClientes.Entities;

namespace BlazorClientes.Pages
{
    public class ProfileBase : ComponentBase
    {
        [CascadingParameter] public UITheming? Theme { get; set; }
    }
}
