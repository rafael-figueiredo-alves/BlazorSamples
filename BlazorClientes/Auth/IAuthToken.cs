namespace BlazorClientes.Auth
{
    public interface IAuthToken
    {
        Task Login(string Token);
        Task Logout();
    }
}
