using BlazorClientes.Entities;
using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages
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
            await Auth!.IsLogged();

            AnoCopyright = DateTime.Now.Year;
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
