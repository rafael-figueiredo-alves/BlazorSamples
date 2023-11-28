using BlazorClientes.Shared.Entities;

namespace BlazorClientes.Services.Interfaces
{
    public interface IAuthServices
    { 
        Task SignIn(LoginUser loginUser);
        Task SignUp(usuarios usuario);
        Task Logout();
        Task<string> GetUserName();
        Task<string> GetEmail();
        Task<string> GetUserID();
        Task<DateTime> GetExpiration();
        Task<string> GetRole();
        Task<bool> IsLoggedIn();
        Task<UserProfile> SaveProfile(UserProfile _UserProfile);
        Task GetProfile(int ID);
        void ChangePassword(int ID);
        Task SaveNewPassword(int ID, string NewPassword);
        void ObterContaAdmin(int ID);
        Task ChangeAccount(int ID);
    }
}
