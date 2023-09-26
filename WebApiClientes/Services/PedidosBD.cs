using BlazorClientes.Shared.Entities;
using MySql.Data.MySqlClient;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Classe fornecedora das operações com a Base de Dados
    /// </summary>
    public class PedidosBD : IPedidos
    {
        private readonly string? Conn;

        /// <summary>
        /// Classe construtora
        /// </summary>
        public PedidosBD()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Conn = configuration.GetConnectionString("Default");
        }

        /// <summary>
        /// Método para apagar pedido
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Nenhum conteúdo</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeletePedido(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "delete from itenspedido where idPedido = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var ResultSet_ItensPedido = await cmd.ExecuteNonQueryAsync();
                sql = "delete from pedidos where idPedido = @id";
                cmd.CommandText = sql;
                var ResultSet_Pedidos = await cmd.ExecuteNonQueryAsync();
                conn.Close();

                return ((!ResultSet_ItensPedido.Equals(0)) && (!ResultSet_Pedidos.Equals(0)));
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
        /// Pega Pedido pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Pedido</returns>
        public async Task<Pedidos> GetPedido(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                Pedidos pedido;

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select pedidos.*, clientes.Nome AS Cliente, vendedores.Vendedor, vendedores.pComissao from pedidos Inner Join clientes ON (clientes.idCliente = pedidos.idCliente) INNER JOIN vendedores on (vendedores.idVendedor = pedidos.idVendedor)  where idPedido = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    pedido = new Pedidos();
                }
                else
                {
                    await reader.ReadAsync();
                    pedido = new Pedidos(
                                            reader["idCliente"].ToString()!,
                                            reader["idVendedor"].ToString()!,
                                            Convert.ToDecimal(reader["vComissao"].ToString()!),
                                            Convert.ToInt32(reader["pComissao"].ToString()!),
                                            Convert.ToDecimal(reader["ValorTotal"].ToString()!),
                                            Convert.ToDateTime(reader["DataEmissao"].ToString()!),
                                            Convert.ToDateTime(reader["DataEntrega"].ToString()!),
                                            reader["status"].ToString()!,
                                            reader["idPedido"].ToString()!);
                    string sql_itens = "select itenspedido.*, produtos.Descricao from itenspedido where idPedido = @id";
                    var cmd_itens = new MySqlCommand(sql_itens, conn);
                    var reader_itens = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        pedido.Itens.Add(new ItensPedido(
                                                        Convert.ToInt32(reader_itens["Indice"].ToString()!),
                                                        reader_itens["idPedido"].ToString()!,
                                                        reader_itens["idProduto"].ToString()!,
                                                        reader_itens["Descricao"].ToString()!,
                                                        Convert.ToInt32(reader_itens["Quantidade"].ToString()!),
                                                        Convert.ToDecimal(reader_itens["ValorUnitario"].ToString()!),
                                                        Convert.ToInt32(reader_itens["pDesconto"].ToString()!),
                                                        Convert.ToDecimal(reader_itens["Valor"].ToString()!)
                            ));
                    }
                }
                conn.Close();

                return pedido;
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
        /// Retorna lista de pedidos
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        public async Task<List<Pedidos>> GetPedidos()
        {
            var pedidos = new List<Pedidos>();

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from clientes";
                var cmd = new MySqlCommand(sql, conn);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    pedidos.Add(new Pedidos(
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
        /// Inclui pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Pedidos> PostPedido(Pedidos pedido)
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
        /// <param name="pedido"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Pedidos> PutPedido(Pedidos pedido, string ID)
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
