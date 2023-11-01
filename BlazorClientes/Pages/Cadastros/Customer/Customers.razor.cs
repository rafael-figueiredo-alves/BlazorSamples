using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Components;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Pages.Cadastros
{
    public class CustomersBase :  ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        #region Injeções de Dependência
        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        [Inject] protected IClientes? Clientes { get; set; }
        #endregion

        #region Variables
        protected List<Clientes>? Lista { get; set; } = new();
        protected int PaginaAtual { get; set; } = 1;
        protected int QuantidadeTotalPaginas { get; set; } = 1;
        protected int ItensPorPagina { get; set; } = 10;
        protected int TotalDeRegistros { get; set; } = 0;
        protected int Teste { get; set; } = 0;
        protected string InfoPaginasERegistros { get; set; } = string.Empty;
        protected ConfirmDlg? MsgDelete { get; set; }
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            GetPage(PaginaAtual);
        }
        
        protected void UpdateCliente(Clientes cliente)
        {
            ParamService!.setParam(cliente);
            NavigationManager!.NavigateTo("editcustomer");
        }

        protected void DeleteCliente(Clientes Cliente)
        {
            MsgDelete!.Exibe();
        }

        protected void ConfirmDeleteCliente()
        {

        }

        protected void OnChangeQtdItensPorPagina(ChangeEventArgs args)
        {
            ItensPorPagina = Convert.ToInt32(args.Value);
            GetPage(PaginaAtual);
        }

        protected async void GetPage(int Page)
        {
            PageClientes? Pagina = await Clientes!.GetClientes(Page, ItensPorPagina);
            Lista = Pagina!.Clientes;
            PaginaAtual = Page;
            QuantidadeTotalPaginas = (int)Pagina!.TotalPaginas!;
            TotalDeRegistros = (int)Pagina!.TotalRecords!;
            InfoPaginasERegistros = $"{TotalDeRegistros} clientes encontrados";
            StateHasChanged();
        }
        #endregion
    }
}
