using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Shared.Components
{
    public class ChooseDlgBase : ComponentBase
    {
        protected bool Exibir { get; set; } = false;
        protected ChooseType Kind { get; set; } = ChooseType.Customers;
        [Parameter] public string? Titulo { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback<object?> OnSelectClick { get; set; }
        [CascadingParameter] public UITheming? Theme { get; set; }


        protected (string? Filtro, int? FiltroIndice) FiltroSelecionado { get; set; }
        protected Dictionary<string, int>? Filtros { get; set; }
        protected string? Title { get; set; } = "Procurar Cliente";

        public void Exibe(ChooseType Choose)
        {
            Exibir = true;
            Kind = Choose;
            FillFilters();
        }

        protected void FillFilters()
        {
            switch (Kind)
            {
                case ChooseType.Customers:
                    {
                        Filtros = new()
                        {
                            { "Nome", 0 },
                            { "Endereço", 1 },
                            { "Código", 2 }
                         };
                        FiltroSelecionado = ("Nome", 0);
                        Title = "Procurar Cliente";
                    }
                    break;
                case ChooseType.Products:
                    {
                        Filtros = new()
                        {
                            { "Nome", 0 },
                            { "Descrição", 1 },
                            { "Barcode", 2 }
                        };
                        FiltroSelecionado = ("Nome", 0);
                        Title = "Procurar Produto";
                    }
                    break;
                case ChooseType.Salespeople:
                    {
                        Filtros = new()
                        {
                            { "Nome", 0 },
                            { "Código", 1 }
                        };
                        FiltroSelecionado = ("Nome", 0);
                        Title = "Procurar Vendedor";
                    }
                    break;
                default:
                    {
                        Filtros = new()
                        {
                            { "Nome", 0 },
                            { "Endereço", 1 },
                            { "Código", 2 }
                         };
                        FiltroSelecionado = ("Nome", 0);
                        Title = "Procurar Cliente";
                    }
                    break;
            }
            StateHasChanged();
        }

        public void Oculta()
        {
            Exibir = false;
        }

        protected void SearchClick((string? Termo, int? Filtro) args)
        {

        }
    }

    public enum ChooseType
    {
        Customers,
        Products,
        Salespeople
    }
}
