using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorClientes.Auth
{
    public class TokenAuthenticationProvider : AuthenticationStateProvider
    {
        private AuthenticationState notAuthenticate => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(notAuthenticate);
        }
    }
}
