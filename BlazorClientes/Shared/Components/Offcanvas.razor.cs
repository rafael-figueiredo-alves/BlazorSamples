using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorClientes.Shared.Components
{
    public class OffcanvasBase : ComponentBase
    {
        [CascadingParameter] protected UITheming? Theme { get; set; }
        [Inject] protected IJSRuntime? Js { get; set; }
        [Inject] protected NavigationManager? NavManager {  get; set; }

        public async void CadastrarClientes()
        {
            NavManager!.NavigateTo("customers");
            await Js!.InvokeVoidAsync("FecharItemMenu", "#cadastros-collapse");
        }

        public async void CadastrarProdutos()
        {
            NavManager!.NavigateTo("products");
            await Js!.InvokeVoidAsync("FecharItemMenu", "#cadastros-collapse");
        }

        public async void CriarNovoPedido()
        {
            NavManager!.NavigateTo("neworder");
            await Js!.InvokeVoidAsync("FecharItemMenu", "#pedidos-collapse");
        }

        public async void VerificarPedidos()
        {
            NavManager!.NavigateTo("orders");
            await Js!.InvokeVoidAsync("FecharItemMenu", "#pedidos-collapse");
        }

        public async void ConsultarManual()
        {
            NavManager!.NavigateTo("guide");
            await Js!.InvokeVoidAsync("FecharItemMenu", "#ajuda-collapse");
        }

        public async void VerSobre()
        {
            NavManager!.NavigateTo("about");
            await Js!.InvokeVoidAsync("FecharItemMenu", "#ajuda-collapse");
        }

        public async void VerPagina404()
        {
            NavManager!.NavigateTo("pagina404");
            await Js!.InvokeVoidAsync("FecharItemMenu", "#ajuda-collapse");
        }

        public async void CadastrarVendedores()
        {
            NavManager!.NavigateTo("salespeople");
            await Js!.InvokeVoidAsync("FecharItemMenu", "#ajuda-collapse");
        }
    }
}
