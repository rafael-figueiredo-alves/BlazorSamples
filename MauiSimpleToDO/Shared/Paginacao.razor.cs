using MauiSimpleToDO.Entities;
using Microsoft.AspNetCore.Components;

namespace MauiSimpleToDO.Shared
{
    public class PaginacaoBase : ComponentBase
    {
        [CascadingParameter]
        protected UITheming Theme { get; set; }
        [Parameter] public int PaginaAtual { get; set; } = 1;
        [Parameter] public int QuantidadeTotalPaginas { get; set; }
        [Parameter] public int Raio { get; set; } = 3;
        [Parameter] public EventCallback<int> PaginaSelecionada { get; set; }

        protected List<LinkModel> Links { get; set; }

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

            public string Text { get; set; }
            public int Page { get; set; }
            public bool Enabled { get; set; } = true;
            public bool Active { get; set; } = false;
        }

        protected async Task PaginaSelecionadaLink(LinkModel link)
        {
            if (link.Page == PaginaAtual)
            {
                return;
            }

            if (!link.Enabled)
            {
                return;
            }

            PaginaAtual = link.Page;

            await PaginaSelecionada.InvokeAsync(link.Page);
        }

        protected override void OnParametersSet()
        {
            CarregaPaginas();
        }

        private void CarregaPaginas()
        {
            Links = new List<LinkModel>();

            var isLinkPaginaAnteriorHabilitado = PaginaAtual != 1;
            var paginaAnterior = PaginaAtual - 1;

            Links.Add(new LinkModel(paginaAnterior, isLinkPaginaAnteriorHabilitado, "Anterior"));

            for (int i = 1; i <= QuantidadeTotalPaginas; i++)
            {
                if (i >= PaginaAtual - Raio && i <= PaginaAtual + Raio)
                {
                    Links.Add(new LinkModel(i)
                    {
                        Active = PaginaAtual == i
                    });
                }
            }

            var isLinkProximaPaginaHabilitado = PaginaAtual != QuantidadeTotalPaginas;
            var proximaPagina = PaginaAtual + 1;

            Links.Add(new LinkModel(proximaPagina, isLinkProximaPaginaHabilitado, "Próximo"));
        }
    }
}
