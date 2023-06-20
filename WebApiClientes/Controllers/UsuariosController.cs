using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiClientes.Entities;
using WebApiClientes.Services;

namespace WebApiClientes.Controllers
{
    /// <summary>
    /// Endpoint dos usuários
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarios fuser = new UsuariosBD();

        private readonly IConfiguration _configuration;

        /// <summary>
        /// Método Construtor
        /// </summary>
        /// <param name="configuration">Injeção de dependência</param>
        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<UserToken>> Login(LoginUser user)
        {
            if (user != null)
            {
                try
                {
                    var usuario = await fuser.Login(user!);
                    if (usuario == null)
                    {
                        return StatusCode(500, "Ocorreu um erro em nossos servidores e não pudemos atender sua requisição. Tente novamente mais tarde.");
                    }
                    else
                    {
                        return Ok(buildToken(usuario));
                    }
                }
                catch (Exception ex)
                {
                    if(ex.Message == "NO USER")
                    {
                        return NotFound(new Erro("Usuário não encontrado", "Usuário informado não pode ser encontrado em nossa base de dados."));
                    }
                    else
                    {
                        return StatusCode(500, new Erro("Erro desconhecido", ex.Message));
                    }
                }
            }
            else
            {
                return BadRequest(new Erro("Solicitação inválida", "Não foi possível executar requisição por falta dos dados de usuário para efetuar Login."));
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserToken>> CreateUser(usuarios usuario)
        {
            if (usuario != null)
            {
                try
                {
                    var user = await fuser.CreateUser(usuario!);
                    if (user == null)
                    {
                        return StatusCode(500, "Ocorreu um erro em nossos servidores e não pudemos atender sua requisição. Tente novamente mais tarde.");
                    }
                    else
                    {
                        return Ok(buildToken(user));
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "USER EXISTS")
                    {
                        return NotFound(new Erro("E-mail informado já em uso", "Já há um usuário usando o e-amil informado. Não é possível criar outro usuário usando o mesmo e-mail. Tente fazer o login."));
                    }
                    else
                    {
                        return StatusCode(500, new Erro("Erro desconhecido", ex.Message));
                    }
                }
            }
            else
            {
                return BadRequest(new Erro("Solicitação inválida", "Não foi possível executar requisição por falta dos dados de usuário para efetuar Login."));
            };
        }

        /// <summary>
        /// Método Construtor de Token de autorização
        /// </summary>
        /// <returns></returns>
        private UserToken buildToken(usuarios usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email!),
                new Claim(usuario.Nome!, usuario.Email!),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]!),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, usuario.TipoConta!)
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
