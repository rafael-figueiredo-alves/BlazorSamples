using Microsoft.AspNetCore.Components;
using SimpleToDOApp.Entities;

namespace SimpleToDOApp.Shared
{
    public class SearchInputBase : ComponentBase
    {
        [CascadingParameter]
        protected UITheming? Theme { get; set; }
        public string? TermoPesquisa { get; set; }
        [Parameter] public EventCallback<string> PesquisarTarefa { get; set; }
        [Parameter] public EventCallback Limpar { get; set; }

        protected async Task Pesquisar()
        {
            if (TermoPesquisa == null)
            {
                return;
            }

            await PesquisarTarefa.InvokeAsync(TermoPesquisa);
        }

        protected async Task LimparFiltro()
        {
            TermoPesquisa = null;
            await Limpar.InvokeAsync();
        }
    }
}
