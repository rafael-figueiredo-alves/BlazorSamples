namespace MauiSimpleToDO
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            //Adicionado título ao App no Windows
            var _Window = base.CreateWindow(activationState);
            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                _Window.Title = "Simple To-do app version 1.0.0";
            }

            return _Window;
        }
    }
}