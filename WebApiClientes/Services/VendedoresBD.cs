using MySql.Data.MySqlClient;
using BlazorClientes.Shared.Entities;
using WebApiClientes.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        public async Task<Vendedores?> GetVendedor(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                Vendedores? vendedor;

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from vendedores where idVendedor = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    vendedor = null;
                }
                else
                {
                    await reader.ReadAsync();
                    vendedor = new Vendedores(
                                            reader["Vendedor"].ToString()!,
                                            Convert.ToInt32(reader["pComissao"].ToString()!),
                                            Convert.ToUInt32(reader["Codigo"]),
                                            reader["idVendedor"].ToString());
                }

                return vendedor;
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
        /// Retorna lista de vendedores
        /// </summary>
        /// <returns>Lista de vendedores</returns>
        public async Task<List<Vendedores>> GetVendedores(PageInfo Page)
        {
            var vendedores = new List<Vendedores>();
            var PageNumber = Page.Page ?? 1;
            var PageSize = Page.PageSize ?? 10;
            int TotalPages;
            int TotalRecords;

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql_counter = "select Count(*) AS Total from vendedores";
                var cmd_counter = new MySqlCommand(sql_counter, conn);
                TotalRecords = Convert.ToInt32(await cmd_counter.ExecuteScalarAsync());

                int inicio = (PageNumber - 1) * PageSize;
                TotalPages = Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));

                Page.Page = PageNumber;
                Page.PageSize = PageSize;
                Page.TotalPages = TotalPages;
                Page.TotalRecords = TotalRecords;

                string sql = "select * from vendedores limit @inicio, @qtd";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("inicio", inicio));
                cmd.Parameters.Add(new MySqlParameter("qtd", PageSize));
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    vendedores.Add(new Vendedores(
                                              reader["Vendedor"].ToString()!,
                                              Convert.ToInt32(reader["pComissao"].ToString()!),
                                              Convert.ToUInt32(reader["Codigo"]),
                                              reader["idVendedor"].ToString()));
                }

                return vendedores;

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
                                              Convert.ToUInt32(reader["Codigo"]),
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
                                              Convert.ToUInt32(reader["Codigo"]),
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
        /// Método para pegar lista de Vendedores filtrados
        /// </summary>
        /// <param name="FiltrarPor"></param>
        /// <param name="TermoBusca"></param>
        /// <param name="Page">Informações sobre Página</param>
        /// <returns></returns>
        public async Task<List<Vendedores>> GetVendedoresPorFiltro(PageInfo Page, FiltroVendedor FiltrarPor, string? TermoBusca)
        {
            var vendedores = new List<Vendedores>();
            var PageNumber = Page.Page ?? 1;
            var PageSize = Page.PageSize ?? 10;
            int TotalPages;
            int TotalRecords;

            if (string.IsNullOrEmpty(TermoBusca))
            {
                TermoBusca = "%";
            }

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();

                string sql;
                string sql_counter;

                switch (FiltrarPor)
                {
                    case FiltroVendedor.PorNome:
                        sql = "select * from vendedores where Vendedor like '%" + TermoBusca + "%'";
                        sql_counter = "select Count(*) AS Total from vendedores where Vendedor like '%" + TermoBusca + "%'";
                        break;
                    case FiltroVendedor.PorCodigo:
                        sql = "select * from vendedores where Codigo like '%" + TermoBusca + "%'";
                        sql_counter = "select Count(*) AS Total from vendedores where Codigo like '%" + TermoBusca + "%'";
                        break;
                    default:
                        sql = "select * from vendedores";
                        sql_counter = "select Count(*) AS Total from vendedores";
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
                    vendedores.Add(new Vendedores(
                                              reader["Vendedor"].ToString()!,
                                              Convert.ToInt32(reader["pComissao"].ToString()!),
                                              Convert.ToUInt32(reader["Codigo"]),
                                              reader["idVendedor"].ToString()));
                }

                return vendedores;

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
        /// Retorna lista completa de vendedores para imprimir
        /// </summary>
        /// <returns>Retorna lista completa de vendedores para imprimir</returns>
        public async Task<List<Vendedores>> GetVendedoresToPrint()
        {
            var vendedores = new List<Vendedores>();

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();

                string sql = "select * from vendedores ";
                var cmd = new MySqlCommand(sql, conn);
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    vendedores.Add(new Vendedores(
                                              reader["Vendedor"].ToString()!,
                                              Convert.ToInt32(reader["pComissao"].ToString()!),
                                              Convert.ToUInt32(reader["Codigo"]),
                                              reader["idVendedor"].ToString()));
                }

                return vendedores;

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
