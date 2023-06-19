using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiClientes.Entities;

namespace WebApiClientes.Controllers
{
    /// <summary>
    /// Endpoint dos usuários
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        /// <summary>
        /// Método Construtor
        /// </summary>
        /// <param name="configuration">Injeção de dependência</param>
        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<UserToken> GetUsers()
        {
            return Ok(buildToken());
        }

        /// <summary>
        /// Método Construtor de Token de autorização
        /// </summary>
        /// <returns></returns>
        private UserToken buildToken()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, "devpegasus@outlook.com"),
                new Claim("DevPegasus", "devpegasus@outlook.com"),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]!),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Adiciona 2 horas para expirar o token
            var Expiration = DateTime.UtcNow.AddHours(2);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                signingCredentials: creds,
                expires: Expiration);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiraEm = Expiration
            };
        }
    }
}
