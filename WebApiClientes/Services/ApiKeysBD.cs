using MySql.Data.MySqlClient;
using WebApiClientes.Services.Interfaces;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Classe concreta da interface IApiKeys
    /// </summary>
    public class ApiKeysBD : IApiKeys
    {
        private readonly string? Conn;

        /// <summary>
        /// Classe construtora
        /// </summary>
        public ApiKeysBD()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Conn = configuration.GetConnectionString("Default");
        }

        /// <summary>
        /// Método para pegar o limite de requesições aceitas pela API Key
        /// </summary>
        /// <param name="apiKey">Chave de API</param>
        /// <returns>Retorna quantidade de requesições permitidas por segundo. Zero representa que não há limite</returns>
        public async Task<RequestLimits> GetRequestLimit(string? apiKey)
        {
            RequestLimits Limite = new RequestLimits();

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();

                var sql = "SELECT RequestLimit, RequestTimeLimit FROM apikeys WHERE APIKey = @key";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("key", apiKey));
                
                var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    Limite.MaxRequests = Convert.ToInt32(reader["RequestLimit"]);
                    Limite.TimeWindowInSeconds = Convert.ToInt32(reader["RequestTimeLimit"]);
                }

                return Limite;
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
        /// Método para validar se a API Key informada é ou não válida
        /// </summary>
        /// <param name="apiKey">Chave de API</param>
        /// <returns>Verdadeiro se for válida e falso se não for</returns>
        public async Task<bool> isValidApiKey(string? apiKey)
        {
            if(string.IsNullOrEmpty(apiKey)) return false;

            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();

                var sql = "SELECT APIKey FROM apikeys WHERE APIKey = @key";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("key", apiKey));
                
                var reader = await cmd.ExecuteReaderAsync();
                
                return reader.HasRows;

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
