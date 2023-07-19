using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace BlazorClientes.Services
{
    public interface ITheming
    {
        string GetTheme();
        void isDark(bool dark);
    }
}
