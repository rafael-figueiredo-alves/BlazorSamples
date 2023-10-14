using BlazorClientes.Shared.Entities;
using MySql.Data.MySqlClient;
using WebApiClientes.Services.Interfaces;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Classe de serviços relacionados ao usuário
    /// </summary>
    public class UsuariosBD : IUsuarios
    {
        private readonly string? Conn;

        /// <summary>
        /// Método Construtor
        /// </summary>
        public UsuariosBD()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Conn = configuration.GetConnectionString("Default");
        }

        /// <summary>
        /// Método para criar novo usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<usuarios> CreateUser(usuarios user)
        {
            if(await ExisteUsuario(user.Email!))
            {
                throw new Exception("USER EXISTS");
            }

            MySqlConnection? conn = null;
            try
            {
                string password = CriptografaSenhaComHash(user.Senha!);
                string TipoConta = user.TipoConta!;

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "insert into usuarios (Nome, Email, Senha, TipoConta) values (@nome, @email, @senha, @tipoconta)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("nome", user.Nome));
                cmd.Parameters.Add(new MySqlParameter("email", user.Email));
                cmd.Parameters.Add(new MySqlParameter("senha", password));
                cmd.Parameters.Add(new MySqlParameter("tipoconta", TipoConta));
                await cmd.ExecuteNonQueryAsync();
                sql = "select * from usuarios where (Email = @email)";
                cmd.CommandText = sql;
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        return new usuarios(Convert.ToInt32(reader["ID"].ToString()),
                                              reader["Nome"].ToString()!,
                                              reader["Email"].ToString()!,
                                              reader["Senha"].ToString()!,
                                              reader["TipoConta"].ToString()!);
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
                throw;
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Método para efetuar login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<usuarios> Login(LoginUser user)
        {
            MySqlConnection? conn = null;
            try
            {
                usuarios usuario;
                string password = CriptografaSenhaComHash(user.Senha!);

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from usuarios where (Email = @email) and (Senha = @senha)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("email", user.Email));
                cmd.Parameters.Add(new MySqlParameter("senha", password));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    throw new Exception("NO USER");
                }
                else
                {
                    await reader.ReadAsync();
                    usuario = new usuarios(Convert.ToInt32(reader["ID"].ToString()),
                                            reader["Nome"].ToString()!,
                                            reader["Email"].ToString()!,
                                            reader["Senha"].ToString()!,
                                            reader["TipoConta"].ToString()!);
                }
                conn.Close();

                return usuario;
            }
            catch
            {
                throw;
            }
            finally
            {
                conn?.Close();
            }
        }

        private async Task<bool> ExisteUsuario(string email)
        {
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from usuarios where (Email = @email)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("email", email));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false!;
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Função para criptografar senha
        /// </summary>
        /// <param name="Senha">Senha a criptografar</param>
        /// <returns>Senha Criptografada</returns>
        public static string CriptografaSenhaComHash(string Senha)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Senha);
                byte[] hash = System.Security.Cryptography.MD5.HashData(inputBytes);
                System.Text.StringBuilder sb = new();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString(); // Retorna senha criptografada 
            }
            catch (Exception)
            {
                return null!; // Caso encontre erro retorna nulo
            }
        }

        /// <summary>
        /// Método para pegar perfil do usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        public async Task<UserProfile> UserProfile(int id)
        {
            MySqlConnection? conn = null;
            try
            {
                UserProfile usuario;

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "select * from usuarios where (id = @id)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("id", id));
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    throw new Exception("NO USER");
                }
                else
                {
                    await reader.ReadAsync();
                    usuario = new UserProfile()
                    {
                        ID = Convert.ToInt32(reader["ID"].ToString()),
                        Nome = reader["Nome"].ToString(),
                        PrimeiroNome = reader["PrimeiroNome"].ToString(),
                        UltimoNome = reader["UltimoNome"].ToString(),
                        Celular = reader["Celular"].ToString(),
                        Endereco = reader["Endereco"].ToString(),
                        Complemento = reader["Complemento"].ToString(),
                        Bairro = reader["Bairro"].ToString(),
                        Cidade = reader["Cidade"].ToString(),
                        CEP = reader["CEP"].ToString(),
                        Email = reader["Email"].ToString(),
                        Estado = reader["Estado"].ToString(),
                        Pais = reader["Pais"].ToString()
                    };
                }
                conn.Close();

                return usuario;
            }
            catch
            {
                throw;
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Método de troca de senha
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="password">Nova senha</param>
        /// <returns>Verdadeiro se operação foi bem sucedida</returns>
        public async Task<bool> ChangePassword(int id, string password)
        {
            MySqlConnection? conn = null;
            try
            {
                string _password = CriptografaSenhaComHash(password);

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "update usuarios set Senha = @senha where id = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("senha", _password));
                cmd.Parameters.Add(new MySqlParameter("id", id));
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch
                { 
                    return false; 
                }
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
        /// Método para trocar tipo de conta do usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="accountType">Tipo de conta</param>
        /// <returns>Verdadeiro se operação foi bem sucedida</returns>
        public async Task<bool> ChangeAccountType(int id, string accountType)
        {
            MySqlConnection? conn = null;
            try
            {
                string _accountType = accountType;

                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "update usuarios set TipoConta = @tipo where id = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("tipo", _accountType));
                cmd.Parameters.Add(new MySqlParameter("id", id));
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch
                {
                    return false;
                }
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
        /// Salva Perfil do Usuário
        /// </summary>
        /// <param name="profile">Perfil do Usuário</param>
        /// <returns>Perfil do Usuário</returns>
        public async Task<UserProfile> SaveUserProfile(UserProfile profile)
        {
            if (!string.IsNullOrEmpty(profile.PrimeiroNome))
            {
                profile.Nome = profile.PrimeiroNome + " " + profile.UltimoNome;
                profile.Nome = profile.Nome.TrimEnd();
            }
            MySqlConnection? conn = null;
            try
            {
                conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = " UPDATE usuarios " +
                             " SET Nome = @Nome, " +
                             " PrimeiroNome = @PrimeiroNome, " +
                             " UltimoNome = @UltimoNome, " +
                             " Celular = @Celular, " +
                             " Endereco = @Endereco, " +
                             " Complemento = @Complemento, " +
                             " CEP = @CEP, " +
                             " Bairro = @Bairro, " +
                             " Cidade = @Cidade, " +
                             " Pais = @Pais, " +
                             " Estado = @Estado " +
                             " WHERE ID = @id ";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.Add(new MySqlParameter("Nome", profile.Nome));
                cmd.Parameters.Add(new MySqlParameter("PrimeiroNome", profile.PrimeiroNome));
                cmd.Parameters.Add(new MySqlParameter("UltimoNome", profile.UltimoNome));
                cmd.Parameters.Add(new MySqlParameter("Celular", profile.Celular));
                cmd.Parameters.Add(new MySqlParameter("Endereco", profile.Endereco));
                cmd.Parameters.Add(new MySqlParameter("Complemento", profile.Complemento));
                cmd.Parameters.Add(new MySqlParameter("Bairro", profile.Bairro));
                cmd.Parameters.Add(new MySqlParameter("CEP", profile.CEP));
                cmd.Parameters.Add(new MySqlParameter("Cidade", profile.Cidade));
                cmd.Parameters.Add(new MySqlParameter("Estado", profile.Estado));
                cmd.Parameters.Add(new MySqlParameter("Pais", profile.Pais));
                cmd.Parameters.Add(new MySqlParameter("id", profile.ID));
                await cmd.ExecuteNonQueryAsync();
                sql = "select * from usuarios where (id = @id)";
                cmd.CommandText = sql;
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        return new UserProfile()
                        {
                            ID = Convert.ToInt32(reader["ID"].ToString()),
                            Nome = reader["Nome"].ToString(),
                            PrimeiroNome = reader["PrimeiroNome"].ToString(),
                            UltimoNome = reader["UltimoNome"].ToString(),
                            Celular = reader["Celular"].ToString(),
                            Endereco = reader["Endereco"].ToString(),
                            Complemento = reader["Complemento"].ToString(),
                            Bairro = reader["Bairro"].ToString(),
                            Cidade = reader["Cidade"].ToString(),
                            CEP = reader["CEP"].ToString(),
                            Email = reader["Email"].ToString(),
                            Estado = reader["Estado"].ToString(),
                            Pais = reader["Pais"].ToString()
                        };
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
                throw;
            }
            finally
            {
                conn?.Close();
            }
        }
    }
}
