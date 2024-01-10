using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Enums;

namespace WebApiClientes.Services.Interfaces
{
    /// <summary>
    /// interface de serviços da classe clientes
    /// </summary>
    public interface IClientes
    {
        /// <summary>
        /// Método ´para pegar lista de clientes
        /// </summary>
        /// <param name="Page">Informações para paginação</param>
        /// <returns>Lista de clientes</returns>
        public Task<List<Clientes>> GetClientes(PageInfo Page);

        /// <summary>
        /// Paga Cliente por ID
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="Kind"></param>
        /// <returns>Retorna cliente</returns>
        public Task<Clientes?> GetCliente(string id, GetKind Kind = GetKind.PorCodigo);

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
        /// <param name="Page">Informações para paginação</param>
        /// <returns>Lista de Clientes</returns>
        public Task<List<Clientes>> GetClientesPorFiltro(PageInfo Page, FiltrosCliente FiltrarPor, string? TermoBusca);

        /// <summary>
        /// Retorna lista de clientes para impressão
        /// </summary>
        /// <returns>Retorna lista de clientes para impressão</returns>
        public Task<List<Clientes>> GetClientesToPrint();
    }
}
