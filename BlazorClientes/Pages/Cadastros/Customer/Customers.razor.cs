using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Components;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorClientes.Pages.Cadastros
{
    public class CustomersBase : ComponentBase
    {
        #region Parameters
        [CascadingParameter] protected UITheming? Theme { get; set; }
        #endregion

        #region Injeções de Dependência
        [Inject] protected IParamService? ParamService { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        [Inject] protected IClientes? Clientes { get; set; }
        [Inject] protected IJSRuntime? JSRuntime { get; set; }
        #endregion

        #region Variables
        protected List<Clientes>? Lista { get; set; } = new();
        protected Dictionary<string, int>? Filtros { get; set; } = null;
        protected (string? Filtro, int? FiltroIndice) FiltroSelecionado { get; set; }
        protected int PaginaAtual { get; set; } = 1;
        protected int QuantidadeTotalPaginas { get; set; } = 1;
        protected int ItensPorPagina { get; set; } = 10;
        protected int TotalDeRegistros { get; set; } = 0;
        protected string InfoPaginasERegistros { get; set; } = string.Empty;
        protected ConfirmDlg? MsgDelete { get; set; }
        protected Clientes? SelectedCliente { get; set; } = null;
        protected string? TermoBusca { get; set; } = null;
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            Filtros = new()
            {
                { "Nome", 0 },
                { "Endereço", 1 },
                { "Código", 2 }
            };
            FiltroSelecionado = ("Nome", 0);
            GetPage(PaginaAtual);
        }

        protected void GetPageClick(int page)
        {
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(page, (FiltrosCliente)FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(page);
            }
        }

        protected void InsertCliente()
        {
            Clientes NovoCliente = new();
            ParamService!.setParam(NovoCliente);
            NavigationManager!.NavigateTo("editcustomer");
        }
        
        protected void UpdateCliente(Clientes cliente)
        {
            ParamService!.setParam(cliente);
            NavigationManager!.NavigateTo("editcustomer");
        }

        protected void DeleteCliente(Clientes Cliente)
        {
            SelectedCliente = Cliente;
            MsgDelete!.Exibe();
        }

        protected async Task ConfirmDeleteCliente()
        {
            MsgDelete!.Oculta();

            bool Result = await Clientes!.DeleteCliente(SelectedCliente!);

            if (Result)
            {
                if (!string.IsNullOrEmpty(TermoBusca))
                {
                    GetPage(PaginaAtual, (FiltrosCliente)FiltroSelecionado.FiltroIndice!, TermoBusca);
                }
                else
                {
                    GetPage(PaginaAtual);
                }
            }
        }

        protected async void PrintClientes()
        {
            await JSRuntime!.InvokeVoidAsync("printComponent", "#ListaClientes");
        }

        protected void OnChangeQtdItensPorPagina(ChangeEventArgs args)
        {
            ItensPorPagina = Convert.ToInt32(args.Value);
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(PaginaAtual, (FiltrosCliente)FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(PaginaAtual);
            }
        }

        protected async void GetPage(int Page, FiltrosCliente? Filtro = null, string? Termo = null)
        {
            GetPageBeginning:

            PageClientes? Pagina = await Clientes!.GetClientes(Page, ItensPorPagina, Filtro, Termo);

            if (Pagina!.Clientes != null)
            {
                Lista = new();
                foreach (var cliente in Pagina!.Clientes)
                {
                    Lista.Add(new Clientes(cliente.Nome!, cliente.Endereco!, cliente.Telefone!, cliente.Celular!, cliente.Email!, cliente.ETag, cliente.Codigo, cliente.idCliente));
                }
            }
            else
            {
                Lista = null;
            }

            PaginaAtual = Page;
            QuantidadeTotalPaginas = (int)Pagina!.TotalPaginas!;
            TotalDeRegistros = (int)Pagina!.TotalRecords!;
            InfoPaginasERegistros = (TotalDeRegistros == 0 ? "Nenhum cliente encontrado" : (TotalDeRegistros == 1 ? $"{TotalDeRegistros} cliente encontrado" : $"{TotalDeRegistros} clientes encontrados"));
            
            if((PaginaAtual > 1) && ((Lista == null) || (!Lista!.Any()) ))
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

            if(args.Filtro != null)
            {
                GetPage(PaginaAtual, (FiltrosCliente)args.Filtro, args.Termo);
                TermoBusca = args.Termo;
            }     
        }
        #endregion
    }
}
