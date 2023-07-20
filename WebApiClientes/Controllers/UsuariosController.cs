using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySqlX.XDevAPI;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
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
    [Authorize(AuthenticationSchemes = "Bearer")]
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

        /// <summary>
        /// Método responsável por realizar o login do usuário e obter o Token de acesso
        /// </summary>
        /// <param name="user">Dados necessários para o login</param>
        /// <returns>O Token de acesso</returns>
        /// <response code="200">Login efetuado com sucesso</response>
        /// <response code="404">Não foi possível encontrar o usuário informado</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpPost]
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                        return Ok(BuildToken(usuario));
                    }
                }
                catch (Exception ex)
                {
                    if(ex.Message == "NO USER")
                    {
                        return NotFound(new Erro("Usuário não encontrado", "Usuário informado não pôde ser encontrado em nossa base de dados."));
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
        /// Método para registrar um novo usuário no sistema
        /// </summary>
        /// <param name="usuario">Dados do Usuário a ser registrado</param>
        /// <returns>O Token de acesso</returns>
        /// <response code="201">Usuário registrado com sucesso</response>
        /// <response code="400">Ocorreu algum problema com os dados informados, violando alguma regra de validação, ou usuário já existente</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                        return Created($"{user.ID!}", BuildToken(user));
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "USER EXISTS")
                    {
                        return BadRequest(new Erro("E-mail informado já em uso", "Já há um usuário usando o e-mail informado. Não é possível criar outro usuário usando o mesmo e-mail. Tente fazer o login ao invés de criar outra conta."));
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
        /// Endpoint para retornar o perfil do usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        [HttpGet("{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<usuarios>> GetProfile(int id)
        {
            var Usuario = await fuser.UserProfile(id);
            try
            {
                if (Usuario != null)
                {
                    if ((Usuario.Nome != null) && (Usuario.Email != null))
                    {
                        return Ok(Usuario);
                    }
                    else
                    {
                        return NotFound(new Erro("Nenhum usuário encontrado", "O ID informado não retornou usuário algum. Tente um outro ID."));
                    }
                }
                else
                {
                    return StatusCode(500, new Erro("Houve um erro interno com o servidor",
                                                    "Ocorreu um problema inesperado em nosso servidor, tente acessar nossa API mais tarde."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint para trocar senha do usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <returns>Status 204</returns>
        [HttpPut("password")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangePassword([FromQuery] int id, [FromQuery] string senha)
        {
            bool TrocouSenha = await fuser.ChangePassword(id, senha);
            if (TrocouSenha) 
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, new Erro("Houve um erro interno com o servidor",
                                                "Ocorreu um problema inesperado em nosso servidor, tente acessar nossa API mais tarde."));
            }
        }

        /// <summary>
        /// Endpoint para trocar tipo de conta
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="tipo">Tipo de conta</param>
        /// <returns>Status 204</returns>
        [HttpPut("account")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangeAccountType([FromQuery] int id, [FromQuery] string tipo)
        {
            if(tipo == null)
            {
                return BadRequest(new Erro("Faltou especificar tipo", "Tipo de conta não foi informado e é necessário para prosseguir."));
            }

            if (tipo != "Admin")
            {
                if (tipo != "User")
                {
                    return BadRequest(new Erro("Tipo de conta não válido", "O tipo de conta informado não é válido. Tente novamente. Valores válidos: Admin ou User."));
                }
            }

            bool TrocouTipoConta = await fuser.ChangeAccountType(id, tipo);
            if (TrocouTipoConta)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, new Erro("Houve um erro interno com o servidor",
                                                "Ocorreu um problema inesperado em nosso servidor, tente acessar nossa API mais tarde."));
            }
        }


        /// <summary>
        /// Método Construtor de Token de autorização
        /// </summary>
        /// <returns></returns>
        private UserToken BuildToken(usuarios usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email!),
                new Claim("Username", usuario.Nome!),
                new Claim("uID", usuario.ID!.ToString()),
                new Claim("Email", usuario.Email!),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]!),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, usuario.TipoConta!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Adiciona 2 horas para expirar o token
            var Expiration = DateTime.UtcNow.AddHours(2);

            JwtSecurityToken token = new(
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
