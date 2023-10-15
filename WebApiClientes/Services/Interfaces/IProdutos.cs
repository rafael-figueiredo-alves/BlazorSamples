using BlazorClientes.Shared.Entities;

namespace WebApiClientes.Services.Interfaces
{
    /// <summary>
    /// Interface relacionada a entidade Produtos
    /// </summary>
    public interface IProdutos
    {
        /// <summary>
        /// Método ´para pegar lista de Produtos
        /// </summary>
        /// <param name="Page">Informações para paginação</param>
        /// <returns>Lista de Produtos</returns>
        public Task<List<Produtos>> GetProdutos(PageInfo Page);

        /// <summary>
        /// Paga Produto por ID
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Retorna Produto</returns>
        public Task<Produtos?> GetProduto(string id);

        /// <summary>
        /// Insere novo Produto
        /// </summary>
        /// <param name="Produto">Produto</param>
        /// <returns>Retorna Produto criado</returns>
        public Task<Produtos> PostProduto(Produtos Produto);

        /// <summary>
        /// Altera dados de um Produto
        /// </summary>
        /// <param name="Produto">Produto</param>
        /// <param name="ID">Id do Produto</param> 
        /// <returns>Retorna o Produto alterado</returns>
        public Task<Produtos> PutProduto(Produtos Produto, string ID);

        /// <summary>
        /// Apaga Produto
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Verdadeiro ou falso</returns>
        public Task<bool> DeleteProduto(string id);

        /// <summary>
        /// Listar pordutos filtrados por um campo e um termo
        /// </summary>
        /// <param name="FiltrarPor">Campo</param>
        /// <param name="TermoBusca">Termo</param>
        /// <param name="Page">Informações para paginação</param>
        /// <returns>Lista de Produtos</returns>
        public Task<List<Produtos>> GetProdutosPorFiltro(PageInfo Page, FiltroProdutos FiltrarPor, string? TermoBusca);
    }
}
