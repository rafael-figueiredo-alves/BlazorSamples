﻿using BlazorClientes.Auth;
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
            usuario.TipoConta = "User";
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

                var httpResponse = await http!.PostAsync("v1/Usuarios/" + _UserProfile.ID.ToString(),
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
                    var jsonResult = JsonSerializer.Deserialize<ErroRetorno>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new Exception(jsonResult!.Info);
                }
            }
            catch
            {
                throw new Exception("Ocorreu um erro inesperado! Tente novamente.");
            }
        }

        public async Task GetProfile(int ID)
        {
            try
            {
                var httpResponse = await http!.GetAsync("v1/Usuarios/" + ID.ToString());

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
                    var jsonResult = JsonSerializer.Deserialize<ErroRetorno>(ResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new Exception(jsonResult!.Info);
                }
            }
            catch
            {
                throw new Exception("Ocorreu um erro inesperado! Tente novamente.");
            }
        }
    }
}
