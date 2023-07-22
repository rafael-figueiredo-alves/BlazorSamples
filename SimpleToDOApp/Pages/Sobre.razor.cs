using Microsoft.AspNetCore.Components;
using SimpleToDOApp.Entities;

namespace SimpleToDOApp.Pages
{
    public class SobreBase : ComponentBase
    {
        [CascadingParameter]
        protected UITheming? Theme { get; set; }
        [Inject] private NavigationManager? nav { get; set; }

        protected int Ano = DateTime.Now.Year;
        protected void Voltar()
        {
            nav.NavigateTo("/", false);
        }
    }
}
