using BlazorClientes.Entities;
using BlazorClientes.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages
{
    public class LoginBase : ComponentBase
    {
        [Inject] public IAuthServices? Auth { get; set; }
        [Inject] public NavigationManager? Nav { get; set; }
        public LoginUser _loginUser = new LoginUser();
        public int anoCopyright { get; set; }

        protected async override void OnInitialized()
        {
            await Auth!.IsLogged();

            anoCopyright = DateTime.Now.Year;
        }

        public void OnValidate()
        {
     
        }
    }
}
