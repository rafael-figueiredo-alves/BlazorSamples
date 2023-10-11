using BlazorClientes.Shared.Entities;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Interface para entidade Pedidos
    /// </summary>
    public interface IPedidos
    {
        /// <summary>
        /// Método ´para pegar lista de Pedidos
        /// </summary>
        /// <param name="Page">Informações para paginação</param>
        /// <returns>Lista de Pedidos</returns>
        public Task<List<Pedidos>> GetPedidos(PageInfo Page);

        /// <summary>
        /// Paga Pedido por ID
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Retorna Pedido</returns>
        public Task<Pedidos> GetPedido(string id);

        /// <summary>
        /// Insere novo Pedido
        /// </summary>
        /// <param name="Pedido">Pedido</param>
        /// <returns>Retorna Pedido criado</returns>
        public Task<Pedidos> PostPedido(Pedidos Pedido);

        /// <summary>
        /// Altera dados de um Pedido
        /// </summary>
        /// <param name="Pedido">Pedido</param>
        /// <param name="ID">Id do Pedido</param> 
        /// <returns>Retorna o Pedido alterado</returns>
        public Task<Pedidos> PutPedido(Pedidos Pedido, string ID);

        /// <summary>
        /// Apaga Pedido
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Verdadeiro ou falso</returns>
        public Task<bool> DeletePedido(string id);

        /// <summary>
        /// Troca Status do Pedido
        /// </summary>
        /// <param name="ID">Id do Pedido</param>
        /// <param name="PedidoStatus">Novo Status</param>
        /// <returns>Pedido atualizado</returns>
        public Task<Pedidos> SetPedidoStatus(string ID, string PedidoStatus);

        /// <summary>
        /// Retorna lista de pedidos por filtro igual
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        public Task<List<Pedidos>> GetPedidosFiltroIgual(PageInfo Page, string Campo, string Termo);

        /// <summary>
        /// Retorna lista de pedidos por filtro LIKE
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        public Task<List<Pedidos>> GetPedidosFiltroLike(PageInfo Page, string Campo, string Termo);

        /// <summary>
        /// Retorna lista de pedidos por período informado
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        public Task<List<Pedidos>> GetPedidosPorPerido(PageInfo Page, string Campo, DateTime De, DateTime Ate);
    }
}
