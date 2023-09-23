using WebApiClientes.Entities;

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
        /// <returns>Lista de Pedidos</returns>
        public Task<List<Pedidos>> GetPedidos();

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
        public Task<bool> DeleteVendedor(string id);
    }
}
