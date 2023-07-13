using BlazorClientes.Auth;
using BlazorClientes.Entities;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Text.Json;

namespace BlazorClientes.Services
{
    public class AuthServices : IAuthServices
    {
        public static readonly string tokenKey = "BlazorClientesToken";
        
        private readonly HttpClient? http;
        private readonly ILocalStorage? Ls;
        private readonly NavigationManager Nav;
        private readonly TokenAuthenticationProvider? authToken;
        private readonly IToastService Toast;

        public AuthServices(HttpClient? _http, ILocalStorage _Ls, NavigationManager _Nav, TokenAuthenticationProvider TokenProvider, IToastService _Toast) 
        {
            http = _http;
            Ls = _Ls;
            Nav = _Nav;
            authToken = TokenProvider;
            Toast = _Toast;
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

                    Toast.ShowSuccess("Login efetuado com sucesso!");

                    Nav!.NavigateTo("/");
                }
                else
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResult = JsonSerializer.Deserialize<ErroRetorno>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new Exception(jsonResult!.Info);
                }
            }
            catch
            {
                throw new Exception("Ocorreu um erro inesperado! Tente novamente.");
            }
        }

        public async Task SignUp(Usuarios usuario)
        {
            var msgErro = string.Empty;
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

                    Toast.ShowSuccess("Conta criada com sucesso!");

                    Nav!.NavigateTo("/");
                }
                else
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                    ErroRetorno? jsonResult = JsonSerializer.Deserialize<ErroRetorno>(ResponseString);
                    msgErro = jsonResult!.Info;
                    throw new Exception(msgErro);
                }
            }
            catch
            {
                if (string.IsNullOrEmpty(msgErro))
                {
                    msgErro = "Ocorreu um erro inesperado! Tente novamente.";
                }
                throw new Exception(msgErro);
            }
        }
    }
}
