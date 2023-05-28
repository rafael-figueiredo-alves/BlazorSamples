using Microsoft.AspNetCore.Components;

namespace MauiSimpleToDO.Shared
{
    public class MsgBase : ComponentBase
    {
        protected bool Exibir { get; set; } = false;
        [Parameter] public string Titulo { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public EventCallback OnYesClick { get; set; }

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
