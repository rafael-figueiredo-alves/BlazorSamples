using BlazorClientes.Shared.Entities;
using BlazorClientes.Services;
using BlazorClientes.Shared.Components;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorClientes.Pages.Account
{
    public class ChangePasswordBase : ComponentBase
    {
        [CascadingParameter] public UITheming? Theme { get; set; }
        [Inject] protected IAuthServices? Auth { get; set; }
        [Inject] protected IParamService? Param { get; set; }

        public bool ExibirAviso = false;
        public string Mensagem = string.Empty;
        public PasswordChange TrocaSenha = new();
        public int? UID { get; set; }

        protected override void OnInitialized()
        {
            UID = (int)Param!.GetParam()!;
        }
        public async void OnValidate()
        {
            if (TrocaSenha.NewPassword != TrocaSenha.ConfirmaSenha)
            {
                Mensagem = "Senha e confirmação de senha não correspondem! Verifique.";
                ExibirAviso = true;
                return;
            }

            try
            {
                await Auth!.SaveNewPassword((int)UID!, TrocaSenha.NewPassword!);
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
    }

    public class PasswordChange
    {
        [Required(ErrorMessage = "É necessário informar uma nova senha!")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos {1} caracteres")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "É necessário repetir a nova senha para confirmar a mesma.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos {1} caracteres")]
        public string? ConfirmaSenha { get; set; }
    }
}
