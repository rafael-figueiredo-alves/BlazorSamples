using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebApiClientes.Entities;
using WebApiClientes.Services;

namespace WebApiClientes.Controllers
{
    /// <summary>
    /// Endpoint Cliente
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClientes fclientes = new ClientesBD();

        /// <summary>
        /// Retorna uma lista com todos os clientes cadastrados no sistema
        /// </summary>
        /// <returns>Retorna lista de clientes</returns>
        /// <remarks>
        /// Obtenha uma relação com todos os dados de todos os clientes
        /// </remarks>
        /// <response code="200">Sucesso ao obter lista de clientes</response>
        /// <response code="404">Não foram encontrados clientes</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Clientes>>> Get()
        {
            var clientes = await fclientes.GetClientes();
            try
            {
                if (clientes != null)
                {
                    if (clientes.Any())
                    {
                        return Ok(clientes);
                    }
                    else
                    {
                        return NotFound(clientes);
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
        /// Retorna dados do cliente com ID informado
        /// </summary>
        /// <returns>Retorna dados do cliente com ID informado</returns>
        /// <remarks>
        /// Obtém dados do cliente que possua o ID informado. Caso informe um ID que não exista cadastro, você receberá mensagem que não foi possível encontrar o cliente
        /// </remarks>
        /// <param name="id">Informe o ID do cliente que deseja consultar</param>
        /// <response code="200">Sucesso ao obter lista de clientes</response>
        /// <response code="404">Não foram encontrados clientes</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clientes>> GetCliente(int id)
        {
            var clientes = await fclientes.GetCliente(id);
            try
            {
                if (clientes != null)
                {
                    if ((clientes.Nome != null) && (clientes.Endereco != null) && (clientes.Telefone != null)
                         && (clientes.Celular != null) && (clientes.Email != null))
                    {
                        return Ok(clientes);
                    }
                    else
                    {
                        return NotFound(new Erro("Nenhum cliente encontrado", "O ID informado não retornou cliente algum. Tente um outro ID."));
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
        /// Utilize este Endpoint para criar um novo cliente
        /// </summary>
        /// <remarks>
        /// Este endpoint permite adicionar novo cliente a base de dados
        /// </remarks>
        /// <param name="cliente"></param>
        /// <returns>Dados do cliente recém criado</returns>
        /// <response code="201">Cliente adicionado com sucesso</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clientes>> PostCliente([FromBody]ClienteDTO cliente)
        {
            var clientes = fclientes.PostCliente(cliente);
            try
            {
                if (clientes != null)
                {
                    return Ok(clientes);
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
    }
}
