using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;
using WebApiClientes.Services;

namespace WebApiClientes.Controllers
{
    //Os comentários com /// são usados no swagger
    /// <summary>
    /// Endpoint para gerenciamento de Vendedores
    /// </summary>
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")] //Usado para adicionar autenticação e autorização
    [ApiController]
    public class VendedoresController : ControllerBase
    {
        private readonly IVendedores fvendedores = new VendedoresBD();

        /// <summary>
        /// Retorna uma lista com todos os vendedores cadastrados no sistema
        /// </summary>
        /// <returns>Retorna lista de vendedores</returns>
        /// <remarks>
        /// Obtenha uma relação com todos os dados de todos os vendedores
        /// </remarks>
        /// <response code="200">Sucesso ao obter lista de vendedores</response>
        /// <response code="404">Não foram encontrados vendedores</response>
        /// <response code="401">Acesso não autorizado. Você precisa se autenticar para poder acessar este endpoint</response>
        /// <response code="403">Recurso só disponível para usuários autenticados com um determinado tipo de conta</response>
        /// <response code="304">Não houve mudanças nos dados, portanto o cache pode ser utilizado porque se encontra atualizado</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)] //Informa qual formato de retorno
        [ProducesResponseType(StatusCodes.Status200OK)] //Informa status codes retornáveis
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Vendedores>>> Get([FromQuery] FiltroVendedor? FiltrarPor, string? TermoBusca, int? Pagina, int? QtdRegistrosPorPagina)
        {
            Response.Headers.Add("AppName", "Web Api Clientes");
            Response.Headers.Add("Version", "1.0.0");

            string dataHash;

            List<Vendedores> vendedores;

            PageInfo Page = new();

            if (Pagina != null)
            {
                Page.Page = Pagina;
            }

            if (QtdRegistrosPorPagina != null)
            {
                Page.PageSize = QtdRegistrosPorPagina;
            }

            if ((FiltrarPor == null) && (string.IsNullOrEmpty(TermoBusca)))
            {
                vendedores = await fvendedores.GetVendedores(Page);
            }
            else
            {
                vendedores = await fvendedores.GetVendedoresPorFiltro(Page, (FiltroVendedor)FiltrarPor!, TermoBusca);
            }

            dataHash = HashMD5.Hash(JsonSerializer.Serialize(vendedores));

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
                    if (vendedores != null)
                    {
                        if (vendedores.Any())
                        {
                            //Os comandos abaixo adicionam Headers personalizados
                            Response.Headers.Add("PageNumber", Page.Page.ToString());
                            Response.Headers.Add("PageSize", Page.PageSize.ToString());
                            Response.Headers.Add("TotalRecords", Page.TotalRecords.ToString());
                            Response.Headers.Add("TotalPages", Page.TotalPages.ToString());
                            //Serializar e enviar o Hash no etag
                            Response.Headers.ETag = dataHash;
                            return Ok(vendedores);
                        }
                        else
                        {
                            return NotFound(vendedores);
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
        }

        /// <summary>
        /// Retorna dados do vendedor com ID informado
        /// </summary>
        /// <returns>Retorna dados do vendedor com ID informado</returns>
        /// <remarks>
        /// Obtém dados do vendedor que possua o ID informado. Caso informe um ID que não exista cadastro, você receberá mensagem que não foi possível encontrar o vendedor
        /// </remarks>
        /// <param name="id">Informe o ID do vendedor que deseja consultar</param>
        /// <response code="200">Sucesso ao obter lista de vendedores</response>
        /// <response code="404">Não foram encontrados vendedores</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Vendedores>> GetVendedor(string id)
        {
            var vendedores = await fvendedores.GetVendedor(id);
            try
            {
                if (vendedores != null)
                {
                    if ((vendedores.Vendedor != null))
                    {
                        return Ok(vendedores);
                    }
                    else
                    {
                        return NotFound(new Erro("Nenhum vendedor encontrado", "O ID informado não retornou vendedor algum. Tente um outro ID."));
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
        /// Utilize este Endpoint para criar um novo vendedor
        /// </summary>
        /// <remarks>
        /// Este endpoint permite adicionar novo vendedor a base de dados
        /// </remarks>
        /// <param name="vendedor">Vendedor</param>
        /// <returns>Dados do vendedor recém criado</returns>
        /// <response code="201">Vendedor adicionado com sucesso</response>
        /// <response code="400">Ocorreu algum problema com so dados informados, violando alguma regra de validação</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)] //Mostra qual formato será consumido
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Vendedores>> PostVendedores([FromBody] Vendedores vendedor)
        {
            var vendedores = await fvendedores.PostVendedor(vendedor);
            try
            {
                if (vendedores != null)
                {
                    return Created($"/v1/Vendedores/{vendedores.idVendedor}", vendedores);
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
        /// Este método é utilizado para atualizar o registro do vendedor informado.
        /// </summary>
        /// <param name="id">Id do vendedor</param>
        /// <param name="vendedor">Dados do vendedor</param>
        /// <returns>Dados do vendedor atualizado</returns>
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
        public async Task<ActionResult<Vendedores>> PutVendedor(string id, [FromBody] Vendedores vendedor)
        {
            var vendedores = await fvendedores.PutVendedor(vendedor, id);
            try
            {
                if (vendedores != null)
                {
                    return Ok(vendedores);
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
        /// Utilize este método para apagar um vendedor
        /// </summary>
        /// <param name="id">ID do vendedor</param>
        /// <returns>Não retorna conteúdo, apenas status code = 204</returns>
        /// <response code="204">Vendedor apagado com sucesso</response>
        /// <response code="400">Ocorreu um erro com os dados informados que não são válidos</response>
        /// <response code="500">Ocorreu um erro inesperado no servidor</response>
        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteVendedor(string id)
        {
            bool Apagou = await fvendedores.DeleteVendedor(id);
            try
            {
                if (Apagou)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest(new Erro("Não foi possível apagar vendedor", "O id informado é inválido e por isso não foi possível apagar o vendedor informado."));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
