using WebApiClientes.Entities;

namespace WebApiClientes.Services
{
    public interface IClientes
    {
        public Task<List<Clientes>> GetClientes();
        public Task<Clientes> GetCliente(int id);
        public Task<Clientes> PostCliente(ClienteDTO cliente);
        public Task<Clientes> PutCliente(Clientes cliente);
        public bool DeleteCliente(int id);
    }
}
