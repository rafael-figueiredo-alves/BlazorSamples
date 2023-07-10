using BlazorClientes.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Services
{
    public class AuthServices : IAuthServices
    {
        public static readonly string tokenKey = "BlazorClientesToken";
        
        private readonly HttpClient? http;
        private readonly ILocalStorage? Ls;
        private readonly NavigationManager Nav;

        public AuthServices(HttpClient? _http, ILocalStorage _Ls, NavigationManager _Nav) 
        {
            http = _http;
            Ls = _Ls;
            Nav = _Nav;
        }
        public async Task IsLogged()
        {
            var Token = await Ls!.GetValue("BlazorClientesToken");

            if (!string.IsNullOrEmpty(Token))
            {
                Nav!.NavigateTo("/");
            }
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public Task SignIn(LoginUser loginUser)
        {
            throw new NotImplementedException();
        }

        public Task SignUp(Usuarios usuario)
        {
            throw new NotImplementedException();
        }
    }
}
