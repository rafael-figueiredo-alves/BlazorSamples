namespace BlazorClientes.Shared.Entities
{
    public class UITheming
    {
        public string ColorClass { get; set; } = string.Empty;
        public string DataBsTheme { get; set; } = "light";
        public string ColorFontStyle { get; set; } = "black";
        public bool IsDark { get; set; }

        public static UITheming SetDarkMode(bool _isDark = false)
        { 
            if (_isDark)
            {
                return UITheming.SetDarkTheme();
            }
            else
            {
                return UITheming.SetLightTheme();
            }
        }

        public static UITheming SetLightTheme()
        {
            return new UITheming();
        }
        public static UITheming SetDarkTheme() 
        {
            return new UITheming()
            {
                ColorFontStyle = "white",
                ColorClass = "bg-dark",
                DataBsTheme = "dark",
                IsDark = true
            };
        }
    }
}
