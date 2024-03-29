﻿using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Components;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using BlazorClientes.Services.Interfaces;

namespace BlazorClientes.Pages.Account
{
    public class RegisterBase : ComponentBase
    {
        public bool ExibirAviso = false;
        public string Mensagem = string.Empty;
        [Inject] public IAuthServices? Auth { get; set; }

        public TermsDlg? TermsOfServiceDlg;
        public int AnoCopyright { get; set; }

        public usuarios _NewUser = new();

        [Required(ErrorMessage = "É necessário repetir sua senha para confirmar a mesma.")]
        public string? ConfirmaSenha { get; set; }

        [Required(ErrorMessage = "É necessário aceitar os termos de serviço para criar a conta.")]
        public bool AcceptTerms { get; set; } = true;

        protected override void OnInitialized()
        {
            AnoCopyright = DateTime.Now.Year;
        }
        public async void OnValidate()
        {
            if (!AcceptTerms)
            {
                Mensagem = "Para criar conta, é necessário marcar \"Aceitar\" os termos.";
                ExibirAviso = true;
                return;
            }

            if (_NewUser.Senha != ConfirmaSenha)
            {
                Mensagem = "Senha e confirmação de senha não correspondem! Verifique.";
                ExibirAviso = true;
                return;
            }

            try
            {
                await Auth!.SignUp(_NewUser);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message;
                ExibirAviso = true;
                StateHasChanged();
            }
        }

        public void FecharAviso()
        {
            ExibirAviso = false;
        }

        public void LerTermos()
        {
            TermsOfServiceDlg!.Exibe();
        }
    }
}
