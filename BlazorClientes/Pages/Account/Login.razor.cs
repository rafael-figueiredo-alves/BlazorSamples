using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Components;
using Blazored.Toast.Services;
using BlazorClientes.Services.Interfaces;

namespace BlazorClientes.Pages.Account
{
    public class LoginBase : ComponentBase
    {
        public bool ExibirAviso = false;
        public string Mensagem = string.Empty;
        [Inject] public IAuthServices? Auth { get; set; }
        [Inject] public NavigationManager? Nav { get; set; }

        public LoginUser _loginUser = new();
        public int AnoCopyright { get; set; }

        protected async override void OnInitialized()
        {
            AnoCopyright = DateTime.Now.Year;

            await Auth!.IsLogged();
        }

        public async void OnValidate()
        {
            try
            {
                await Auth!.SignIn(_loginUser);
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
}
