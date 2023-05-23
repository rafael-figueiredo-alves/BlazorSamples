using Microsoft.AspNetCore.Components;

namespace SimpleToDOApp.Pages
{
    public class SobreBase : ComponentBase
    {
        [Inject] private NavigationManager? nav { get; set; }

        protected int Ano = DateTime.Now.Year;
        protected void Voltar()
        {
            nav.NavigateTo("/", false);
        }
    }
}
