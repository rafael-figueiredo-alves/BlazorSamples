using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Shared.Components
{
    public class ConfirmDlgBase : ComponentBase
    {
        protected bool Exibir { get; set; } = false;
        [Parameter] public string? Titulo { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback OnYesClick { get; set; }
        [CascadingParameter] public UITheming? Theme { get; set; }

        public void Exibe()
        {
            Exibir = true;
        }

        public void Oculta()
        {
            Exibir = false;
        }
    }
}
