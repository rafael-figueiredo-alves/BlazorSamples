using BlazorClientes.Shared.Entities;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using WebApiClientes.Services.Interfaces;

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
        /// <param name="id">ID do Cliente a apagar</param>
        /// <returns>Nenhum conteúdo</returns>
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

                return (!ResultSet.Equals(0));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
        public async Task<Clientes?> GetCliente(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                Clientes? clientes;

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from clientes where Codigo = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    clientes = null;
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
                                            Convert.ToUInt32(reader["Codigo"]),
                                            reader["idCliente"].ToString()) ;
                }

                return clientes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Retorna lista de clientes
        /// </summary>
        /// <param name="Page">Informações para paginação</param>
        /// <returns>Lista de clientes</returns>
        public async Task<List<Clientes>> GetClientes(PageInfo Page)
        {
            var clientes = new List<Clientes>();
            var PageNumber = Page.Page ?? 1;
            var PageSize = Page.PageSize ?? 10;
            int TotalPages;
            int TotalRecords;

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select Count(*) AS Total from clientes";
                var cmd_counter = new MySqlCommand(sql, conn);
                TotalRecords = Convert.ToInt32(await cmd_counter.ExecuteScalarAsync())!;
                
                int inicio = (PageNumber - 1) * PageSize;
                TotalPages = Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));

                Page.Page = PageNumber;
                Page.PageSize = PageSize;
                Page.TotalPages = TotalPages;
                Page.TotalRecords = TotalRecords;

                sql = "select * from clientes limit @inicio, @qtd";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("inicio", inicio));
                cmd.Parameters.Add(new MySqlParameter("qtd", PageSize));
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    clientes.Add(new Clientes(
                                              reader["Nome"].ToString()!,
                                              reader["Endereco"].ToString()!,
                                              reader["Telefone"].ToString()!,
                                              reader["Celular"].ToString()!,
                                              reader["Email"].ToString()!,
                                              Convert.ToUInt32(reader["Codigo"]),
                                              reader["idCliente"].ToString()));
                }

                return clientes;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                string sql = "insert into clientes (IdCliente, Nome, Endereco, Telefone, Celular, Email, Codigo) values (@id, @nome, @endereco, @telefone, @celular, @email, @codigo)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", cliente.idCliente));
                cmd.Parameters.Add(new MySqlParameter("nome", cliente.Nome));
                cmd.Parameters.Add(new MySqlParameter("endereco", cliente.Endereco));
                cmd.Parameters.Add(new MySqlParameter("telefone", cliente.Telefone));
                cmd.Parameters.Add(new MySqlParameter("celular", cliente.Celular));
                cmd.Parameters.Add(new MySqlParameter("email", cliente.Email));
                cmd.Parameters.Add(new MySqlParameter("codigo", await GetCodigo()));
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
                                              Convert.ToUInt32(reader["Codigo"]),
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
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
            }
        }

        private async Task<uint?> GetCodigo()
        {
            uint codigo = 0;

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();

                var sql = "select Max(Codigo) AS Cod from clientes";
                var cmd = new MySqlCommand(sql, conn);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    codigo = Convert.ToUInt32(reader["Cod"]);
                }

                codigo += 1;

                return codigo;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                                              Convert.ToUInt32(reader["Codigo"]),
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Buscar por clientes usando um filtro de busca
        /// </summary>
        /// <param name="FiltrarPor">Campo a filtrar</param>
        /// <param name="TermoBusca">Termo a buscar</param>
        /// <param name="Page">Informações para paginação</param>
        /// <returns>Lista de Clientes</returns>
        public async Task<List<Clientes>> GetClientesPorFiltro(PageInfo Page, FiltrosCliente FiltrarPor, string? TermoBusca)
        {
            var clientes = new List<Clientes>();

            if(string.IsNullOrEmpty(TermoBusca))
            {
                TermoBusca = "%";
            }

            MySqlConnection? conn = null;
            try
            {
                var PageNumber = Page.Page ?? 1;
                var PageSize = Page.PageSize ?? 10;
                int TotalPages;
                int TotalRecords;

                conn = new MySqlConnection(Conn);
                conn.Open();
                
                string sql;
                string sql_counter;

                switch(FiltrarPor)
                {
                    case FiltrosCliente.PorNome:
                        sql = "select * from clientes where Nome like '%" + TermoBusca + "%'";
                        sql_counter = "select Count(*) AS Total from clientes where Nome like '%" + TermoBusca + "%'";
                        break;
                    case FiltrosCliente.PorEndereco:
                        sql = "select * from clientes where Endereco like '%" + TermoBusca + "%'";
                        sql_counter = "select Count(*) AS Total from clientes where Endereco like '%" + TermoBusca + "%'";
                        break;
                    case FiltrosCliente.PorCodigo:
                        sql = "select * from clientes where Codigo like '%" + TermoBusca + "%'";
                        sql_counter = "select Count(*) AS Total from clientes where Codigo like '%" + TermoBusca + "%'";
                        break;
                    default:
                        sql = "select * from clientes";
                        sql_counter = "select Count(*) from clientes";
                        break;
                }

                var cmd_counter = new MySqlCommand(sql_counter, conn);
                TotalRecords = Convert.ToInt32(await cmd_counter.ExecuteScalarAsync());

                int inicio = (PageNumber - 1) * PageSize;
                TotalPages = Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));

                Page.Page = PageNumber;
                Page.PageSize = PageSize;
                Page.TotalPages = TotalPages;
                Page.TotalRecords = TotalRecords;

                sql += " limit @inicio, @qtd";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("inicio", inicio));
                cmd.Parameters.Add(new MySqlParameter("qtd", PageSize));
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    clientes.Add(new Clientes(
                                              reader["Nome"].ToString()!,
                                              reader["Endereco"].ToString()!,
                                              reader["Telefone"].ToString()!,
                                              reader["Celular"].ToString()!,
                                              reader["Email"].ToString()!,
                                              Convert.ToUInt32(reader["Codigo"]),
                                              reader["idCliente"].ToString()));
                }

                return clientes;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Retorna lista de clientes
        /// </summary>
        /// <param name="Page">Informações para paginação</param>
        /// <returns>Lista de clientes</returns>
        public async Task<List<Clientes>> GetClientesToPrint()
        {
            var clientes = new List<Clientes>();

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();

                var sql = "select * from clientes";
                var cmd = new MySqlCommand(sql, conn);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    clientes.Add(new Clientes(
                                              reader["Nome"].ToString()!,
                                              reader["Endereco"].ToString()!,
                                              reader["Telefone"].ToString()!,
                                              reader["Celular"].ToString()!,
                                              reader["Email"].ToString()!,
                                              Convert.ToUInt32(reader["Codigo"]),
                                              reader["idCliente"].ToString()));
                }

                return clientes;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
            }
        }
    }
}
