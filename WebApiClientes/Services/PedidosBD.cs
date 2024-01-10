using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Utils;
using MySql.Data.MySqlClient;
using System.Text.Json;
using WebApiClientes.Services.Interfaces;

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

                return ((!ResultSet_ItensPedido.Equals(0)) && (!ResultSet_Pedidos.Equals(0)));
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
        /// Pega Pedido pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Pedido</returns>
        public async Task<Pedidos?> GetPedido(string id)
        {
            MySqlConnection? conn = null;
            MySqlConnection? conn2 = null;
            try
            {
                Pedidos? pedido;

                conn = new MySqlConnection(Conn);
                conn.Open();
                conn2 = new MySqlConnection(Conn);
                conn2.Open();
                string sql = "select pedidos.*, clientes.Nome AS Cliente, vendedores.Vendedor, vendedores.pComissao from pedidos Inner Join clientes ON (clientes.idCliente = pedidos.idCliente) INNER JOIN vendedores on (vendedores.idVendedor = pedidos.idVendedor)  where idPedido = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    pedido = null;
                }
                else
                {
                    await reader.ReadAsync();
                    pedido = new(
                                            reader["idCliente"].ToString()!,
                                            reader["idVendedor"].ToString()!,
                                            Convert.ToDecimal(reader["vComissao"].ToString()!),
                                            Convert.ToInt32(reader["pComissao"].ToString()!),
                                            Convert.ToDecimal(reader["ValorTotal"].ToString()!),
                                            Convert.ToDateTime(reader["DataEmissao"].ToString()!),
                                            Convert.ToDateTime(reader["DataEntrega"].ToString()!),
                                            reader["status"].ToString()!,
                                            reader["idPedido"].ToString()!)
                    {
                        Cliente = reader["Cliente"].ToString()!,
                        Vendedor = reader["Vendedor"].ToString()!
                    };
                    string sql_itens = "select itenspedido.*, produtos.Descricao from itenspedido inner join Produtos on (produtos.idProduto = itenspedido.idProduto) where idPedido = @id";
                    var cmd_itens = new MySqlCommand(sql_itens, conn2);
                    cmd_itens.Parameters.Add(new MySqlParameter("id", id));
                    var reader_itens = await cmd_itens.ExecuteReaderAsync();
                    while (await reader_itens.ReadAsync())
                    {
                        pedido.Itens!.Add(new ItensPedido(
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
                
                if (pedido != null)
                    pedido.ETag = HashMD5.Hash(JsonSerializer.Serialize(pedido));

                return pedido;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
                conn2?.Close();
            }
        }

        /// <summary>
        /// Retorna lista de pedidos
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        public async Task<List<Pedidos>> GetPedidos(PageInfo Page)
        {
            var pedidos = new List<Pedidos>();
            var PageNumber = Page.Page ?? 1;
            var PageSize = Page.PageSize ?? 10;
            int TotalPages;
            int TotalRecords;

            MySqlConnection? conn = null;
            MySqlConnection? conn2 = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                conn2 = new MySqlConnection(Conn);
                conn2.Open();
                string sql_counter = "select Count(*) AS Total from pedidos";
                var cmd_counter = new MySqlCommand(sql_counter, conn);
                TotalRecords = Convert.ToInt32(await cmd_counter.ExecuteScalarAsync());

                int inicio = (PageNumber - 1) * PageSize;
                TotalPages = Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));

                Page.Page = PageNumber;
                Page.PageSize = PageSize;
                Page.TotalPages = TotalPages;
                Page.TotalRecords = TotalRecords;

                string sql = "select pedidos.*, clientes.Nome AS Cliente, vendedores.Vendedor, vendedores.pComissao from pedidos Inner Join clientes ON (clientes.idCliente = pedidos.idCliente) INNER JOIN vendedores on (vendedores.idVendedor = pedidos.idVendedor) LIMIT @inicio, @qtd";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("inicio", inicio));
                cmd.Parameters.Add(new MySqlParameter("qtd", PageSize));
                var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    Pedidos pedido = new(
                                            reader["idCliente"].ToString()!,
                                            reader["idVendedor"].ToString()!,
                                            Convert.ToDecimal(reader["vComissao"].ToString()!),
                                            Convert.ToInt32(reader["pComissao"].ToString()!),
                                            Convert.ToDecimal(reader["ValorTotal"].ToString()!),
                                            Convert.ToDateTime(reader["DataEmissao"].ToString()!),
                                            Convert.ToDateTime(reader["DataEntrega"].ToString()!),
                                            reader["status"].ToString()!,
                                            reader["idPedido"].ToString()!)
                    {
                        Cliente = reader["Cliente"].ToString()!,
                        Vendedor = reader["Vendedor"].ToString()!
                    };
                    string sql_itens = "select itenspedido.*, produtos.Descricao from itenspedido inner join produtos ON (Produtos.idProduto = Itenspedido.IdProduto) where idPedido = @id";
                    var cmd_itens = new MySqlCommand(sql_itens, conn2);
                    cmd_itens.Parameters.Add(new MySqlParameter("id", pedido.idPedido));
                    var reader_itens = await cmd_itens.ExecuteReaderAsync();
                    while (await reader_itens.ReadAsync())
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

                    if (pedido != null)
                        pedido.ETag = HashMD5.Hash(JsonSerializer.Serialize(pedido));

                    pedidos.Add(pedido!);
                    conn2?.Close();
                    conn2!.Open();
                }

                return pedidos;

            }
            catch (IndexOutOfRangeException)
            {
                return pedidos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
                conn2?.Close();
            }
        }

        /// <summary>
        /// Inclui pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Pedidos?> PostPedido(Pedidos pedido)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                
                string sql = "insert into pedidos (IdPedido, IdCliente, DataEmissao, DataEntrega, IdVendedor, vComissao, ValorTotal, Status) values (@idPedido, @idCliente, @dataEmissao, @dataEntrega, @idVendedor, @vcomissao, @valorTotal, @status)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("idPedido", pedido.idPedido));
                cmd.Parameters.Add(new MySqlParameter("idCliente", pedido.idCliente));
                cmd.Parameters.Add(new MySqlParameter("dataEmissao", pedido.DataEmissao.Date));
                cmd.Parameters.Add(new MySqlParameter("dataEntrega", pedido.DataEntrega.Date));
                cmd.Parameters.Add(new MySqlParameter("idVendedor", pedido.idVendedor));
                cmd.Parameters.Add(new MySqlParameter("vcomissao", pedido.vComissao));
                cmd.Parameters.Add(new MySqlParameter("valorTotal", pedido.ValorTotal));
                cmd.Parameters.Add(new MySqlParameter("status", pedido.Status));
                await cmd.ExecuteNonQueryAsync();

                foreach(var item in pedido.Itens)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "insert into itenspedido (IdPedido, IdProduto, Quantidade, ValorUnitario, pDesconto, Valor) values (@idPedido, @idProduto, @quantidade, @valorUnitario, @pdesconto, @valor)";
                    cmd.Parameters.Add(new MySqlParameter("idPedido", pedido.idPedido));
                    cmd.Parameters.Add(new MySqlParameter("idProduto", item.idProduto));
                    cmd.Parameters.Add(new MySqlParameter("quantidade", item.Quantidade));
                    cmd.Parameters.Add(new MySqlParameter("valorUnitario", item.ValorUnitario));
                    cmd.Parameters.Add(new MySqlParameter("pdesconto", item.pDesconto));
                    cmd.Parameters.Add(new MySqlParameter("valor", item.Valor));
                    await cmd.ExecuteNonQueryAsync();
                }
                
                return await GetPedido(pedido.idPedido!);
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
        /// <param name="pedido"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Pedidos?> PutPedido(Pedidos pedido, string ID)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();

                string sql = "Update pedidos set IdCliente = @idCliente, DataEmissao = @dataEmissao, DataEntrega = @dataEntrega, IdVendedor = @idVendedor, vComissao = @vcomissao, ValorTotal = @valorTotal, Status = @status WHERE idPedido = @idPedido";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("idPedido", pedido.idPedido));
                cmd.Parameters.Add(new MySqlParameter("idCliente", pedido.idCliente));
                cmd.Parameters.Add(new MySqlParameter("dataEmissao", pedido.DataEmissao.Date));
                cmd.Parameters.Add(new MySqlParameter("dataEntrega", pedido.DataEntrega.Date));
                cmd.Parameters.Add(new MySqlParameter("idVendedor", pedido.idVendedor));
                cmd.Parameters.Add(new MySqlParameter("vcomissao", pedido.vComissao));
                cmd.Parameters.Add(new MySqlParameter("valorTotal", pedido.ValorTotal));
                cmd.Parameters.Add(new MySqlParameter("status", pedido.Status));
                await cmd.ExecuteNonQueryAsync();

                foreach (var item in pedido.Itens)
                {
                    if (item.Indice > 0)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "update itenspedido set IdProduto = @idProduto, Quantidade = @quantidade, ValorUnitario = @valorUnitario, pDesconto = @pdesconto, Valor = @valor where IdPedido = @idPedido";
                        cmd.Parameters.Add(new MySqlParameter("idPedido", item.idPedido));
                        cmd.Parameters.Add(new MySqlParameter("idProduto", item.idProduto));
                        cmd.Parameters.Add(new MySqlParameter("quantidade", item.Quantidade));
                        cmd.Parameters.Add(new MySqlParameter("valorUnitario", item.ValorUnitario));
                        cmd.Parameters.Add(new MySqlParameter("pdesconto", item.pDesconto));
                        cmd.Parameters.Add(new MySqlParameter("valor", item.Valor));
                        await cmd.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "insert into itenspedido (IdPedido, IdProduto, Quantidade, ValorUnitario, pDesconto, Valor) values (@idPedido, @idProduto, @quantidade, @valorUnitario, @pdesconto, @valor)";
                        cmd.Parameters.Add(new MySqlParameter("idPedido", item.idPedido));
                        cmd.Parameters.Add(new MySqlParameter("idProduto", item.idProduto));
                        cmd.Parameters.Add(new MySqlParameter("quantidade", item.Quantidade));
                        cmd.Parameters.Add(new MySqlParameter("valorUnitario", item.ValorUnitario));
                        cmd.Parameters.Add(new MySqlParameter("pdesconto", item.pDesconto));
                        cmd.Parameters.Add(new MySqlParameter("valor", item.Valor));
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return await GetPedido(pedido.idCliente!);
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
        /// Troca Status do Pedido
        /// </summary>
        /// <param name="ID">Id do Pedido</param>
        /// <param name="PedidoStatus">Novo Status</param>
        /// <returns>Pedido atualizado</returns>
        public async Task<Pedidos?> SetPedidoStatus(string ID, string PedidoStatus)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                if(PedidoStatus == "Entregue")
                {
                    string sql = "update pedidos set Status = @status, DataEntrega = @entregueEm where idPedido = @id";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.Add(new MySqlParameter("id", ID));
                    cmd.Parameters.Add(new MySqlParameter("status", PedidoStatus));
                    cmd.Parameters.Add(new MySqlParameter("entregueEm", DateTime.Now.Date));
                    var ResultSet = await cmd.ExecuteNonQueryAsync();
                }
                else
                {
                    string sql = "update pedidos set Status = @status where idPedido = @id";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.Add(new MySqlParameter("id", ID));
                    cmd.Parameters.Add(new MySqlParameter("status", PedidoStatus));
                    var ResultSet = await cmd.ExecuteNonQueryAsync();
                }

                return await GetPedido(ID);
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
        /// Retorna lista de pedidos por período informado
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        public async Task<List<Pedidos>> GetPedidosPorPeriodo(PageInfo Page, string Campo, DateTime De, DateTime Ate)
        {
            var pedidos = new List<Pedidos>();
            var PageNumber = Page.Page ?? 1;
            var PageSize = Page.PageSize ?? 10;
            int TotalPages;
            int TotalRecords;

            MySqlConnection? conn = null;
            MySqlConnection? conn2 = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                conn2 = new MySqlConnection(Conn);
                conn2.Open();
                string sql_counter = "select Count(*) AS Total from pedidos  WHERE (" + Campo + " >= @de) and (" + Campo + " <= @ate)";
                var cmd_counter = new MySqlCommand(sql_counter, conn);
                cmd_counter.Parameters.Add(new MySqlParameter("de", De.Date));
                cmd_counter.Parameters.Add(new MySqlParameter("ate", Ate.Date));
                TotalRecords = Convert.ToInt32(await cmd_counter.ExecuteScalarAsync());

                int inicio = (PageNumber - 1) * PageSize;
                TotalPages = Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));

                Page.Page = PageNumber;
                Page.PageSize = PageSize;
                Page.TotalPages = TotalPages;
                Page.TotalRecords = TotalRecords;

                string sql = "select pedidos.*, clientes.Nome AS Cliente, vendedores.Vendedor, vendedores.pComissao from pedidos Inner Join clientes ON (clientes.idCliente = pedidos.idCliente) INNER JOIN vendedores on (vendedores.idVendedor = pedidos.idVendedor) WHERE (" + Campo + " >= @de) and (" + Campo + " <= @ate)  limit @inicio, @qtd";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("de", De));
                cmd.Parameters.Add(new MySqlParameter("ate", Ate));
                cmd.Parameters.Add(new MySqlParameter("inicio", inicio));
                cmd.Parameters.Add(new MySqlParameter("qtd", PageSize));
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Pedidos pedido = new(
                                            reader["idCliente"].ToString()!,
                                            reader["idVendedor"].ToString()!,
                                            Convert.ToDecimal(reader["vComissao"].ToString()!),
                                            Convert.ToInt32(reader["pComissao"].ToString()!),
                                            Convert.ToDecimal(reader["ValorTotal"].ToString()!),
                                            Convert.ToDateTime(reader["DataEmissao"].ToString()!),
                                            Convert.ToDateTime(reader["DataEntrega"].ToString()!),
                                            reader["status"].ToString()!,
                                            reader["idPedido"].ToString()!)
                    {
                        Cliente = reader["Cliente"].ToString()!,
                        Vendedor = reader["Vendedor"].ToString()!
                    };
                    string sql_itens = "select itenspedido.*, produtos.Descricao from itenspedido inner join produtos ON (Produtos.idProduto = Itenspedido.IdProduto) where idPedido = @id";
                    var cmd_itens = new MySqlCommand(sql_itens, conn2);
                    cmd_itens.Parameters.Add(new MySqlParameter("id", pedido.idPedido));
                    var reader_itens = await cmd_itens.ExecuteReaderAsync();
                    while (await reader_itens.ReadAsync())
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

                    if (pedido != null)
                        pedido.ETag = HashMD5.Hash(JsonSerializer.Serialize(pedido));

                    pedidos.Add(pedido);
                }

                return pedidos;

            }
            catch (IndexOutOfRangeException)
            {
                return pedidos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
                conn2?.Close();
            }
        }

        /// <summary>
        /// Retorna lista de pedidos por filtro LIKE
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        public async Task<List<Pedidos>> GetPedidosFiltroLike(PageInfo Page, string Campo, string Termo)
        {
            var pedidos = new List<Pedidos>();
            var PageNumber = Page.Page ?? 1;
            var PageSize = Page.PageSize ?? 10;
            int TotalPages;
            int TotalRecords;

            MySqlConnection? conn = null;
            MySqlConnection? conn2 = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                conn2 = new MySqlConnection(Conn);
                conn2.Open();
                string sql_counter = "select Count(*) AS Total from pedidos Inner Join clientes ON (clientes.idCliente = pedidos.idCliente) INNER JOIN vendedores on (vendedores.idVendedor = pedidos.idVendedor) WHERE (" + Campo + " LIKE @termo)";
                var cmd_counter = new MySqlCommand(sql_counter, conn);
                cmd_counter.Parameters.Add(new MySqlParameter("termo", "%" + Termo + "%"));
                TotalRecords = Convert.ToInt32(await cmd_counter.ExecuteScalarAsync());

                int inicio = (PageNumber - 1) * PageSize;
                TotalPages = Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));

                Page.Page = PageNumber;
                Page.PageSize = PageSize;
                Page.TotalPages = TotalPages;
                Page.TotalRecords = TotalRecords;

                string sql = "select pedidos.*, clientes.Nome AS Cliente, vendedores.Vendedor, vendedores.pComissao from pedidos Inner Join clientes ON (clientes.idCliente = pedidos.idCliente) INNER JOIN vendedores on (vendedores.idVendedor = pedidos.idVendedor) WHERE (" + Campo + " LIKE @termo) limit @inicio, @qtd";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("termo", "%" + Termo + "%"));
                cmd.Parameters.Add(new MySqlParameter("inicio", inicio));
                cmd.Parameters.Add(new MySqlParameter("qtd", PageSize));
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Pedidos pedido = new(
                                            reader["idCliente"].ToString()!,
                                            reader["idVendedor"].ToString()!,
                                            Convert.ToDecimal(reader["vComissao"].ToString()!),
                                            Convert.ToInt32(reader["pComissao"].ToString()!),
                                            Convert.ToDecimal(reader["ValorTotal"].ToString()!),
                                            Convert.ToDateTime(reader["DataEmissao"].ToString()!),
                                            Convert.ToDateTime(reader["DataEntrega"].ToString()!),
                                            reader["status"].ToString()!,
                                            reader["idPedido"].ToString()!)
                    {
                        Cliente = reader["Cliente"].ToString()!,
                        Vendedor = reader["Vendedor"].ToString()!
                    };
                    string sql_itens = "select itenspedido.*, produtos.Descricao from itenspedido inner join produtos ON (Produtos.idProduto = Itenspedido.IdProduto) where idPedido = @id";
                    var cmd_itens = new MySqlCommand(sql_itens, conn2);
                    cmd_itens.Parameters.Add(new MySqlParameter("id", pedido.idPedido));
                    var reader_itens = await cmd_itens.ExecuteReaderAsync();
                    while (await reader_itens.ReadAsync())
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

                    if (pedido != null)
                        pedido.ETag = HashMD5.Hash(JsonSerializer.Serialize(pedido));

                    pedidos.Add(pedido);
                }

                return pedidos;

            }
            catch (IndexOutOfRangeException)
            {
                return pedidos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
                conn2?.Close();
            }
        }

        /// <summary>
        /// Retorna lista de pedidos por filtro Igual
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        public async Task<List<Pedidos>> GetPedidosFiltroIgual(PageInfo Page, string Campo, string Termo)
        {
            var pedidos = new List<Pedidos>();
            var PageNumber = Page.Page ?? 1;
            var PageSize = Page.PageSize ?? 10;
            int TotalPages;
            int TotalRecords;

            MySqlConnection? conn = null;
            MySqlConnection? conn2 = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                conn2 = new MySqlConnection(Conn);
                conn2.Open();
                string sql_counter = "select Count(*) AS Total from pedidos WHERE (" + Campo + " = @termo)";
                var cmd_counter = new MySqlCommand(sql_counter, conn);
                cmd_counter.Parameters.Add(new MySqlParameter("termo", Termo));
                TotalRecords = Convert.ToInt32(await cmd_counter.ExecuteScalarAsync());

                int inicio = (PageNumber - 1) * PageSize;
                TotalPages = Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));

                Page.Page = PageNumber;
                Page.PageSize = PageSize;
                Page.TotalPages = TotalPages;
                Page.TotalRecords = TotalRecords;


                string sql = "select pedidos.*, clientes.Nome AS Cliente, vendedores.Vendedor, vendedores.pComissao from pedidos Inner Join clientes ON (clientes.idCliente = pedidos.idCliente) INNER JOIN vendedores on (vendedores.idVendedor = pedidos.idVendedor) WHERE (" + Campo + " = @termo) limit @inicio, @qtd";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("termo", Termo));
                cmd.Parameters.Add(new MySqlParameter("inicio", inicio));
                cmd.Parameters.Add(new MySqlParameter("qtd", PageSize));
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Pedidos pedido = new(
                                            reader["idCliente"].ToString()!,
                                            reader["idVendedor"].ToString()!,
                                            Convert.ToDecimal(reader["vComissao"].ToString()!),
                                            Convert.ToInt32(reader["pComissao"].ToString()!),
                                            Convert.ToDecimal(reader["ValorTotal"].ToString()!),
                                            Convert.ToDateTime(reader["DataEmissao"].ToString()!),
                                            Convert.ToDateTime(reader["DataEntrega"].ToString()!),
                                            reader["status"].ToString()!,
                                            reader["idPedido"].ToString()!)
                    {
                        Cliente = reader["Cliente"].ToString()!,
                        Vendedor = reader["Vendedor"].ToString()!
                    };
                    string sql_itens = "select itenspedido.*, produtos.Descricao from itenspedido inner join produtos ON (Produtos.idProduto = Itenspedido.IdProduto) where idPedido = @id";
                    var cmd_itens = new MySqlCommand(sql_itens, conn2);
                    cmd_itens.Parameters.Add(new MySqlParameter("id", pedido.idPedido));
                    var reader_itens = await cmd_itens.ExecuteReaderAsync();
                    while (await reader_itens.ReadAsync())
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

                    if (pedido != null)
                        pedido.ETag = HashMD5.Hash(JsonSerializer.Serialize(pedido));

                    pedidos.Add(pedido);
                }

                return pedidos;

            }
            catch (IndexOutOfRangeException)
            {
                return pedidos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn?.Close();
                conn2?.Close();
            }
        }
    }
}
