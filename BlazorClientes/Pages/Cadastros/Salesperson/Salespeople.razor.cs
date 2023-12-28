using BlazorClientes.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using BlazorClientes.Shared.Entities;
using Microsoft.JSInterop;
using BlazorClientes.Shared.Components;
using BlazorClientes.Shared.Entities.PageResults;

namespace BlazorClientes.Pages.Cadastros.Salesperson
{
    public class SalespeopleBase : ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        #region Injeções de Dependência
        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        [Inject] protected IVendedores? Vendedores { get; set; }
        [Inject] protected IJSRuntime? JSRuntime { get; set; }
        #endregion

        #region Variables
        protected List<Vendedores>? Lista { get; set; } = new();
        protected Dictionary<string, int>? Filtros { get; set; } = null;
        protected (string? Filtro, int? FiltroIndice) FiltroSelecionado { get; set; }
        protected int PaginaAtual { get; set; } = 1;
        protected int QuantidadeTotalPaginas { get; set; } = 1;
        protected int ItensPorPagina { get; set; } = 10;
        protected int TotalDeRegistros { get; set; } = 0;
        protected string InfoPaginasERegistros { get; set; } = string.Empty;
        protected ConfirmDlg? MsgDelete { get; set; }
        protected Vendedores? SelectedVendedor { get; set; } = null;
        protected string? TermoBusca { get; set; } = null;
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            Filtros = new()
            {
                { "Nome", 0 },
                { "Código", 1 }
            };
            FiltroSelecionado = ("Nome", 0);
            GetPage(PaginaAtual);
        }

        protected void GetPageClick(int page)
        {
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(page, (FiltroVendedor)FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(page);
            }
        }

        protected void InsertVendedor()
        {
            Vendedores NovoVendedor = new();
            ParamService!.setParam(NovoVendedor);
            NavigationManager!.NavigateTo("editsalesperson");
        }

        protected void UpdateVendedor(Vendedores vendedor)
        {
            ParamService!.setParam(vendedor);
            NavigationManager!.NavigateTo("editsalesperson");
        }

        protected void DeleteVendedor(Vendedores vendedor)
        {
            SelectedVendedor = vendedor;
            MsgDelete!.Exibe();
        }

        protected async Task ConfirmDeleteVendedor()
        {
            MsgDelete!.Oculta();

            bool Result = await Vendedores!.DeleteVendedor(SelectedVendedor!);

            if (Result)
            {
                if (!string.IsNullOrEmpty(TermoBusca))
                {
                    GetPage(PaginaAtual, (FiltroVendedor)FiltroSelecionado.FiltroIndice!, TermoBusca);
                }
                else
                {
                    GetPage(PaginaAtual);
                }
            }
        }

        protected void PrintVendedores()
        {
            //Implementar no futuro
        }

        protected void OnChangeQtdItensPorPagina(ChangeEventArgs args)
        {
            ItensPorPagina = Convert.ToInt32(args.Value);
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(PaginaAtual, (FiltroVendedor)FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(PaginaAtual);
            }
        }

        protected async void GetPage(int Page, FiltroVendedor? Filtro = null, string? Termo = null)
        {
        GetPageBeginning:

            PageVendedores? Pagina = await Vendedores!.GetVendedores(Page, ItensPorPagina, Filtro, Termo);

            if (Pagina!.Vendedores != null)
            {
                Lista = new();
                foreach (var vendedor in Pagina!.Vendedores)
                {
                    Lista.Add(new Vendedores(vendedor.Vendedor!, vendedor.pComissao!, vendedor.ETag!, vendedor.Codigo, vendedor.idVendedor!));
                }
            }
            else
            {
                Lista = null;
            }

            PaginaAtual = Page;
            QuantidadeTotalPaginas = (int)Pagina!.TotalPaginas!;
            TotalDeRegistros = (int)Pagina!.TotalRecords!;
            InfoPaginasERegistros = (TotalDeRegistros == 0 ? "Nenhum vendedor encontrado" : (TotalDeRegistros == 1 ? $"{TotalDeRegistros} vendedor encontrado" : $"{TotalDeRegistros} vendedores encontrados"));

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
                GetPage(PaginaAtual, (FiltroVendedor)args.Filtro, args.Termo);
                TermoBusca = args.Termo;
            }
        }
        #endregion
    }
}
