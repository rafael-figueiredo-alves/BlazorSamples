using System.Reflection.Metadata.Ecma335;

namespace BlazorClientes.Services
{
    public class Theming : ITheming
    {
        public UITheming Theme { get; set; } = new() { ClasseCor = "bg-light" };
    }
}
