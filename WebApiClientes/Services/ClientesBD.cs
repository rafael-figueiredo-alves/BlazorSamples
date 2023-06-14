using MySql.Data.MySqlClient;
using WebApiClientes.Entities;

namespace WebApiClientes.Services
{
    public class ClientesBD : IClientes
    {
        public List<Clientes> GetClientes()
        {
            var clientes = new List<Clientes>();

            MySqlConnection? conn = null;
            try
            {
                string connStr = @"Server=localhost;Database=myerp;Uid=root;Pwd='_sql';Connect Timeout=30;";
                conn = new MySqlConnection(connStr);
                conn.Open();
                string sql = "select * from clientes";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
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
            catch (MySqlException ex)
            {
                return clientes; 
            }
            finally 
            { 
                conn?.Close(); 
            }
        }
    }
}
