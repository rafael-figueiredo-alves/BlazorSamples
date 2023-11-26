using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorClientes.Shared.Components
{
    public class SearchInputBase : ComponentBase
    {
        [CascadingParameter]
        protected UITheming? Theme { get; set; }
        public string? TermoPesquisa { get; set; }
        [Parameter] public EventCallback<(string?, int?)> SearchClick { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public Dictionary<string, int>? Filtros { get; set; } = null;
        [Parameter] public (string? Filtro, int? FiltroIndice) FiltroSelecionado { get; set; } = (null, null);

        protected async Task Pesquisar()
        {
            await SearchClick.InvokeAsync((TermoPesquisa, FiltroSelecionado.FiltroIndice));
        }

        protected async Task LimparFiltro()
        {
            TermoPesquisa = null;
            await Pesquisar();
        }

        protected void TrocarFiltro(string Filtro, int indice)
        {
            FiltroSelecionado = (Filtro, indice);
            StateHasChanged();
        }

        protected async Task OnKeyUp(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            { 
                await Pesquisar();
            }
        }
    }
}
