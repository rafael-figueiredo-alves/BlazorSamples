using BlazorClientes.Services.Interfaces;
using BlazorClientes.Shared.Entities;
using BlazorClientes.Shared.Entities.PageResults;
using Microsoft.AspNetCore.Components;

namespace BlazorClientes.Shared.Components
{
    public class ChooseDlgBase : ComponentBase
    {
        [Inject] protected IClientes? Clientes { get; set; }
        [Inject] protected IVendedores? Vendedores { get; set; }
        [Inject] protected IProdutos? ProdutosService { get; set; }

        protected bool Exibir { get; set; } = false;
        protected ChooseType Kind { get; set; } = ChooseType.Customers;
        [Parameter] public string? Titulo { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback<object?> OnSelectClick { get; set; }
        [CascadingParameter] public UITheming? Theme { get; set; }


        protected (string? Filtro, int? FiltroIndice) FiltroSelecionado { get; set; }
        protected Dictionary<string, int>? Filtros { get; set; }
        protected int PaginaAtual { get; set; } = 1;
        protected int QuantidadeTotalPaginas { get; set; } = 1;
        protected int ItensPorPagina { get; set; } = 10;
        protected int TotalDeRegistros { get; set; } = 0;
        protected List<Clientes>? ListaClientes { get; set; } = null;
        protected List<Vendedores>? ListaVendedores { get; set; } = null;
        protected List<Produtos>? ListaProdutos { get; set; } = null;
        protected string? Title { get; set; } = "Procurar Cliente";
        protected string? TermoBusca { get; set; } = null;

        public void Exibe(ChooseType Choose)
        {
            Exibir = true;
            Kind = Choose;
            switch(Kind)
            {
                case ChooseType.Customers:
                    Titulo = "Selecionar Cliente";
                    break;
                case ChooseType.Products:
                    Titulo = "Selecionar Produto";
                    break;
                case ChooseType.Salespeople:
                    Titulo = "Selecionar Vendedor";
                    break;
                default:
                    Titulo = "Selecionar Cliente";
                    break;
            }
            PaginaAtual = 1;
            ItensPorPagina = 10;
            TotalDeRegistros = 0;
            GetPage(PaginaAtual);
            FillFilters();
        }

        protected void GetChecked(object? obj)
        {
            OnSelectClick.InvokeAsync(obj);
            Oculta();
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
            switch(Kind)
            {
                case ChooseType.Customers:
                    {
                        TermoBusca = null;
                        FiltroSelecionado = (Filtros!.Where(x => x.Value == args.Filtro).FirstOrDefault().Key, args.Filtro);

                        if (args.Filtro != null)
                        {
                            GetPage(PaginaAtual, (FiltrosCliente)args.Filtro, args.Termo);
                            TermoBusca = args.Termo;
                        }
                    }
                    break;
                case ChooseType.Salespeople:
                    {
                        TermoBusca = null;
                        FiltroSelecionado = (Filtros!.Where(x => x.Value == args.Filtro).FirstOrDefault().Key, args.Filtro);

                        if (args.Filtro != null)
                        {
                            GetPage(PaginaAtual, (FiltroVendedor)args.Filtro, args.Termo);
                            TermoBusca = args.Termo;
                        }
                    }
                    break;
                case ChooseType.Products:
                    {
                        TermoBusca = null;
                        FiltroSelecionado = (Filtros!.Where(x => x.Value == args.Filtro).FirstOrDefault().Key, args.Filtro);

                        if (args.Filtro != null)
                        {
                            GetPage(PaginaAtual, (FiltroProdutos)args.Filtro, args.Termo);
                            TermoBusca = args.Termo;
                        }
                    }
                    break;
            }
        }

        protected void GetPageClick(int Page)
        {
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(Page, FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(Page);
            }
        }

        protected void OnChangeQtdItensPorPagina(ChangeEventArgs args)
        {
            ItensPorPagina = Convert.ToInt32(args.Value);
            if (!string.IsNullOrEmpty(TermoBusca))
            {
                GetPage(PaginaAtual, FiltroSelecionado.FiltroIndice!, TermoBusca);
            }
            else
            {
                GetPage(PaginaAtual);
            }
        }

        protected async void GetPage(int Page, object? Filtro = null, string? Termo = null)
        {
        GetPageBeginning:

            switch(Kind)
            {
                case ChooseType.Customers:
                    {
                        PageClientes? Pagina = await Clientes!.GetClientes(Page, ItensPorPagina, (FiltrosCliente?)Filtro, Termo);

                        if (Pagina!.Clientes != null)
                        {
                            ListaClientes = new();
                            foreach (var cliente in Pagina!.Clientes)
                            {
                                ListaClientes.Add(new Clientes(cliente.Nome!, cliente.Endereco!, cliente.Telefone!, cliente.Celular!, cliente.Email!, cliente.ETag, cliente.Codigo, cliente.idCliente));
                            }
                        }
                        else
                        {
                            ListaClientes = null;
                        }

                        PaginaAtual = Page;
                        QuantidadeTotalPaginas = (int)Pagina!.TotalPaginas!;
                        TotalDeRegistros = (int)Pagina!.TotalRecords!;

                        if ((PaginaAtual > 1) && ((ListaClientes == null) || (!ListaClientes!.Any())))
                        {
                            Page--;
                            goto GetPageBeginning;
                        }

                        StateHasChanged();
                    }
                    break;
                case ChooseType.Salespeople:
                    {
                        PageVendedores? Pagina = await Vendedores!.GetVendedores(Page, ItensPorPagina, (FiltroVendedor?)Filtro, Termo);

                        if (Pagina!.Vendedores != null)
                        {
                            ListaVendedores = new();
                            foreach (var vendedor in Pagina!.Vendedores)
                            {
                                ListaVendedores.Add(new Vendedores(vendedor.Vendedor!, vendedor.pComissao!, vendedor.ETag!, vendedor.Codigo, vendedor.idVendedor!));
                            }
                        }
                        else
                        {
                            ListaVendedores = null;
                        }

                        PaginaAtual = Page;
                        QuantidadeTotalPaginas = (int)Pagina!.TotalPaginas!;
                        TotalDeRegistros = (int)Pagina!.TotalRecords!;

                        if ((PaginaAtual > 1) && ((ListaVendedores == null) || (!ListaVendedores!.Any())))
                        {
                            Page--;
                            goto GetPageBeginning;
                        }

                        StateHasChanged();
                    }
                    break;
                case ChooseType.Products:
                    {
                        PageProdutos? Pagina = await ProdutosService!.GetProdutos(Page, ItensPorPagina, (FiltroProdutos?)Filtro, Termo);

                        if (Pagina!.Produtos != null)
                        {
                            ListaProdutos = new();
                            foreach (var produto in Pagina!.Produtos)
                            {
                                ListaProdutos.Add(new Produtos(produto.Produto, produto.Descricao, produto.Valor, produto.Barcode, produto.ETag, produto.idProduto));
                            }
                        }
                        else
                        {
                            ListaProdutos = null;
                        }

                        PaginaAtual = Page;
                        QuantidadeTotalPaginas = (int)Pagina!.TotalPaginas!;
                        TotalDeRegistros = (int)Pagina!.TotalRecords!;

                        if ((PaginaAtual > 1) && ((ListaProdutos == null) || (!ListaProdutos!.Any())))
                        {
                            Page--;
                            goto GetPageBeginning;
                        }

                        StateHasChanged();
                    }
                    break;
                default:
                    {
                        PageClientes? Pagina = await Clientes!.GetClientes(Page, ItensPorPagina, (FiltrosCliente?)Filtro, Termo);

                        if (Pagina!.Clientes != null)
                        {
                            ListaClientes = new();
                            foreach (var cliente in Pagina!.Clientes)
                            {
                                ListaClientes.Add(new Clientes(cliente.Nome!, cliente.Endereco!, cliente.Telefone!, cliente.Celular!, cliente.Email!, cliente.ETag, cliente.Codigo, cliente.idCliente));
                            }
                        }
                        else
                        {
                            ListaClientes = null;
                        }

                        PaginaAtual = Page;
                        QuantidadeTotalPaginas = (int)Pagina!.TotalPaginas!;
                        TotalDeRegistros = (int)Pagina!.TotalRecords!;

                        if ((PaginaAtual > 1) && ((ListaClientes == null) || (!ListaClientes!.Any())))
                        {
                            Page--;
                            goto GetPageBeginning;
                        }

                        StateHasChanged();
                    }
                    break;
            }
        }
    }

    public enum ChooseType
    {
        Customers,
        Products,
        Salespeople
    }
}
