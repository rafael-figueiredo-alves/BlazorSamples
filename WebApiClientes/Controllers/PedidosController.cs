using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;
using WebApiClientes.Services;
using WebApiClientes.Services.Interfaces;

namespace WebApiClientes.Controllers
{
    //Os comentários com /// são usados no swagger
    /// <summary>
    /// Endpoint Pedidos
    /// </summary>
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")] //Usado para adicionar autenticação e autorização
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidos fpedidos = new PedidosBD();

        /// <summary>
        /// Retorna uma lista com todos os pedidos cadastrados no sistema
        /// </summary>
        /// <returns>Retorna lista de pedidos</returns>
        /// <remarks>
        /// Obtenha uma relação com todos os dados de todos os pedidos
        /// </remarks>
        /// <response code="200">Sucesso ao obter lista de pedidos</response>
        /// <response code="404">Não foram encontrados pedidos</response>
        /// <response code="401">Acesso não autorizado. Você precisa se autenticar para poder acessar este endpoint</response>
        /// <response code="403">Recurso só disponível para usuários autenticados com um determinado tipo de conta</response>
        /// <response code="304">Não houve mudanças nos dados, portanto o cache pode ser utilizado porque se encontra atualizado</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)] //Informa qual formato de retorno
        [ProducesResponseType(StatusCodes.Status200OK)] //Informa status codes retornáveis
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        public async Task<ActionResult<List<Pedidos>>> Get([FromQuery] FiltrosPedido? FiltrarPor, string? Termo1, string? Termo2, int? Pagina, int? QtdRegistrosPorPagina)
        {
            Response.Headers.Add("AppName", "Web Api Clientes");
            Response.Headers.Add("Version", "1.0.0");

            string dataHash;

            PageInfo Page = new();

            if (Pagina != null)
            {
                Page.Page = Pagina;
            }

            if (QtdRegistrosPorPagina != null)
            {
                Page.PageSize = QtdRegistrosPorPagina;
            }

            List<Pedidos> pedidos;

            if(FiltrarPor == null)
            {
                pedidos = await fpedidos.GetPedidos(Page);
            }
            else
            {
                pedidos = FiltrarPor switch
                {
                    FiltrosPedido.PorDataEmissao => await fpedidos.GetPedidosPorPerido(Page, "DataEmissao", Convert.ToDateTime(Termo1 ?? DateTime.Now.ToString()), Convert.ToDateTime(Termo2 ?? DateTime.Now.ToString())),
                    FiltrosPedido.PorDataEntrega => await fpedidos.GetPedidosPorPerido(Page, "DataEntrega", Convert.ToDateTime(Termo1 ?? DateTime.Now.ToString()), Convert.ToDateTime(Termo2 ?? DateTime.Now.ToString())),
                    FiltrosPedido.PorClienteID => await fpedidos.GetPedidosFiltroIgual(Page, "idCliente", Termo1 ?? "0"),
                    FiltrosPedido.PorVendedorID => await fpedidos.GetPedidosFiltroIgual(Page, "idVendedor", Termo1 ?? "0"),
                    FiltrosPedido.PorClienteNome => await fpedidos.GetPedidosFiltroLike(Page, "clientes.Nome", Termo1 ?? "%"),
                    FiltrosPedido.PorVendedorNome => await fpedidos.GetPedidosFiltroLike(Page, "vendedores.Vendedor", Termo1 ?? "%"),
                    FiltrosPedido.PorStatus => await fpedidos.GetPedidosFiltroLike(Page, "Status", Termo1 ?? "%"),
                    _ => await fpedidos.GetPedidos(Page),
                };
            }

            dataHash = HashMD5.Hash(JsonSerializer.Serialize(pedidos));

            if ((!string.IsNullOrEmpty(Request.Headers.IfNoneMatch)) && (HashMD5.VerifyETag(Request.Headers.IfNoneMatch!, dataHash)))
            {
                //Os comandos abaixo adicionam Headers personalizados
                Response.Headers.Add("PageNumber", Page.Page.ToString());
                Response.Headers.Add("PageSize", Page.PageSize.ToString());
                Response.Headers.Add("TotalRecords", Page.TotalRecords.ToString());
                Response.Headers.Add("TotalPages", Page.TotalPages.ToString());
                //Serializar e enviar o Hash no etag
                Response.Headers.ETag = dataHash;
                return StatusCode(StatusCodes.Status304NotModified, null);
            }
            else
            {
                try
                {
                    if (pedidos != null)
                    {
                        if (pedidos.Any())
                        {
                            //Os comandos abaixo adicionam Headers personalizados
                            Response.Headers.Add("PageNumber", Page.Page.ToString());
                            Response.Headers.Add("PageSize", Page.PageSize.ToString());
                            Response.Headers.Add("TotalRecords", Page.TotalRecords.ToString());
                            Response.Headers.Add("TotalPages", Page.TotalPages.ToString());
                            //Serializar e enviar o Hash no etag
                            Response.Headers.ETag = dataHash;
                            return Ok(pedidos);
                        }
                        else
                        {
                            return NotFound(pedidos);
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
                    return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
                }
            }
        }

        /// <summary>
        /// Retorna dados do pedido com ID informado
        /// </summary>
        /// <returns>Retorna dados do pedido com ID informado</returns>
        /// <remarks>
        /// Obtém dados do pedido que possua o ID informado. Caso informe um ID que não exista cadastro, você receberá mensagem que não foi possível encontrar o pedido
        /// </remarks>
        /// <param name="id">Informe o ID do pedido que deseja consultar</param>
        /// <response code="200">Sucesso ao obter lista de pedidos</response>
        /// <response code="404">Não foram encontrados pedidos</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Pedidos>> GetPedido(string id)
        {
            var pedidos = await fpedidos.GetPedido(id);
            try
            {
                if (pedidos != null)
                {
                    if ((pedidos.idPedido != null) && (pedidos.idCliente != null) && (pedidos.idVendedor != null))
                    {
                        return Ok(pedidos);
                    }
                    else
                    {
                        return NotFound(new Erro("Nenhum pedido encontrado.", "O ID informado não retornou pedido algum. Tente um outro ID."));
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
                return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
            }
        }

        /// <summary>
        /// Utilize este Endpoint para criar um novo pedido
        /// </summary>
        /// <remarks>
        /// Este endpoint permite adicionar novo pedido a base de dados
        /// </remarks>
        /// <param name="pedido">Pedido a ser adicionado</param>
        /// <returns>Dados do pedido recém criado</returns>
        /// <response code="201">Pedido adicionado com sucesso</response>
        /// <response code="400">Ocorreu algum problema com so dados informados, violando alguma regra de validação</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)] //Mostra qual formatyo será consumido
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Pedidos>> PostPedido([FromBody] Pedidos pedido)
        {
            var pedidos = await fpedidos.PostPedido(pedido);
            try
            {
                if (pedidos != null)
                {
                    return Created($"/v1/Pedidos/{pedidos.idPedido}", pedidos);
                }
                else
                {
                    return StatusCode(500, new Erro("Houve um erro interno com o servidor",
                                                    "Ocorreu um problema inesperado em nosso servidor, tente acessar nossa API mais tarde."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
            }
        }

        /// <summary>
        /// Este método é utilizado para atualizar o registro do pedido informado.
        /// </summary>
        /// <param name="id">Id do pedido</param>
        /// <param name="pedido">Dados do pedido</param>
        /// <returns>Dados do pedido atualizado</returns>
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
        public async Task<ActionResult<Pedidos>> PutPedido(string id, [FromBody] Pedidos pedido)
        {
            var pedidos = await fpedidos.PutPedido(pedido, id);
            try
            {
                if (pedidos != null)
                {
                    if (pedidos.isNewRecord)
                    {
                        return BadRequest(new Erro("Registro não encontrado", "O id informado é inválido e por isso não foi possível alterar o pedido informado"));
                    }
                    else
                        return Ok(pedidos);
                }
                else
                {
                    return StatusCode(500, new Erro("Houve um erro interno com o servidor",
                                                    "Ocorreu um problema inesperado em nosso servidor, tente acessar nossa API mais tarde."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
            }
        }

        /// <summary>
        /// Utilize este método para apagar um pedido
        /// </summary>
        /// <param name="id">ID do pedido</param>
        /// <returns>Não retorna conteúdo, apenas status code = 204</returns>
        /// <response code="204">Pedido apagado com sucesso</response>
        /// <response code="400">Ocorreu um erro com os dados informados que não são válidos</response>
        /// <response code="500">Ocorreu um erro inesperado no servidor</response>
        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePedido(string id)
        {
            bool Apagou = await fpedidos.DeletePedido(id);
            try
            {
                if (Apagou)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest(new Erro("Não foi possível apagar pedido", "O id informado é inválido e por isso não foi possível apagar o pedido informado"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
            }
        }

        /// <summary>
        /// Este método é utilizado para atualizar o status do pedido informado.
        /// </summary>
        /// <param name="id">Id do pedido</param>
        /// <param name="pedidoStatus">Status do pedido</param>
        /// <returns>Dados do pedido atualizado</returns>
        /// <response code="200">Dados atualizados com sucesso</response>
        /// <response code="400">Ocorreu um erro com os dados informados que não são válidos</response>
        /// <response code="500">Ocorreu um erro inesperado no servidor</response>
        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Pedidos>> PutPedido([FromQuery] string id, string pedidoStatus)
        {
            var pedidos = await fpedidos.SetPedidoStatus(id, pedidoStatus);
            try
            {
                if (pedidos != null)
                {
                    if(pedidos.isNewRecord)
                    {
                        return BadRequest(new Erro("Registro não encontrado", "O id informado é inválido e por isso não foi possível alterar status do pedido informado"));
                    }
                    else
                     return Ok(pedidos);
                }
                else
                {
                    return StatusCode(500, new Erro("Houve um erro interno com o servidor",
                                                    "Ocorreu um problema inesperado em nosso servidor, tente acessar nossa API mais tarde."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
            }
        }
    }
}
