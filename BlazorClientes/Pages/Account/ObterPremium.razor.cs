using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;
using BlazorClientes.Services.Interfaces;

namespace BlazorClientes.Pages.Account
{
    public class ObterPremiumBase : ComponentBase
    {
        [Inject] protected IParamService? param { get; set; }
        [Inject] protected IAuthServices? Auth { get; set; }
        [CascadingParameter] protected UITheming? Theme { get; set; }

        protected void ClickObterPremium()
        {
            Auth!.ChangeAccount((int)param!.GetParam()!);
        }

    }
}
