using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Shared.Components
{
    public class BtnTrocaTemaBase : ComponentBase
    {
        [Inject] protected ITheming? theme { get; set; }
        [Parameter] public string ExtraClass { get; set; } = string.Empty;
        protected string Icone { get; set; } = "oi oi-moon";
        private bool isDark { get; set; } = false;

        protected void TrocarTema()
        {
            isDark = !isDark;
            theme!.isDark(isDark);
            StateHasChanged();
            TrocarIcone();
        }

        private void TrocarIcone()
        {
            if (isDark)
            {
                Icone = "oi oi-sun";
            }
            else
            {
                Icone = "oi oi-moon";
            }
            StateHasChanged();
        }
    }
}
