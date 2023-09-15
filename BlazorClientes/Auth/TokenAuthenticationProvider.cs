using BlazorClientes.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorClientes.Auth
{
    public class TokenAuthenticationProvider : AuthenticationStateProvider, IAuthToken
    {
        public static readonly string tokenKey = "BlazorClientesToken";
        private readonly HttpClient http;
        private readonly ILocalStorage Ls;
        private static AuthenticationState NotAuthenticate => new(new ClaimsPrincipal(new ClaimsIdentity()));
        //---------------------------------------------------------------------------------
        public TokenAuthenticationProvider(HttpClient _http, ILocalStorage _Ls)
        {
            http = _http;
            Ls   = _Ls;
        }
        //---------------------------------------------------------------------------------
        //                    Métodos de suporte
        //---------------------------------------------------------------------------------
        /// <summary>
        /// Método para pegar os claims do JWT
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer
                .Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs!.TryGetValue(ClaimTypes.Role, out object? roles);

            if (roles != null)
            {
                if (roles.ToString()!.Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);
                    foreach (var parsedRole in parsedRoles!)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
                }
                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp =>
            new Claim(kvp.Key, kvp.Value.ToString()!)));
            return claims;
        }

        /// <summary>
        /// Método para pegar retorno do token JWT
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
        //---------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------
        //                   Métodos principais da classe
        //---------------------------------------------------------------------------------
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await Ls.GetValue(tokenKey);
            
            if (string.IsNullOrEmpty(token))
            {
                return NotAuthenticate;
            }

            return CreateAuthenticationState(token);
        }

        public AuthenticationState CreateAuthenticationState(string Token)
        {
            // colocar o token obtido do localstorage no header do request 
            // na seção Authorization assim poderemos estar autenticando 
            // cada requisição HTTP enviada ao servidor por este cliente
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", Token);

            //extrair as claims
            return new AuthenticationState(new ClaimsPrincipal
                (new ClaimsIdentity(ParseClaimsFromJwt(Token), "jwt")));
        }

        //---------------------------------------------------------------------------------
        //       Métodos da interface para Login e Logout
        //---------------------------------------------------------------------------------
        public async Task Login(string Token)
        {
            try
            {
                await Ls.SetValue(tokenKey, Token);
                var authState = CreateAuthenticationState(Token);
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Logout()
        {
            try
            {
                await Ls.DeleteValue(tokenKey);
                http.DefaultRequestHeaders.Authorization = null;
                NotifyAuthenticationStateChanged(Task.FromResult(NotAuthenticate));
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetUsername()
        {
            var fUser = (await GetAuthenticationStateAsync()).User;
            return fUser.FindFirst(c => c.Type.Contains("Username"))?.Value ?? "Unknown";
        }

        public async Task<string> GetEmail()
        {
            var fUser = (await GetAuthenticationStateAsync()).User;
            return fUser.FindFirst(c => c.Type.Contains("Email"))?.Value ?? "Unknown";
        }

        public async Task<string> GetUserID()
        {
            var fUser = (await GetAuthenticationStateAsync()).User;
            return fUser.FindFirst(c => c.Type.Contains("uID"))?.Value ?? "-1";
        }

        public async Task<DateTime> GetExpiration()
        {
            var fUser = (await GetAuthenticationStateAsync()).User;
            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(fUser.FindFirst(x => x.Type == "exp")!.Value)).UtcDateTime;
        }

        public async Task<string> GetRole()
        {
            var fUser = (await GetAuthenticationStateAsync()).User;
            return fUser.FindFirst(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        }
    }
}
