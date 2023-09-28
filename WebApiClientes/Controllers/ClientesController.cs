using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using BlazorClientes.Shared.Entities;
using WebApiClientes.Services;
using System.Collections.Generic;

namespace WebApiClientes.Controllers
{
    //Os comentários com /// são usados no swagger
    /// <summary>
    /// Endpoint Cliente
    /// </summary>
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")] //Usado para adicionar autenticação e autorização
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
        [Produces(MediaTypeNames.Application.Json)] //Informa qual formato de retorno
        [ProducesResponseType(StatusCodes.Status200OK)] //Informa status codes retornáveis
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Clientes>>> Get([FromQuery] FiltrosCliente? FiltrarPor, string? Termo)
        {
            List<Clientes> clientes;

            if ((FiltrarPor == null) && (string.IsNullOrEmpty(Termo)))
            {
                clientes = await fclientes.GetClientes();
            }
            else
            {
                clientes = await fclientes.GetClientesPorFiltro((FiltrosCliente)FiltrarPor!, Termo);
            }
             
            try
            {
                if (clientes != null)
                {
                    if (clientes.Any())
                    {
                        //Os dois comandos abaixo adicionam Headers personalizados
                        Response.Headers.Add("AppName", "Web Api Clientes");
                        Response.Headers.Add("Version", "1.0.0");
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
        public async Task<ActionResult<Clientes>> GetCliente(string id)
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
        /// <response code="400">Ocorreu algum problema com so dados informados, violando alguma regra de validação</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)] //Mostra qual formatyo será consumido
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clientes>> PostCliente([FromBody]Clientes cliente)
        {
            var clientes = await fclientes.PostCliente(cliente);
            try
            {
                if (clientes != null)
                {
                    return Created($"/v1/Clientes/{clientes.idCliente}", clientes);
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
        /// Este método é utilizado para atualizar o registro do cliente informado.
        /// </summary>
        /// <param name="id">Id do cliente</param>
        /// <param name="cliente">Dados do cliente</param>
        /// <returns>Dados do cliente atualizado</returns>
        /// <response code="200">Dados atualizados com sucesso</response>
        /// <response code="400">Ocorreu um erro com os dados informados que não são válidos</response>
        /// <response code="500">Ocorreu um erro inesperado no servidor</response>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clientes>> PutCliente(string id, [FromBody]Clientes cliente)
        {
            var clientes = await fclientes.PutCliente(cliente, id);
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

        /// <summary>
        /// Utilize este método para apagar um cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Não retorna conteúdo, apenas status code = 204</returns>
        /// <response code="204">Cliente apagado com sucesso</response>
        /// <response code="400">Ocorreu um erro com os dados informados que não são válidos</response>
        /// <response code="500">Ocorreu um erro inesperado no servidor</response>
        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCliente(string id)
        {
            bool Apagou = await fclientes.DeleteCliente(id);
            try
            {
                if (Apagou)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest(new Erro("Não foi possível apagar cliente", "O id informado é inválido e por isso não foi possível apagar o cliente informado"));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
