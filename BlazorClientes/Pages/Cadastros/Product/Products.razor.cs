using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;
using BlazorClientes.Services.Interfaces;
using Microsoft.JSInterop;
using BlazorClientes.Shared.Components;

namespace BlazorClientes.Pages.Cadastros.Product
{
    public class ProductsBase : ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        #region Injeções de Dependência
        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        [Inject] protected IProdutos? ProdutosService { get; set; }
        [Inject] protected IJSRuntime? JSRuntime { get; set; }
        #endregion

        #region Variables
        protected List<Produtos>? Lista { get; set; } = new();
        protected Dictionary<string, int>? Filtros { get; set; } = null;
        protected (string? Filtro, int? FiltroIndice) FiltroSelecionado { get; set; }
        protected int PaginaAtual { get; set; } = 1;
        protected int QuantidadeTotalPaginas { get; set; } = 1;
        protected int ItensPorPagina { get; set; } = 10;
        protected int TotalDeRegistros { get; set; } = 0;
        protected string InfoPaginasERegistros { get; set; } = string.Empty;
        protected ConfirmDlg? MsgDelete { get; set; }
        protected Produtos? SelectedProduto { get; set; } = null;
        protected string? TermoBusca { get; set; } = null;
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            Filtros = new()
            {
                { "Nome", 0 },
                { "Descrição", 1 },
                { "Barcode", 2 }
            };
            FiltroSelecionado = ("Nome", 0);
            GetPage(PaginaAtual);
        }

        protected void GetPageClick(int page)
        {
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(page, (FiltroProdutos)FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(page);
            }
        }

        protected void InsertProduto()
        {
            Produtos NovoProduto = new();
            ParamService!.setParam(NovoProduto);
            NavigationManager!.NavigateTo("editproduct");
        }

        protected void UpdateProduto(Produtos produto)
        {
            ParamService!.setParam(produto);
            NavigationManager!.NavigateTo("editproduct");
        }

        protected void DeleteProduto(Produtos produto)
        {
            SelectedProduto = produto;
            MsgDelete!.Exibe();
        }

        protected async Task ConfirmDeleteProduto()
        {
            MsgDelete!.Oculta();

            bool Result = await ProdutosService!.DeleteProduto(SelectedProduto!);

            if (Result)
            {
                if (!string.IsNullOrEmpty(TermoBusca))
                {
                    GetPage(PaginaAtual, (FiltroProdutos)FiltroSelecionado.FiltroIndice!, TermoBusca);
                }
                else
                {
                    GetPage(PaginaAtual);
                }
            }
        }

        protected void PrintProdutos()
        {
            //Implementar no futuro
        }

        protected void OnChangeQtdItensPorPagina(ChangeEventArgs args)
        {
            ItensPorPagina = Convert.ToInt32(args.Value);
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(PaginaAtual, (FiltroProdutos)FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(PaginaAtual);
            }
        }

        protected async void GetPage(int Page, FiltroProdutos? Filtro = null, string? Termo = null)
        {
        GetPageBeginning:

            PageProdutos? Pagina = await ProdutosService!.GetProdutos(Page, ItensPorPagina, Filtro, Termo);

            if (Pagina!.Produtos != null)
            {
                Lista = new();
                foreach (var produto in Pagina!.Produtos)
                {
                    Lista.Add(new Produtos(produto.Produto, produto.Descricao, produto.Valor, produto.Barcode, produto.ETag, produto.idProduto));
                }
            }
            else
            {
                Lista = null;
            }

            PaginaAtual = Page;
            QuantidadeTotalPaginas = (int)Pagina!.TotalPaginas!;
            TotalDeRegistros = (int)Pagina!.TotalRecords!;
            InfoPaginasERegistros = (TotalDeRegistros == 0 ? "Nenhum produto encontrado" : (TotalDeRegistros == 1 ? $"{TotalDeRegistros} produto encontrado" : $"{TotalDeRegistros} produtos encontrados"));

            if ((PaginaAtual > 1) && ((Lista == null) || (!Lista!.Any())))
            {
                Page--;
                goto GetPageBeginning;
            }

            StateHasChanged();
        }

        protected void SearchClick((string? Termo, int? Filtro) args)
        {
            TermoBusca = null;
            FiltroSelecionado = (Filtros!.Where(x => x.Value == args.Filtro).FirstOrDefault().Key, args.Filtro);

            if (args.Filtro != null)
            {
                GetPage(PaginaAtual, (FiltroProdutos)args.Filtro, args.Termo);
                TermoBusca = args.Termo;
            }
        }
        #endregion
    }
}
