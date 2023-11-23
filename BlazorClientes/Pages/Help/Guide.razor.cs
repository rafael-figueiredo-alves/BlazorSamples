using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using static System.Net.Mime.MediaTypeNames;

namespace BlazorClientes.Pages
{
    public class GuideBase : ComponentBase
    {
        public List<Teste>? Lista { get; set; } = new() { Teste.None, Teste.All };
    }

    public enum Teste
    {
        None = 0,
        All = 1
    }
}
