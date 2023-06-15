using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebApiClientes.Entities;

namespace WebApiClientes.Services
{
    public class ClientesBD : IClientes
    {
        private readonly string? Conn;
        public ClientesBD() 
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Conn = configuration.GetConnectionString("Default");
        }
        public bool DeleteCliente(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Clientes> GetCliente(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Clientes>> GetClientes()
        {
            var clientes = new List<Clientes>();

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from clientes";
                var cmd = new MySqlCommand(sql, conn);
                var reader = await cmd.ExecuteReaderAsync();
                while(await reader.ReadAsync())
                {
                    clientes.Add(new Clientes(Convert.ToInt32(reader["idClientes"].ToString()),
                                              reader["Nome"].ToString()!,
                                              reader["Endereco"].ToString()!,
                                              reader["Telefone"].ToString()!,
                                              reader["Celular"].ToString()!,
                                              reader["Email"].ToString()!));
                }

                conn.Close();
                return clientes;

            }
            catch
            {
                return null!; 
            }
            finally 
            { 
                conn?.Close(); 
            }
        }

        public Task<Clientes> PostCliente(Clientes cliente)
        {
            throw new NotImplementedException();
        }

        public Task<Clientes> PutCliente(Clientes cliente)
        {
            throw new NotImplementedException();
        }
    }
}
