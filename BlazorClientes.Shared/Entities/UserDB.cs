using BlazorClientes.Shared.Entities.PageResults;

namespace BlazorClientes.Shared.Entities
{
    public class UserDB
    {
        public string? teste { get; set; }
        public List<PageClientes>? Clientes { get; set; }
        public List<PagePedidos>? Pedidos { get; set; }
        public List<PageProdutos>? Produtos { get; set; }
        public List<PageVendedores>? Vendedores { get; set; }
    }
}
