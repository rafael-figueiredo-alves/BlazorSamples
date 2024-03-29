﻿using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Enums;
using BlazorClientes.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using System.Net.Mime;
using System.Text.Json;
using WebApiClientes.Services;
using WebApiClientes.Services.Interfaces;

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

        #region Read endpoints
        /// <summary>
        /// Retorna uma lista com todos os clientes cadastrados no sistema
        /// </summary>
        /// <returns>Retorna lista de clientes</returns>
        /// <remarks>
        /// Obtenha uma relação com todos os dados de todos os clientes
        /// </remarks>
        /// <response code="200">Sucesso ao obter lista de clientes</response>
        /// <response code="304">Não houve mudanças nos dados, portanto o cache pode ser utilizado porque se encontra atualizado</response>
        /// <response code="401">Acesso não autorizado. Você precisa se autenticar para poder acessar este endpoint</response>
        /// <response code="403">Recurso só disponível para usuários autenticados com um determinado tipo de conta</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)] //Informa qual formato de retorno
        [ProducesResponseType(StatusCodes.Status200OK)] //Informa status codes retornáveis
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Clientes>>> Get([FromQuery] FiltrosCliente? FiltrarPor, string? Termo, int? Pagina, int? QtdRegistrosPorPagina)
        {
            Response.Headers.Add("Access-Control-Expose-Headers", "ETag,AppName,Version,PageNumber,PageSize,TotalRecords,TotalPages");
            Response.Headers.Add("AppName", "Web Api Clientes");
            Response.Headers.Add("Version", "1.0.0");

            string dataHash;

            List<Clientes> clientes;

            PageInfo Page = new();

            if(Pagina != null)
            {
                Page.Page = Pagina;
            }

            if(QtdRegistrosPorPagina != null)
            {
                Page.PageSize = QtdRegistrosPorPagina;
            }

            if ((FiltrarPor == null) && (string.IsNullOrEmpty(Termo)))
            {
                clientes = await fclientes.GetClientes(Page);
            }
            else
            {
                clientes = await fclientes.GetClientesPorFiltro(Page, (FiltrosCliente)FiltrarPor!, Termo);
            }

            dataHash = HashMD5.Hash(JsonSerializer.Serialize(clientes));

            if ((!string.IsNullOrEmpty(Request.Headers.IfNoneMatch)) && (HashMD5.VerifyETag(Request.Headers.IfNoneMatch!, dataHash)))
            {
                //Os comandos abaixo adicionam Headers personalizados
                Response.Headers.Add("PageNumber", Page.Page.ToString());
                Response.Headers.Add("PageSize", Page.PageSize.ToString());
                Response.Headers.Add("TotalRecords", Page.TotalRecords.ToString());
                Response.Headers.Add("TotalPages", Page.TotalPages.ToString());
                //Serializar e enviar o Hash no etag
                HttpContext.Response.Headers.ETag = dataHash;

                //HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "PageNumber,PageSize,TotalRecords,TotalPages");
                return StatusCode(StatusCodes.Status304NotModified, null);
            }
            else
            {
                try
                {
                    if (clientes != null)
                    {
                        if (clientes.Any())
                        {
                            //Os comandos abaixo adicionam Headers personalizados
                            Response.Headers.Add("PageNumber", Page.Page.ToString());
                            Response.Headers.Add("PageSize", Page.PageSize.ToString());
                            Response.Headers.Add("TotalRecords", Page.TotalRecords.ToString());
                            Response.Headers.Add("TotalPages", Page.TotalPages.ToString());
                            //Serializar e enviar o Hash no etag
                            Response.Headers.ETag = dataHash;
                            return Ok(clientes);
                        }
                        else
                        {
                            //Os comandos abaixo adicionam Headers personalizados
                            Response.Headers.Add("PageNumber", Page.Page.ToString());
                            Response.Headers.Add("PageSize", Page.PageSize.ToString());
                            Response.Headers.Add("TotalRecords", Page.TotalRecords.ToString());
                            Response.Headers.Add("TotalPages", Page.TotalPages.ToString());
                            //Serializar e enviar o Hash no etag
                            Response.Headers.ETag = dataHash;
                            return Ok(clientes);
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
        /// Retorna uma lista com todos os clientes cadastrados no sistema
        /// </summary>
        /// <returns>Retorna lista de clientes</returns>
        /// <remarks>
        /// Obtenha uma relação com todos os dados de todos os clientes
        /// </remarks>
        /// <response code="200">Sucesso ao obter lista de clientes</response>
        /// <response code="401">Acesso não autorizado. Você precisa se autenticar para poder acessar este endpoint</response>
        /// <response code="403">Recurso só disponível para usuários autenticados com um determinado tipo de conta</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet("print")]
        [Produces(MediaTypeNames.Application.Json)] //Informa qual formato de retorno
        [ProducesResponseType(StatusCodes.Status200OK)] //Informa status codes retornáveis
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Clientes>>> GetClientesToPrint()
        {
            Response.Headers.Add("Access-Control-Expose-Headers", "ETag,AppName,Version,PageNumber,PageSize,TotalRecords,TotalPages");
            Response.Headers.Add("AppName", "Web Api Clientes");
            Response.Headers.Add("Version", "1.0.0");

            List<Clientes> clientes = await fclientes.GetClientesToPrint();

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
                        return Ok(clientes);
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
        /// Retorna dados do cliente com ID informado
        /// </summary>
        /// <returns>Retorna dados do cliente com ID informado</returns>
        /// <remarks>
        /// Obtém dados do cliente que possua o ID informado. Caso informe um ID que não exista cadastro, você receberá mensagem que não foi possível encontrar o cliente
        /// </remarks>
        /// <param name="id">Informe o ID do cliente que deseja consultar</param>
        /// <param name="Kind">Tipo</param>
        /// <response code="200">Sucesso ao obter lista de clientes</response>
        /// <response code="404">Não foram encontrados clientes</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clientes>> GetCliente(string id, [FromQuery] GetKind Kind = GetKind.PorCodigo)
        {
            string dataHash;

            try
            {
                var clientes = await fclientes.GetCliente(id, Kind);

                if(clientes == null)
                {
                    return NotFound(new Erro("Cliente não encontrado.", "Cliente solicitado não foi encontrado na base de dados."));
                }

                dataHash = HashMD5.Hash(JsonSerializer.Serialize(clientes));

                if ((!string.IsNullOrEmpty(Request.Headers.IfNoneMatch)) && (HashMD5.VerifyETag(Request.Headers.IfNoneMatch!, dataHash)))
                {
                    return StatusCode(StatusCodes.Status304NotModified, null);
                }
                else
                {
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
                        return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
            }
        }
        #endregion

        #region Insert/Update/Delete Endpoints
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
                return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
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
        /// <response code="412">Entidade informada não corresponde a entidade encontrada no banco de dados</response>
        /// <response code="500">Ocorreu um erro inesperado no servidor</response>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Clientes>> PutCliente(string id, [FromBody]Clientes cliente)
        {
            if (!string.IsNullOrEmpty(Request.Headers.IfMatch))
            {
                string dataHash;

                var Clientes = await fclientes.GetCliente(id);

                if (Clientes == null)
                {
                    return NotFound(new Erro("Cliente não encontrado!", "Cliente que foi solicitado alteração não foi encontrado na base de dados."));
                }

                Clientes.ETag = null;
                dataHash = HashMD5.Hash(JsonSerializer.Serialize(Clientes));

                if ((!string.IsNullOrEmpty(Request.Headers.IfMatch)) && (!HashMD5.VerifyETag(Request.Headers.IfMatch!, dataHash)))
                {
                    return StatusCode(StatusCodes.Status412PreconditionFailed, new Erro("Não foi possível alterar cliente", "O cliente que foi solicitado alteração não corresponde com a entidade encontrada no banco de dados. Não é possível alterar cliente."));
                }
            }

            try
            {
                var clientes = await fclientes.PutCliente(cliente, id);
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
                return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
            }
        }

        /// <summary>
        /// Utilize este método para apagar um cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Não retorna conteúdo, apenas status code = 204</returns>
        /// <response code="204">Cliente apagado com sucesso</response>
        /// <response code="400">Ocorreu um erro com os dados informados que não são válidos</response>
        /// <response code="412">Entidade encontrada nãom corresponde com entidade informada</response>
        /// <response code="500">Ocorreu um erro inesperado no servidor</response>
        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCliente(string id)
        {
            if(!string.IsNullOrEmpty(Request.Headers.IfMatch))
            {
                string dataHash;

                var clientes = await fclientes.GetCliente(id);

                if(clientes == null)
                {
                    return NotFound(new Erro("Cliente não encontrado!", "Cliente que foi solicitado remoção não foi encontrado na base de dados."));
                }

                clientes.ETag = null;
                dataHash = HashMD5.Hash(JsonSerializer.Serialize(clientes));

                if ((!string.IsNullOrEmpty(Request.Headers.IfMatch)) && (!HashMD5.VerifyETag(Request.Headers.IfMatch!, dataHash)))
                {
                    return StatusCode(StatusCodes.Status412PreconditionFailed, new Erro("Não foi possível apagar cliente", "O cliente que foi solicitada remoção não corresponde com a entidade encontrada no banco de dados. Não é possível apagar cliente."));
                }
            }

            
            try
            {
                bool Apagou = await fclientes.DeleteCliente(id);
                if (Apagou)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest(new Erro("Não foi possível apagar cliente", "O id informado é inválido e por isso não foi possível apagar o cliente informado"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Erro("Houve um erro interno com o servidor", ex.Message));
            }
        }
        #endregion
    }
}
