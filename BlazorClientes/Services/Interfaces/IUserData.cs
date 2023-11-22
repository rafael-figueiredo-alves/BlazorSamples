using BlazorClientes.Shared.Entities;

namespace BlazorClientes.Services.Interfaces
{
    public interface IUserData
    {
        UserDB UserDB();
        Task<IUserData> ReadData(string UserID);
        Task<IUserData> SaveData();
    }
}
