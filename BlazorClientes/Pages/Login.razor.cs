using BlazorClientes.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages
{
    public class LoginBase : ComponentBase
    {
        [Inject] public required HttpClient Http { get; set; }
        public LoginUser _loginUser = new LoginUser();
        public int anoCopyright { get; set; }

        protected override void OnInitialized()
        {
            anoCopyright = DateTime.Now.Year;
        }
        public void OnValidate()
        {
     
        }
    }
}
