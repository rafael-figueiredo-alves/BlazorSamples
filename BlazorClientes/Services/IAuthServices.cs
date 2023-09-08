using BlazorClientes.Entities;

namespace BlazorClientes.Services
{
    public interface IAuthServices
    {
        Task IsLogged();
        Task SignIn(LoginUser loginUser);
        Task SignUp(Usuarios usuario);
        Task Logout();
        Task<string> GetUserName();
        Task<string> GetEmail();
        Task<string> GetUserID();
        Task<DateTime> GetExpiration();
        Task<bool> IsLoggedIn();
        Task<UserProfile> SaveProfile(UserProfile _UserProfile);
        Task GetProfile(int ID);
    }
}
