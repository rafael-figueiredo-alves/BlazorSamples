using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using static System.Net.Mime.MediaTypeNames;

namespace BlazorClientes.Pages
{
    public class GuideBase : ComponentBase
    {
        [Inject] protected IUserData? Data {  get; set; }
        protected void GravarValorTeste() => Data!.SaveData();
    }
}
