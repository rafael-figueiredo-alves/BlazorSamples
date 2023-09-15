using BlazorClientes.Auth;
using BlazorClientes.Entities;
using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorClientes.Shared.Components
{
    public class UserMenuBase : ComponentBase
    {
        [CascadingParameter] protected UITheming? Theme { get; set; }
        [Inject] protected IAuthServices? auth { get; set; }
        protected string? UsuarioLogado { get; set; } = "Desconhecido";
        protected string? EmailUsuario { get; set; } = "example@example.com";
        protected string? Conta { get; set; }

        protected int uID { get; set; } = 0;

        protected async override void OnInitialized()
        {
            if (auth != null)
            {
                bool logado = await auth!.IsLoggedIn();
                if (logado)
                {
                    DateTime ExpiraEm = await auth!.GetExpiration();
                    UsuarioLogado = await auth!.GetUserName();
                    EmailUsuario = await auth!.GetEmail();
                    uID = Convert.ToInt16(await auth!.GetUserID());
                    Conta = await auth!.GetRole();
                    StateHasChanged();
                    if (ExpiraEm < DateTime.Now)
                    {
                        await auth!.Logout();
                    }
                }
            }
        }

        protected async void EfetuarLogout()
        {
            await auth!.Logout();
        }

        protected async void VerPerfil()
        {
            await auth!.GetProfile(uID);
        }

        protected void TrocarSenha()
        {
            auth!.ChangePassword(uID);
        }

        protected void ObterAdmin()
        {
            auth!.ObterContaAdmin(uID);
        }
    }
}
