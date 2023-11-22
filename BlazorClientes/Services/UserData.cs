using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorClientes.Services
{
    public class UserData : IUserData
    {
        [Inject] private ILocalStorage? LocalStorage { get; set; }
        private UserDB Dados {  get; set; }
        private string? DataKey { get; set; } = null;

        public UserData() 
        {
            Dados = new UserDB();
        }

        public UserDB UserDB()
        { 
            return Dados; 
        }

        public async Task<IUserData> ReadData(string UserID)
        {
            string? Data = null;

            if (UserID != null)
            {
                DataKey = UserID;
                Data = await LocalStorage!.GetValue(DataKey);
            }

            if (Data != null)
            {
                var DataTemp = JsonSerializer.Deserialize<UserDB?>(Data, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (DataTemp != null)
                {
                    Dados = DataTemp;
                }
            }

            return this;
        }
        public async Task<IUserData> SaveData()
        {
            if (DataKey != null)
            {
                string DadosToSave = JsonSerializer.Serialize<UserDB>(Dados).ToString();
                await LocalStorage!.SetValue(DataKey, DadosToSave);
            }

            return this;
        }
    }
}
