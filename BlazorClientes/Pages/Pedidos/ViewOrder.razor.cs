using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages.Pedidos
{
    public class ViewOrderBase : ComponentBase
    {
        [Inject] protected IPedidos? Pedidos {  get; set; }
        [Inject] protected IParamService? ParamService { get; set; }
        protected BlazorClientes.Shared.Entities.Pedidos? Pedido { get; set; }
        protected string? TituloPagina { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TituloPagina = "Visualizando Pedido";
        }
    }
}
