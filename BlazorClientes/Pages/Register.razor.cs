using BlazorClientes.Entities;
using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorClientes.Pages
{
    public class RegisterBase : ComponentBase
    {
        [Inject] public IAuthServices? Auth { get; set; }

        public Usuarios _NewUser = new Usuarios();

        [Required(ErrorMessage = "É necessário repetir sua senha para confirmar a mesma.")]
        public string? ConfirmaSenha { get; set; }

        [Required(ErrorMessage = "É necessário aceitar os termos de serviço para criar a conta.")]
        public bool AcceptTerms { get; set; } = true;

        protected async override void OnInitialized()
        {
            await Auth!.IsLogged();   
        }
        public void OnValidate()
        {
            try
            {
                Auth!.SignUp(_NewUser);
            }
            catch
            {
                Console.WriteLine("Erro");
            }
        }
    }
}
