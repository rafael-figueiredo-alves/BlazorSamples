using Microsoft.AspNetCore.Components;
using SimpleToDOApp.Entities;

namespace SimpleToDOApp.Shared
{
    public class PaginacaoBase : ComponentBase
    {
        [CascadingParameter]
        protected UITheming? Theme { get; set; }
        [Parameter] public int paginaAtual { get; set; } = 1;
        [Parameter] public int QuantidadeTotalPaginas { get; set; }
        [Parameter] public int Raio { get; set; } = 3;
        [Parameter] public EventCallback<int> PaginaSelecionada { get; set; }

        protected List<LinkModel>? links { get; set; }

        protected class LinkModel
        {
            public LinkModel(int page) : this(page, true)
            { }

            public LinkModel(int page, bool enabled) : this(page, enabled, page.ToString())
            { }

            public LinkModel(int page, bool enabled, string text)
            {
                Page = page;
                Enabled = enabled;
                Text = text;
            }

            public string? Text { get; set; }
            public int Page { get; set; }
            public bool Enabled { get; set; } = true;
            public bool Active { get; set; } = false;
        }

        protected async Task PaginaSelecionadaLink(LinkModel link)
        {
            if (link.Page == paginaAtual)
            {
                return;
            }

            if (!link.Enabled)
            {
                return;
            }

            paginaAtual = link.Page;

            await PaginaSelecionada.InvokeAsync(link.Page);
        }

        protected override void OnParametersSet()
        {
            CarregaPaginas();
        }

        private void CarregaPaginas()
        {
            links = new List<LinkModel>();

            var isLinkPaginaAnteriorHabilitado = paginaAtual != 1;
            var paginaAnterior = paginaAtual - 1;

            links.Add(new LinkModel(paginaAnterior, isLinkPaginaAnteriorHabilitado, "Anterior"));

            for (int i = 1; i <= QuantidadeTotalPaginas; i++)
            {
                if (i >= paginaAtual - Raio && i <= paginaAtual + Raio)
                {
                    links.Add(new LinkModel(i)
                    {
                        Active = paginaAtual == i
                    });
                }
            }

            var isLinkProximaPaginaHabilitado = paginaAtual != QuantidadeTotalPaginas;
            var proximaPagina = paginaAtual + 1;

            links.Add(new LinkModel(proximaPagina, isLinkProximaPaginaHabilitado, "Próximo"));
        }
    }
}
