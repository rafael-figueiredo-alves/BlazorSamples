using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorClientes.Services
{
    public class UserData : IUserData
    {
        private readonly ILocalStorage? LocalStorage;
        private UserDB Dados {  get; set; }
        private string? DataKey { get; set; } = null;

        public UserData(IJSRuntime JS) 
        {
            LocalStorage = new LocalStorage(JS);
            Dados = new UserDB();
        }

        public UserDB UserDB()
        { 
            return Dados; 
        }

        public async Task<IUserData> ReadData(string? UserID = null)
        {
            string? Data = null;

            if ((UserID != null) && (UserID != "-1"))
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
        public async Task<IUserData> SaveData(string? UserID = null)
        {
            if ((UserID != null) && (UserID != "-1"))
            {
                DataKey = UserID;
            }

            if (DataKey != null)
            {
                string DadosToSave = JsonSerializer.Serialize<UserDB>(Dados).ToString();
                await LocalStorage!.SetValue(DataKey, DadosToSave);
            }

            return this;
        }

        public void CleanCache()
        {
            DataKey = null;
            Dados = new();
        }
    }
}
