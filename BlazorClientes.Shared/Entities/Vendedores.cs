using BlazorClientes.Shared.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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
        /// %Campo Código do Vendedor
        /// </summary>
        public uint? Codigo { get; set; }

        /// <summary>
        /// Propriedade para identificar que registro é novo ou não
        /// </summary>
        public bool isNewRecord { get; set; } = true;

        public string? ETag { get; set; }

        /// <summary>
        /// Método construtor
        /// </summary>
        /// <param name="vendedor">Nome do vendedor</param>
        /// <param name="_pComissao">% de Comissão</param>
        /// <param name="_idVendedor">Opcional - ID do vendedor</param>
        public Vendedores(string? vendedor, int _pComissao, uint? codigo, string? _idVendedor = null)
        {
            if(_idVendedor != null)
            {
                idVendedor = _idVendedor!;
                isNewRecord = false;
            }
            else
            {
                idVendedor = Guid.NewGuid().ToString();
            }

            Vendedor = vendedor;
            pComissao = _pComissao;
            Codigo = codigo;
            ETag = HashMD5.Hash(JsonSerializer.Serialize(this));
        }

        public Vendedores(string? vendedor, int _pComissao, string? _Etag = null, uint? codigo = null, string? _idVendedor = null)
        {
            if (_idVendedor != null)
            {
                idVendedor = _idVendedor!;
                isNewRecord = false;
            }
            else
            {
                idVendedor = Guid.NewGuid().ToString();
            }

            Vendedor = vendedor;
            pComissao = _pComissao;
            Codigo = codigo;
            ETag = _Etag;
        }

        /// <summary>
        /// Construtor simples
        /// </summary>
        public Vendedores()
        {
            idVendedor = Guid.NewGuid().ToString();
        }
    }

    public enum FiltroVendedor
    {
        PorNome,
        PorCodigo
    }

    public class VendedoresDTO : Vendedores
    {
        /// <summary>
        /// Campo Id do tipo GUID
        /// </summary>
        public new string? idVendedor { get; set; }

        public VendedoresDTO() { }

        public VendedoresDTO(string? vendedor, int _pComissao, string? _Etag = null, uint? codigo = null, string? _idVendedor = null)
        {
            if (_idVendedor != null)
            {
                idVendedor = _idVendedor!;
                isNewRecord = false;
            }
            else
            {
                idVendedor = Guid.NewGuid().ToString();
            }

            Vendedor = vendedor;
            pComissao = _pComissao;
            Codigo = codigo;
            ETag = _Etag;
        }
    }
}
