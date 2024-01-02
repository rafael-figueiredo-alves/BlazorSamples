using BlazorClientes.Shared.Entities.PageResults;
using BlazorClientes.Shared.Entities;

namespace BlazorClientes.Services.Interfaces
{
    public interface IProdutos
    {
        Task<PageProdutos?> GetProdutos(int? Pagina = 1, int? QtdRegistrosPorPagina = 10, FiltroProdutos? FiltrarPor = null, string? Termo = null);
        Task<Produtos?> InsertOrUpdateProduto(Produtos Produto);
        Task<bool> DeleteProduto(Produtos Produto);
        Task<List<Produtos>?> GetAllProdutosToPrint();
        Task<Produtos?> GetProduto(string Codigo);
    }
}
