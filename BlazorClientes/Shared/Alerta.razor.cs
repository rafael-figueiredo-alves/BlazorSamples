using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Shared
{
    public class AlertaBase : ComponentBase
    {
        public enum AlertType
        {
            atAviso,
            atSucesso
        }
        public string? TipoAlerta;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public AlertType TipoDeAlerta { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }

        protected override void OnParametersSet()
        {
            switch (TipoDeAlerta)
            {
                case AlertType.atAviso:
                    TipoAlerta = "alert alert-danger alert-dismissible fade show";
                    break;
                case AlertType.atSucesso:
                    TipoAlerta = "alert alert-success alert-dismissible fade show";
                    break;
                default:
                    TipoAlerta = "alert alert-danger alert-dismissible fade show";
                    break;
            }
        }
    }
}
