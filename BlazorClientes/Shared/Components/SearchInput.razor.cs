using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Entities;

namespace BlazorClientes.Shared.Components
{
    public class SearchInputBase : ComponentBase
    {
        [CascadingParameter]
        protected UITheming? Theme { get; set; }
        public string? TermoPesquisa { get; set; }
        [Parameter] public EventCallback<string> SearchClick { get; set; }
        [Parameter] public EventCallback ClearClick { get; set; }

        protected async Task Pesquisar()
        {
            if (TermoPesquisa == null)
            {
                return;
            }

            await SearchClick.InvokeAsync(TermoPesquisa);
        }

        protected async Task LimparFiltro()
        {
            TermoPesquisa = null;
            await ClearClick.InvokeAsync();
        }
    }
}
