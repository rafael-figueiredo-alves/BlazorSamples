using WebApiClientes.Entities;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Interface dos serviços relacionados a entidade vendedores
    /// </summary>
    public interface IVendedores
    {
        /// <summary>
        /// Método ´para pegar lista de Vendedores
        /// </summary>
        /// <returns>Lista de Vendedores</returns>
        public Task<List<Vendedores>> GetVendedores();

        /// <summary>
        /// Paga Vendedor por ID
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Retorna Vendedor</returns>
        public Task<Vendedores> GetVendedor(string id);

        /// <summary>
        /// Insere novo Vendedor
        /// </summary>
        /// <param name="Vendedor">Vendedor</param>
        /// <returns>Retorna Vendedor criado</returns>
        public Task<Vendedores> PostVendedor(Vendedores Vendedor);

        /// <summary>
        /// Altera dados de um Vendedor
        /// </summary>
        /// <param name="Vendedor">Vendedor</param>
        /// <param name="ID">Id do Vendedor</param> 
        /// <returns>Retorna o Vendedor alterado</returns>
        public Task<Vendedores> PutVendedor(Vendedores Vendedor, string ID);

        /// <summary>
        /// Apaga vendedor
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Verdadeiro ou falso</returns>
        public Task<bool> DeleteVendedor(string id);
    }
}
