using BlazorClientes.Shared.Entities;

namespace WebApiClientes.Services
{
    /// <summary>
    /// interface de serviços da classe clientes
    /// </summary>
    public interface IClientes
    {
        /// <summary>
        /// Método ´para pegar lista de clientes
        /// </summary>
        /// <returns>Lista de clientes</returns>
        public Task<List<Clientes>> GetClientes();
       
        /// <summary>
        /// Paga Cliente por ID
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Retorna cliente</returns>
        public Task<Clientes> GetCliente(string id);
       
        /// <summary>
        /// Insere novo cliente
        /// </summary>
        /// <param name="cliente">Cliente</param>
        /// <returns>Retorna cliente criado</returns>
        public Task<Clientes> PostCliente(Clientes cliente);

        /// <summary>
        /// Altera dados de um cliente
        /// </summary>
        /// <param name="cliente">Cliente</param>
        /// <param name="ID">Id do Cliente</param> 
        /// <returns>Retorna o cliente alterado</returns>
        public Task<Clientes> PutCliente(Clientes cliente, string ID);
     
        /// <summary>
        /// Apaga cliente
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Verdadeiro ou falso</returns>
        public Task<bool> DeleteCliente(string id);

        /// <summary>
        /// Buscar por clientes usando um filtro de busca
        /// </summary>
        /// <param name="FiltrarPor">Campo a filtrar</param>
        /// <param name="TermoBusca">Termo a buscar</param>
        /// <returns>Lista de Clientes</returns>
        public Task<List<Clientes>> GetClientesPorFiltro(FiltrosCliente FiltrarPor, string? TermoBusca);
    }
}
