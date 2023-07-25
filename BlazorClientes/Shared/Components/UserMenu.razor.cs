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

        protected int uID { get; set; } = 0;

        protected async override void OnInitialized()
        {
            if (auth != null)
            {
                UsuarioLogado = await auth!.GetUserName();
                EmailUsuario = await auth!.GetEmail();
                uID = Convert.ToInt16(await auth!.GetUserID());
                StateHasChanged();
            }
        }

        protected async void EfetuarLogout()
        {
            await auth!.Logout();
        }

        protected void VerPerfil()
        {
            //Implementar endpoint para exibir informações do usuário
        }

        protected void TrocarSenha()
        {
            //Implementar endpoint para troca de senha
        }
    }
}
