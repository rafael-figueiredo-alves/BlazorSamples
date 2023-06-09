﻿using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using WebApiClientes.Entities;

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
                string TipoConta = "Admin";

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
                byte[] hash = md5.ComputeHash(inputBytes);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
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
    }
}
