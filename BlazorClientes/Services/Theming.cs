using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace BlazorClientes.Services
{
    public class Theming : ITheming
    {
        private bool _isDark { get; set; } = false;
        public string GetTheme()
        {
            if (_isDark)
                return "bg-dark";
            else
                return "bg-light";
        }

        public void isDark(bool dark)
        {
            _isDark = dark;
        }
    }
}
