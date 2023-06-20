using WebApiClientes.Entities;

namespace WebApiClientes.Services
{
    /// <summary>
    /// Interface de serviços de usuário
    /// </summary>
    public interface IUsuarios
    {
        /// <summary>
        /// Função de Login
        /// </summary>
        /// <param name="user">dados de login</param>
        /// <returns>Dados do Usuário</returns>
        public Task<usuarios> Login(LoginUser user);
        /// <summary>
        /// Criar usuário
        /// </summary>
        /// <param name="user">Dados do usuário</param>
        /// <returns>Dados do usuário</returns>
        public Task<usuarios> CreateUser(usuarios user);
    }
}
