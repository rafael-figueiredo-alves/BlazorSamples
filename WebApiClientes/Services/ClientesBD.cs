using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using BlazorClientes.Shared.Entities;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Classe fornecedora das operações com a Base de Dados
    /// </summary>
    public class ClientesBD : IClientes
    {
        private readonly string? Conn;
        
        /// <summary>
        /// Classe construtora
        /// </summary>
        public ClientesBD() 
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Conn = configuration.GetConnectionString("Default");
        }
        
        /// <summary>
        /// Método para apagar clientes
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Nenhum conteúdo</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeleteCliente(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "delete from clientes where idCliente = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var ResultSet = await cmd.ExecuteNonQueryAsync();
                conn.Close();

                return (!ResultSet.Equals(0));
            }
            catch
            {
                return false;
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Pega Cliente pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Cliente</returns>
        public async Task<Clientes> GetCliente(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                Clientes clientes;

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from clientes where idCliente = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    clientes = new Clientes();
                }
                else
                {
                    await reader.ReadAsync();
                    clientes = new Clientes(
                                            reader["Nome"].ToString()!,
                                            reader["Endereco"].ToString()!,
                                            reader["Telefone"].ToString()!,
                                            reader["Celular"].ToString()!,
                                            reader["Email"].ToString()!,
                                            reader["idCliente"].ToString());
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

        /// <summary>
        /// Retorna lista de clientes
        /// </summary>
        /// <returns>Lista de clientes</returns>
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
                    clientes.Add(new Clientes(
                                              reader["Nome"].ToString()!,
                                              reader["Endereco"].ToString()!,
                                              reader["Telefone"].ToString()!,
                                              reader["Celular"].ToString()!,
                                              reader["Email"].ToString()!,
                                              reader["idCliente"].ToString()));
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

        /// <summary>
        /// Inclui cliente
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Clientes> PostCliente(Clientes cliente)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "insert into clientes (IdCliente, Nome, Endereco, Telefone, Celular, Email) values (@id, @nome, @endereco, @telefone, @celular, @email)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", cliente.idCliente));
                cmd.Parameters.Add(new MySqlParameter("nome", cliente.Nome));
                cmd.Parameters.Add(new MySqlParameter("endereco", cliente.Endereco));
                cmd.Parameters.Add(new MySqlParameter("telefone", cliente.Telefone));
                cmd.Parameters.Add(new MySqlParameter("celular", cliente.Celular));
                cmd.Parameters.Add(new MySqlParameter("email", cliente.Email));
                await cmd.ExecuteNonQueryAsync();
                sql = "select * from Clientes where (idCliente = @id)";
                cmd.CommandText = sql;
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        return new Clientes(
                                              reader["Nome"].ToString()!,
                                              reader["Endereco"].ToString()!,
                                              reader["Telefone"].ToString()!,
                                              reader["Celular"].ToString()!,
                                              reader["Email"].ToString()!,
                                              reader["idCliente"].ToString());
                    }
                    else
                    {
                        return null!;
                    }
                }
                else 
                {
                    return null!;
                }
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

        /// <summary>
        /// Altera dados de cliente
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Clientes> PutCliente(Clientes cliente, string ID)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "update clientes set Nome = @nome, Endereco = @endereco, Telefone = @telefone, Celular = @celular, Email = @email where idCliente = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", ID));
                cmd.Parameters.Add(new MySqlParameter("nome", cliente.Nome));
                cmd.Parameters.Add(new MySqlParameter("endereco", cliente.Endereco));
                cmd.Parameters.Add(new MySqlParameter("telefone", cliente.Telefone));
                cmd.Parameters.Add(new MySqlParameter("celular", cliente.Celular));
                cmd.Parameters.Add(new MySqlParameter("email", cliente.Email));
                await cmd.ExecuteNonQueryAsync();
                sql = "select * from Clientes where idCliente = @id";
                cmd.CommandText = sql;
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        return new Clientes(
                                              reader["Nome"].ToString()!,
                                              reader["Endereco"].ToString()!,
                                              reader["Telefone"].ToString()!,
                                              reader["Celular"].ToString()!,
                                              reader["Email"].ToString()!,
                                              reader["idClientes"].ToString());
                    }
                    else
                    {
                        return null!;
                    }
                }
                else
                {
                    return null!;
                }
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
    }
}
