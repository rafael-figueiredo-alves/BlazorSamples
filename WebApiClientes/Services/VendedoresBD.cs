using MySql.Data.MySqlClient;
using BlazorClientes.Shared.Entities;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Implementação da interface IVendedodres
    /// </summary>
    public class VendedoresBD : IVendedores
    {
        private readonly string? Conn;

        /// <summary>
        /// Classe construtora
        /// </summary>
        public VendedoresBD()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Conn = configuration.GetConnectionString("Default");
        }

        /// <summary>
        /// Método para apagar Vendedores
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Nenhum conteúdo</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeleteVendedor(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "delete from vendedores where idVendedor = @id";
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
        public async Task<Vendedores> GetVendedor(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                Vendedores vendedor;

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from vendedores where idVendedor = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    vendedor = new Vendedores();
                }
                else
                {
                    await reader.ReadAsync();
                    vendedor = new Vendedores(
                                            reader["Vendedor"].ToString()!,
                                            Convert.ToInt32(reader["pComissao"].ToString()!),
                                            reader["idVendedor"].ToString());
                }
                conn.Close();

                return vendedor;
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
        /// Retorna lista de vendedores
        /// </summary>
        /// <returns>Lista de vendedores</returns>
        public async Task<List<Vendedores>> GetVendedores()
        {
            var vendedores = new List<Vendedores>();

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from vendedores";
                var cmd = new MySqlCommand(sql, conn);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    vendedores.Add(new Vendedores(
                                              reader["Vendedor"].ToString()!,
                                              Convert.ToInt32(reader["pComissao"].ToString()!),
                                              reader["idVendedor"].ToString()));
                }

                conn.Close();
                return vendedores;

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
        /// Inclui vendedor
        /// </summary>
        /// <param name="vendedor">vendedor</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Vendedores> PostVendedor(Vendedores vendedor)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "insert into vendedores (IdVendedor, Vendedor, pComissao) values (@id, @vendedor, @pcomissao)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", vendedor.idVendedor));
                cmd.Parameters.Add(new MySqlParameter("vendedor", vendedor.Vendedor));
                cmd.Parameters.Add(new MySqlParameter("pcomissao", vendedor.pComissao));
                await cmd.ExecuteNonQueryAsync();
                sql = "select * from vendedores where (idVendedor = @id)";
                cmd.CommandText = sql;
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        return new Vendedores(
                                              reader["Vendedor"].ToString()!,
                                              Convert.ToInt32(reader["pComissao"].ToString()!),
                                              reader["idVendedor"].ToString());
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
        /// Altera dados de vendedor
        /// </summary>
        /// <param name="vendedor"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Vendedores> PutVendedor(Vendedores vendedor, string ID)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "update vendedores set Vendedor = @vendedor, pComissao = @pcomissao where idVendedor = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", ID));
                cmd.Parameters.Add(new MySqlParameter("vendedor", vendedor.Vendedor));
                cmd.Parameters.Add(new MySqlParameter("pcomissao", vendedor.pComissao));
                await cmd.ExecuteNonQueryAsync();
                sql = "select * from vendedores where idVendedor = @id";
                cmd.CommandText = sql;
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        return new Vendedores(
                                              reader["Vendedor"].ToString()!,
                                              Convert.ToInt32(reader["pComissao"].ToString()!),
                                              reader["idVendedor"].ToString());
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
