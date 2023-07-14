using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Shared.Components
{
    public class TermsDlgBase : ComponentBase
    {
        protected bool Exibir { get; set; } = false;
        [Parameter] public string? Titulo { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

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
