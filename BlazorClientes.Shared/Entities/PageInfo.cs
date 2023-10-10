using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorClientes.Shared.Entities
{
    public class PageInfo
    {
        public int? Page { get; set; } = null;
        public int? PageSize { get; set; } = null;
        public int? TotalPages { get; set; } = null;
        public int? TotalRecords { get; set; } = null;
    }
}
