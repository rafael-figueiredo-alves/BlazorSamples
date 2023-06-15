using Microsoft.AspNetCore.Mvc;
using WebApiClientes.Entities;
using WebApiClientes.Services;

namespace WebApiClientes.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClientes fclientes = new ClientesBD();

        [HttpGet]
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
                    return StatusCode(500, "Problemas com o servidor");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
