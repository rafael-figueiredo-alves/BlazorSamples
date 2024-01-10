using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Enums;
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
    /// Endpoint para gerenciamento de Produtos
    /// </summary>
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")] //Usado para adicionar autenticação e autorização
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutos fprodutos = new ProdutosBD();

        #region Read Endpoints
        /// <summary>
        /// Retorna uma lista com todos os produtos cadastrados no sistema
        /// </summary>
        /// <returns>Retorna lista de produtos</returns>
        /// <remarks>
        /// Obtenha uma relação com todos os dados de todos os produtos
        /// </remarks>
        /// <response code="200">Sucesso ao obter lista de produtos</response>
        /// <response code="304">Não houve mudanças nos dados, portanto o cache pode ser utilizado porque se encontra atualizado</response>
        /// <response code="401">Acesso não autorizado. Você precisa se autenticar para poder acessar este endpoint</response>
        /// <response code="403">Recurso só disponível para usuários autenticados com um determinado tipo de conta</response>
        /// <response code="404">Não foram encontrados produtos</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)] //Informa qual formato de retorno
        [ProducesResponseType(StatusCodes.Status200OK)] //Informa status codes retornáveis
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Produtos>>> Get([FromQuery] FiltroProdutos? FiltrarPor, string? Termo, int? Pagina, int? QtdRegistrosPorPagina)
        {
            Response.Headers.Add("Access-Control-Expose-Headers", "ETag,AppName,Version,PageNumber,PageSize,TotalRecords,TotalPages");
            Response.Headers.Add("AppName", "Web Api Clientes");
            Response.Headers.Add("Version", "1.0.0");

            string dataHash;

            List<Produtos> produtos;

            PageInfo Page = new();

            if (Pagina != null)
            {
                Page.Page = Pagina;
            }

            if (QtdRegistrosPorPagina != null)
            {
                Page.PageSize = QtdRegistrosPorPagina;
            }

            if ((FiltrarPor == null) && (string.IsNullOrEmpty(Termo)))
            {
                produtos = await fprodutos.GetProdutos(Page);
            }
            else
            {
                produtos = await fprodutos.GetProdutosPorFiltro(Page, (FiltroProdutos)FiltrarPor!, Termo);
            }

            dataHash = HashMD5.Hash(JsonSerializer.Serialize(produtos));

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
                    if (produtos != null)
                    {
                        if (produtos.Any())
                        {
                            //Os comandos abaixo adicionam Headers personalizados
                            Response.Headers.Add("PageNumber", Page.Page.ToString());
                            Response.Headers.Add("PageSize", Page.PageSize.ToString());
                            Response.Headers.Add("TotalRecords", Page.TotalRecords.ToString());
                            Response.Headers.Add("TotalPages", Page.TotalPages.ToString());
                            //Serializar e enviar o Hash no etag
                            Response.Headers.ETag = dataHash;
                            return Ok(produtos);
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
                            return Ok(produtos);
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
        /// Retorna dados do produto com ID informado
        /// </summary>
        /// <returns>Retorna dados do produto com ID informado</returns>
        /// <remarks>
        /// Obtém dados do produto que possua o ID informado. Caso informe um ID que não exista cadastro, você receberá mensagem que não foi possível encontrar o produto
        /// </remarks>
        /// <param name="id">Informe o ID do produto que deseja consultar</param>
        /// <param name=""></param>
        /// <param name="Kind"></param>
        /// <response code="200">Sucesso ao obter lista de produtos</response>
        /// <response code="404">Não foram encontrados produtos</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Produtos>> GetProduto(string id, [FromQuery] GetKind Kind = GetKind.PorCodigo)
        {
            var produtos = await fprodutos.GetProduto(id, Kind);
            try
            {
                if (produtos != null)
                {
                    if ((produtos.Produto != null) && (produtos.Descricao != null))
                    {
                        return Ok(produtos);
                    }
                    else
                    {
                        return NotFound(new Erro("Nenhum produto encontrado", "O ID informado não retornou produto algum. Tente um outro ID."));
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
        /// Retorna uma lista com todos os produtos cadastrados no sistema
        /// </summary>
        /// <returns>Retorna lista de produtos para imprimir</returns>
        /// <remarks>
        /// Obtenha uma relação com todos os dados de todos os produtos
        /// </remarks>
        /// <response code="200">Sucesso ao obter lista de produtos</response>
        /// <response code="401">Acesso não autorizado. Você precisa se autenticar para poder acessar este endpoint</response>
        /// <response code="403">Recurso só disponível para usuários autenticados com um determinado tipo de conta</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpGet("print")]
        [Produces(MediaTypeNames.Application.Json)] //Informa qual formato de retorno
        [ProducesResponseType(StatusCodes.Status200OK)] //Informa status codes retornáveis
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Produtos>>> GetAllVendedoresToPrint()
        {
            Response.Headers.Add("Access-Control-Expose-Headers", "ETag,AppName,Version,PageNumber,PageSize,TotalRecords,TotalPages");
            Response.Headers.Add("AppName", "Web Api Clientes");
            Response.Headers.Add("Version", "1.0.0");

            List<Produtos> produtos = await fprodutos.GetAllProdutosToPrint();

            try
            {
                if (produtos != null)
                {
                    if (produtos.Any())
                    {
                        return Ok(produtos);
                    }
                    else
                    {
                        return Ok(produtos);
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
        #endregion

        #region Insert/Update/Delete endpoints
        /// <summary>
        /// Utilize este Endpoint para criar um novo produto
        /// </summary>
        /// <remarks>
        /// Este endpoint permite adicionar novo produto a base de dados
        /// </remarks>
        /// <param name="produto">produto</param>
        /// <returns>Dados do produto recém criado</returns>
        /// <response code="201">Produto adicionado com sucesso</response>
        /// <response code="400">Ocorreu algum problema com so dados informados, violando alguma regra de validação</response>
        /// <response code="500">Ocorreu um erro interno no servidor</response>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)] //Mostra qual formato será consumido
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Produtos>> PostVendedores([FromBody] Produtos produto)
        {
            var produtos = await fprodutos.PostProduto(produto);
            try
            {
                if (produtos != null)
                {
                    return Created($"/v1/Produtos/{produtos.idProduto}", produtos);
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
        /// Este método é utilizado para atualizar o registro do produto informado.
        /// </summary>
        /// <param name="id">Id do produto</param>
        /// <param name="produto">Dados do produto</param>
        /// <returns>Dados do produto atualizado</returns>
        /// <response code="200">Dados atualizados com sucesso</response>
        /// <response code="400">Ocorreu um erro com os dados informados que não são válidos</response>
        /// <response code="412">Produto informado não corresponde a entidade encontrada</response>
        /// <response code="500">Ocorreu um erro inesperado no servidor</response>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Produtos>> PutProduto(string id, [FromBody] Produtos produto)
        {
            if (!string.IsNullOrEmpty(Request.Headers.IfMatch))
            {
                string dataHash;

                var Produto = await fprodutos.GetProduto(id);

                if (Produto == null)
                {
                    return NotFound(new Erro("Produto não encontrado!", "Produto que foi solicitado alteração não foi encontrado na base de dados."));
                }

                Produto.ETag = null;
                dataHash = HashMD5.Hash(JsonSerializer.Serialize(Produto));

                if ((!string.IsNullOrEmpty(Request.Headers.IfMatch)) && (!HashMD5.VerifyETag(Request.Headers.IfMatch!, dataHash)))
                {
                    return StatusCode(StatusCodes.Status412PreconditionFailed, new Erro("Não foi possível alterar produto", "O produto que foi solicitado alteração não corresponde com a entidade encontrada no banco de dados. Não é possível alterar produto."));
                }
            }
            
            try
            {
                var produtos = await fprodutos.PutProduto(produto, id);
                if (produtos != null)
                {
                    return Ok(produtos);
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
        /// Utilize este método para apagar um produto
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <returns>Não retorna conteúdo, apenas status code = 204</returns>
        /// <response code="204">Produto apagado com sucesso</response>
        /// <response code="400">Ocorreu um erro com os dados informados que não são válidos</response>
        /// <response code="412">Produto informado não corresponde a entidade encontrada</response>
        /// <response code="500">Ocorreu um erro inesperado no servidor</response>
        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesErrorResponseType(typeof(Erro))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteProduto(string id)
        {
            if (!string.IsNullOrEmpty(Request.Headers.IfMatch))
            {
                string dataHash;

                var Produto = await fprodutos.GetProduto(id);

                if (Produto == null)
                {
                    return NotFound(new Erro("Produto não encontrado!", "Produto que foi solicitado remoção não foi encontrado na base de dados."));
                }

                Produto.ETag = null;
                dataHash = HashMD5.Hash(JsonSerializer.Serialize(Produto));

                if ((!string.IsNullOrEmpty(Request.Headers.IfMatch)) && (!HashMD5.VerifyETag(Request.Headers.IfMatch!, dataHash)))
                {
                    return StatusCode(StatusCodes.Status412PreconditionFailed, new Erro("Não foi possível apagar produto", "O produto que foi solicitada remoção não corresponde com a entidade encontrada no banco de dados. Não é possível apagar produto."));
                }
            }

            
            try
            {
                bool Apagou = await fprodutos.DeleteProduto(id);
                if (Apagou)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest(new Erro("Não foi possível apagar produto.", "O id informado é inválido e por isso não foi possível apagar o produto informado."));
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
