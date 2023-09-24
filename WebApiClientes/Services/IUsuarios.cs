using BlazorClientes.Shared.Entities;

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

        /// <summary>
        /// Método para exibir o perfil do usuário
        /// </summary>
        /// <param name="id">ID do usuário a exibir dados</param>
        /// <returns>Retorna os dados do usuário (perfil)</returns>
        public Task<UserProfile> UserProfile(int id);

        /// <summary>
        /// Salva alterações no perfil de usuário
        /// </summary>
        /// <param name="profile">Perfil do usuário</param>
        /// <returns>Perfil do Usuário</returns>
        public Task<UserProfile> SaveUserProfile(UserProfile profile);

        /// <summary>
        /// Método para Trocar a senha do usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="password">nova senha</param>
        /// <returns>Verdadeiro se operação de troca fopi bem sucedida</returns>
        public Task<bool> ChangePassword(int id, string password);

        /// <summary>
        /// Método para trocar tipo de conta
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <param name="accountType">Tipo de conta</param>
        /// <returns>Verdadeiro se operação foi bem sucedida</returns>
        public Task<bool> ChangeAccountType(int id, string accountType);
    }
}
