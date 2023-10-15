using BlazorClientes.Shared.Entities;
using MySql.Data.MySqlClient;
using WebApiClientes.Services.Interfaces;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Classe concreta da interface IProdutos
    /// </summary>
    public class ProdutosBD : IProdutos
    {
        private readonly string? Conn;

        /// <summary>
        /// Classe construtora
        /// </summary>
        public ProdutosBD()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Conn = configuration.GetConnectionString("Default");
        }

        /// <summary>
        /// Método para apagar Produto
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Nenhum conteúdo</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeleteProduto(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "delete from produtos where idProduto = @id";
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
        /// Pega Produto pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Produto</returns>
        public async Task<Produtos?> GetProduto(string id)
        {
            MySqlConnection? conn = null;
            try
            {
                Produtos? produto;

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from produtos where idProduto = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    produto = null;
                }
                else
                {
                    await reader.ReadAsync();
                    produto = new Produtos(
                                            reader["Produto"].ToString()!,
                                            reader["Descricao"].ToString()!,
                                            Convert.ToDecimal(reader["Valor"].ToString()!),
                                            reader["Barcode"].ToString()!,
                                            reader["idProduto"].ToString());
                }

                return produto;
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
        /// Retorna lista de produtos
        /// </summary>
        /// <returns>Lista de produtos</returns>
        public async Task<List<Produtos>> GetProdutos(PageInfo Page)
        {
            var produtos = new List<Produtos>();
            var PageNumber = Page.Page ?? 1;
            var PageSize = Page.PageSize ?? 10;
            int TotalPages;
            int TotalRecords;

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql_counter = "select Count(*) AS Total from produtos";
                var cmd_counter = new MySqlCommand(sql_counter, conn);
                TotalRecords = Convert.ToInt32(await cmd_counter.ExecuteScalarAsync());
 
                int inicio = (PageNumber - 1) * PageSize;
                TotalPages = Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));

                Page.Page = PageNumber;
                Page.PageSize = PageSize;
                Page.TotalPages = TotalPages;
                Page.TotalRecords = TotalRecords;

                string sql = "select * from produtos limit @inicio, @qtd";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("inicio", inicio));
                cmd.Parameters.Add(new MySqlParameter("qtd", PageSize));
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    produtos.Add(new Produtos(
                                              reader["Produto"].ToString()!,
                                              reader["Descricao"].ToString()!,
                                              Convert.ToDecimal(reader["Valor"].ToString()!),
                                              reader["Barcode"].ToString()!,
                                              reader["idProduto"].ToString()));
                }

                return produtos;

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
        /// Inclui produto
        /// </summary>
        /// <param name="produto">produto</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Produtos> PostProduto(Produtos produto)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "insert into produtos (IdProduto, Produto, Descricao, Valor, Barcode) values (@id, @produto, @descricao, @valor, @barcode)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", produto.idProduto));
                cmd.Parameters.Add(new MySqlParameter("produto", produto.Produto));
                cmd.Parameters.Add(new MySqlParameter("descricao", produto.Descricao));
                cmd.Parameters.Add(new MySqlParameter("valor", produto.Valor));
                cmd.Parameters.Add(new MySqlParameter("barcode", produto.Barcode));
                await cmd.ExecuteNonQueryAsync();
                sql = "select * from produtos where (idProduto = @id)";
                cmd.CommandText = sql;
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        return new Produtos(
                                              reader["Produto"].ToString()!,
                                              reader["Descricao"].ToString()!,
                                              Convert.ToDecimal(reader["Valor"].ToString()!),
                                              reader["Barcode"].ToString()!,
                                              reader["idProduto"].ToString());
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
        /// Altera dados de produto
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Produtos> PutProduto(Produtos produto, string ID)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "update produtos set Produto = @produto, Descricao = @descricao, Valor = @valor, Barcode = @barcode where idProduto = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", ID));
                cmd.Parameters.Add(new MySqlParameter("produto", produto.Produto));
                cmd.Parameters.Add(new MySqlParameter("descricao", produto.Descricao));
                cmd.Parameters.Add(new MySqlParameter("valor", produto.Valor));
                cmd.Parameters.Add(new MySqlParameter("barcode", produto.Barcode));
                await cmd.ExecuteNonQueryAsync();
                sql = "select * from produtos where idProduto = @id";
                cmd.CommandText = sql;
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        return new Produtos(
                                              reader["Produto"].ToString()!,
                                              reader["Descricao"].ToString()!,
                                              Convert.ToDecimal(reader["Valor"].ToString()!),
                                              reader["Barcode"].ToString()!,
                                              reader["idProduto"].ToString());
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
        /// Retorna lista de produtos com nome ou descrição informado
        /// </summary>
        /// <returns>Lista de produtos</returns>
        public async Task<List<Produtos>> GetProdutosPorFiltro(PageInfo Page, FiltroProdutos FiltrarPor, string? TermoBusca)
        {
            var produtos = new List<Produtos>();
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
                    case FiltroProdutos.PorProduto:
                        sql = "select * from produtos where produto like '%" + TermoBusca + "%'";
                        sql_counter = "select Count(*) AS Total from produtos where produto like '%" + TermoBusca + "%'";
                        break;
                    case FiltroProdutos.PorDescricao:
                        sql = "select * from produtos where descricao like '%" + TermoBusca + "%'";
                        sql_counter = "select Count(*) AS Total from produtos where descricao like '%" + TermoBusca + "%'";
                        break;
                    case FiltroProdutos.PorBarcode:
                        sql = "select * from produtos where barcode like '%" + TermoBusca + "%'";
                        sql_counter = "select Count(*) AS Total from produtos where barcode like '%" + TermoBusca + "%'";
                        break;
                    default:
                        sql = "select * from produtos";
                        sql_counter = "select Count(*) from produtos";
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
                    produtos.Add(new Produtos(
                                              reader["Produto"].ToString()!,
                                              reader["Descricao"].ToString()!,
                                              Convert.ToDecimal(reader["Valor"].ToString()!),
                                              reader["Barcode"].ToString()!,
                                              reader["idProduto"].ToString()));
                }

                return produtos;

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
