using BlazorClientes.Shared.Entities;

namespace BlazorClientes.Services.Interfaces
{
    public interface IUserData
    {
        UserDB UserDB();
        Task<IUserData> ReadData(string? UserID = null);
        Task<IUserData> SaveData(string? UserID = null);
    }
}
