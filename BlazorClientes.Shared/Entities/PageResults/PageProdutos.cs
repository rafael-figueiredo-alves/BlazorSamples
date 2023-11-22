using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorClientes.Shared.Entities.PageResults
{
    public class PageProdutos
    {
        public string? Endpoint { get; set; }
        public int? Pagina { get; set; }
        public int? TotalPaginas { get; set; }
        public int? TotalRecords { get; set; }
        public string? ETag { get; set; }
        public List<Produtos>? Produtos { get; set; }
    }
}
