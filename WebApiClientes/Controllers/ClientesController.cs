using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebApiClientes.Entities;
using WebApiClientes.Services;

namespace WebApiClientes.Controllers
{
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
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Clientes>>> Get()
        {
            var clientes = await fclientes.GetClientes();
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
