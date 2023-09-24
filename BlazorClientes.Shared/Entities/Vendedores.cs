using System.ComponentModel.DataAnnotations;

namespace BlazorClientes.Shared.Entities
{
    /// <summary>
    /// Entidade responsável pelos Vendedores
    /// </summary>
    public class Vendedores
    {
        /// <summary>
        /// Campo Id do tipo GUID
        /// </summary>
        public string idVendedor { get; private set; }

        /// <summary>
        /// Campo Vendedor
        /// </summary>
        [Required]
        [StringLength(60, ErrorMessage = "Campo {0} aceita apenas {1} digitos. Verifique se colocou algum espaço ou caracter a mais")]
        public string? Vendedor { get; set; }

        /// <summary>
        /// % de comissão do vendedor
        /// </summary>
        public int pComissao { get; set; } = 5;

        /// <summary>
        /// Método construtor
        /// </summary>
        /// <param name="vendedor">Nome do vendedor</param>
        /// <param name="_pComissao">% de Comissão</param>
        /// <param name="_idVendedor">Opcional - ID do vendedor</param>
        public Vendedores(string? vendedor, int _pComissao, string? _idVendedor = null)
        {
            if(idVendedor != null)
            {
                idVendedor = _idVendedor!;
            }
            else
            {
                idVendedor = Guid.NewGuid().ToString();
            }

            Vendedor = vendedor;
            pComissao = _pComissao;
        }

        /// <summary>
        /// Construtor simples
        /// </summary>
        public Vendedores()
        {
            idVendedor = Guid.NewGuid().ToString();
        }
    }
}
