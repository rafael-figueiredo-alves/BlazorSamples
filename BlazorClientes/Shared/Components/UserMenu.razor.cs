﻿using BlazorClientes.Auth;
using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorClientes.Services.Interfaces;

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

        protected override async Task OnInitializedAsync()
        {
            if (auth != null)
            {
                bool logado = await auth!.IsLoggedIn();
                if (logado)
                {
                    UsuarioLogado = await auth!.GetUserName();
                    EmailUsuario = await auth!.GetEmail();
                    uID = Convert.ToInt16(await auth!.GetUserID());
                    Conta = await auth!.GetRole();
                    StateHasChanged();
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
