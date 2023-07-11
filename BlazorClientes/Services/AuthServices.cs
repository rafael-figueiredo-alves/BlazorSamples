using BlazorClientes.Auth;
using BlazorClientes.Entities;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Text.Json;

namespace BlazorClientes.Services
{
    public class AuthServices : IAuthServices
    {
        private string API_Address = "https://localhost:7235/";
        public static readonly string tokenKey = "BlazorClientesToken";
        
        private readonly HttpClient? http;
        private readonly ILocalStorage? Ls;
        private readonly NavigationManager Nav;
        private readonly TokenAuthenticationProvider? authToken;

        public AuthServices(HttpClient? _http, ILocalStorage _Ls, NavigationManager _Nav, TokenAuthenticationProvider TokenProvider) 
        {
            http = _http;
            Ls = _Ls;
            Nav = _Nav;
            authToken = TokenProvider;
        }
        public async Task IsLogged()
        {
            var Token = await Ls!.GetValue("BlazorClientesToken");

            if (!string.IsNullOrEmpty(Token))
            {
                Nav!.NavigateTo("/");
            }
        }

        public async Task Logout()
        {
            await authToken!.Logout();
            Nav!.NavigateTo("/login");
        }

        public async Task SignIn(LoginUser loginUser)
        {
            try
            {
                var JSONBody = JsonSerializer.Serialize(loginUser);

                var httpResponse = await http!.PostAsync("v1/Usuarios",
                                                        new StringContent(JSONBody, Encoding.UTF8, "application/json"));

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<UserToken>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    await authToken!.Login(jsonResult!.Token!);
                    Nav!.NavigateTo("/");
                }
                else
                {
                    throw new Exception("Não foi possível acessar o servidor!");
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task SignUp(Usuarios usuario)
        {
            try
            {
                var JSONBody = JsonSerializer.Serialize(usuario);

                var httpResponse = await http!.PostAsync("v1/Usuarios/register",
                                                        new StringContent(JSONBody, Encoding.UTF8, "application/json"));

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<UserToken>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    await authToken!.Login(jsonResult!.Token!);
                    Nav!.NavigateTo("/");
                }
                else
                {
                    throw new Exception("Não foi possível acessar o servidor!");
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
