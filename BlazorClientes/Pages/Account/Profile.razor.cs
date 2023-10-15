using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Services.Interfaces;

namespace BlazorClientes.Pages.Account
{
    public class ProfileBase : ComponentBase
    {
        [CascadingParameter] public UITheming? Theme { get; set; }
        [Inject] protected IAuthServices? auth { get; set; }
        [Inject] protected IParamService? param { get; set; }

        protected UserProfile? _UserProfile { get; set; } = null;

        protected override void OnInitialized()
        {
            _UserProfile = (UserProfile)param!.GetParam()!;
        }

        protected async void OnValidSubmit()
        {
            _UserProfile = await auth!.SaveProfile(_UserProfile!);
            StateHasChanged();
        }

    }
}
