using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
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
        private readonly NavigationManager Nav;
        public IUserData UserData { get;  set; }
        private static AuthenticationState NotAuthenticate => new(new ClaimsPrincipal(new ClaimsIdentity()));
        //---------------------------------------------------------------------------------
        public TokenAuthenticationProvider(HttpClient _http, ILocalStorage _Ls, IUserData _UserData, NavigationManager _Nav)
        {
            http = _http;
            Ls   = _Ls;
            UserData = _UserData;
            Nav = _Nav;
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
            else
            {
                DateTime exp = GetExpirationFromToken(token);
                string? uID = GetuID(token);

                if (exp < DateTime.UtcNow)
                {
                    await Ls.DeleteValue(tokenKey);
                    await Ls.DeleteValue(uID);
                    Nav.NavigateTo("login");
                    return NotAuthenticate;
                }

                await UserData!.ReadData(uID);
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
                
                if (!string.IsNullOrEmpty(Token))
                {
                    string? uID = GetuID(Token);

                    await UserData!.ReadData(uID);
                }

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
                await Ls.DeleteValue(await GetUserID());
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
            var token = await Ls.GetValue(tokenKey);
            if(!string.IsNullOrEmpty(token))
            {
                IEnumerable<Claim> claims = ParseClaimsFromJwt(token);
                return claims.Where(c => c.Type.Contains("Username"))!.First().Value ?? "Unknown";
            }
            else
                return "Unknown";
        }

        public async Task<string> GetEmail()
        {
            var token = await Ls.GetValue(tokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                IEnumerable<Claim> claims = ParseClaimsFromJwt(token);
                return claims.Where(c => c.Type.Contains("Email"))!.First().Value ?? "Unknown";
            }
            else
                return "Unknown";
        }

        public async Task<string> GetUserID()
        {
            var token = await Ls.GetValue(tokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                IEnumerable<Claim> claims = ParseClaimsFromJwt(token);
                return claims.Where(c => c.Type.Contains("uID"))!.First().Value ?? "-1";
            }
            else
                return "-1";
        }

        private string GetuID(string Token)
        {
            if (!string.IsNullOrEmpty(Token))
            {
                IEnumerable<Claim> claims = ParseClaimsFromJwt(Token);
                return claims.Where(c => c.Type.Contains("uID"))!.First().Value ?? "-1";
            }
            else
                return "-1";
        }

        public async Task<DateTime> GetExpiration()
        {
            var token = await Ls.GetValue(tokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                IEnumerable<Claim> claims = ParseClaimsFromJwt(token);
                return DateTimeOffset.FromUnixTimeSeconds(long.Parse(claims.Where(x => x.Type == "exp")!.First().Value)).UtcDateTime; ;
            }
            else
                return DateTime.UtcNow.AddDays(-1);
        }

        private DateTime GetExpirationFromToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                IEnumerable<Claim> claims = ParseClaimsFromJwt(token);
                return DateTimeOffset.FromUnixTimeSeconds(long.Parse(claims.Where(x => x.Type == "exp")!.First().Value)).UtcDateTime; ;
            }
            else
                return DateTime.UtcNow.AddDays(-1);
        }

        public async Task<string> GetRole()
        {
            var token = await Ls.GetValue(tokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                IEnumerable<Claim> claims = ParseClaimsFromJwt(token);
                return claims.Where(c => c.Type == ClaimTypes.Role)!.First().Value ?? string.Empty;
            }
            else
                return string.Empty;
        }
    }
}
