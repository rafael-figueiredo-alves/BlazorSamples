using BlazorClientes.Auth;
using BlazorClientes.Shared.Entities;
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
        private readonly IParamService paramService;

        public AuthServices(HttpClient? _http, ILocalStorage _Ls, NavigationManager _Nav, TokenAuthenticationProvider TokenProvider, IToastService _Toast, IParamService _paramserv) 
        {
            http = _http;
            Ls = _Ls;
            Nav = _Nav;
            authToken = TokenProvider;
            Toast = _Toast;
            paramService = _paramserv;
        }

        public Task<string> GetUserName()
        {
            return authToken!.GetUsername();
        }

        public Task<string> GetEmail()
        {
            return authToken!.GetEmail();
        }

        public Task<string> GetUserID()
        {
            return authToken!.GetUserID();
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

        public Task<string> GetRole()
        {
            return authToken!.GetRole();
        }

        public async Task SignIn(LoginUser loginUser)
        {
            try
            {
                var JSONBody = JsonSerializer.Serialize(loginUser);

                var httpResponse = await http!.PostAsync("api/v1/Usuarios",
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
                    var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new Exception(jsonResult!.Info);
                }
            }
            catch
            {
                throw new Exception("Ocorreu um erro inesperado! Tente novamente.");
            }
        }

        public async Task SignUp(usuarios usuario)
        {
            usuario.TipoConta = "User";
            var msgErro = string.Empty;
            try
            {
                var JSONBody = JsonSerializer.Serialize(usuario);

                var httpResponse = await http!.PostAsync("api/v1/Usuarios/register",
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
                    Erro? jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString);
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

        public Task<DateTime> GetExpiration()
        {
            return authToken!.GetExpiration();
        }

        public async Task<bool> IsLoggedIn()
        {
            var Token = await Ls!.GetValue("BlazorClientesToken");

            return !string.IsNullOrEmpty(Token);
        }

        public async Task<UserProfile> SaveProfile(UserProfile _UserProfile)
        {
            try
            {
                var JSONBody = JsonSerializer.Serialize(_UserProfile);

                var httpResponse = await http!.PostAsync("api/v1/Usuarios/" + _UserProfile.ID.ToString(),
                                                        new StringContent(JSONBody, Encoding.UTF8, "application/json"));

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<UserProfile>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    Toast.ShowSuccess("Perfil atualizado com sucesso!");

                    return jsonResult!;
                }
                else
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    Toast.ShowError(jsonResult!.Info!);

                    return _UserProfile;
                }
            }
            catch
            {
                Toast.ShowError("Ocorreu um erro inesperado! Tente novamente.");
                return _UserProfile;
            }
        }

        public async Task GetProfile(int ID)
        {
            try
            {
                var httpResponse = await http!.GetAsync("api/v1/Usuarios/" + ID.ToString());

                if (httpResponse.IsSuccessStatusCode)
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResult = JsonSerializer.Deserialize<UserProfile>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    paramService!.setParam(jsonResult);

                    Nav!.NavigateTo("/profile");
                }
                else
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new Exception(jsonResult!.Info);
                }
            }
            catch
            {
                throw new Exception("Ocorreu um erro inesperado! Tente novamente.");
            }
        }

        public void ChangePassword(int ID)
        {
            paramService!.setParam(ID);

            Nav!.NavigateTo("/changepassword");
        }

        public void ObterContaAdmin(int ID)
        {
            paramService!.setParam(ID);

            Nav!.NavigateTo("/getpremium");
        }

        public async Task SaveNewPassword(int ID, string NewPassword)
        {
            try
            {
                var httpResponse = await http!.PutAsync("api/v1/Usuarios/password?id=" + ID.ToString() + "&senha=" + NewPassword, null);

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Toast.ShowSuccess("Senha trocada com sucesso!");

                    Nav!.NavigateTo("/");
                }
                else
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Toast.ShowError(jsonResult!.Info!);
                }
            }
            catch
            {
                Toast.ShowError("Ocorreu um erro inesperado! Tente novamente.");
            }
        }

        public async Task ChangeAccount(int ID)
        {
            try
            {
                var httpResponse = await http!.PutAsync("api/v1/Usuarios/account?id=" + ID.ToString() + "&tipo=Admin", null);

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Toast.ShowSuccess("Parabéns, sua conta foi atualizada para o plano Premium com sucesso!");

                    await Logout();
                }
                else
                {
                    var ResponseString = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResult = JsonSerializer.Deserialize<Erro>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Toast.ShowError(jsonResult!.Info!);
                }
            }
            catch
            {
                Toast.ShowError("Ocorreu um erro inesperado! Tente novamente.");
            }
        }
    }
}
