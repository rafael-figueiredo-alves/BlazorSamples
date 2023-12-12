using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages.Cadastros.Product
{
    public class EditProductBase : ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? Nav { get; set; }
        [Inject] protected IProdutos? ProdutosService { get; set; }
        protected Produtos? Produto { get; set; }
        protected string TituloPagina { get; set; } = string.Empty;

        protected override void OnInitialized()
        {
            if (ParamService!.GetParam() == null)
            {
                Nav!.NavigateTo("products");
            }
            else
            {
                Produto = (Produtos)ParamService!.GetParam()!;
                if (Produto.isNewRecord)
                {
                    TituloPagina = "Novo Produto";
                }
                else
                {
                    TituloPagina = "Editando Produto";
                }
            }
        }

        protected void GoBack()
        {
            Nav!.NavigateTo("products");
        }

        protected async Task SubmitCustomer()
        {
            await ProdutosService!.InsertOrUpdateProduto(Produto!);
        }
    }
}
