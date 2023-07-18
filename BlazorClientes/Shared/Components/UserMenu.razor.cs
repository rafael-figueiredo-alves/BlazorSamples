using BlazorClientes.Auth;
using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorClientes.Shared.Components
{
    public class UserMenuBase : ComponentBase
    {
        [Inject] protected IAuthServices? auth { get; set; }
        protected string? UsuarioLogado { get; set; } = "Desconhecido";
        protected string? EmailUsuario { get; set; } = "example@example.com";

        protected async override void OnInitialized()
        {
            UsuarioLogado = await auth!.GetUserName();
            EmailUsuario = await auth!.GetEmail();
            StateHasChanged();
        }

        protected async void EfetuarLogout()
        {
            await auth!.Logout();
            //StateHasChanged();
        }
    }
}
