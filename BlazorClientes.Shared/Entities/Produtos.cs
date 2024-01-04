using BlazorClientes.Shared.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlazorClientes.Shared.Entities
{
    /// <summary>
    /// Entidade Produtos
    /// </summary>
    public class Produtos
    {
        /// <summary>
        /// Campo Id do tipo GUID
        /// </summary>
        public string idProduto { get; private set; }

        /// <summary>
        /// Campo Produto
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Campo {0} aceita apenas {1} digitos. Verifique se colocou algum espaço ou caracter a mais")]
        public string? Produto { get; set; }

        /// <summary>
        /// Campo Descrição
        /// </summary>
        [Required]
        [StringLength(45, ErrorMessage = "Campo {0} aceita apenas {1} digitos. Verifique se colocou algum espaço ou caracter a mais")]
        public string? Descricao { get; set; }

        /// <summary>
        /// Campo Valor / Preço
        /// </summary>
        [Required]
        public decimal? Valor { get; set; } = 0;

        /// <summary>
        /// Campo Barcode
        /// </summary>
        [Required]
        [StringLength(45, ErrorMessage = "Campo {0} aceita apenas {1} digitos. Verifique se colocou algum espaço ou caracter a mais")]
        public string? Barcode { get; set; }

        /// <summary>
        /// Propriedade para identificar que registro é novo ou não
        /// </summary>
        public bool isNewRecord { get; set; } = true;

        public string? ETag { get; set; }

        /// <summary>
        /// Método Construtor
        /// </summary>
        /// <param name="produto">Produto</param>
        /// <param name="descricao">Descrição</param>
        /// <param name="valor">Valor</param>
        /// <param name="barcode">Barcode</param>
        /// <param name="_idProduto">Id do produto</param>
        public Produtos(string? produto, string? descricao, decimal? valor, string? barcode, string? _idProduto = null)
        {
            if(_idProduto != null)
            {
                idProduto = _idProduto;
                isNewRecord = false;
            }
            else
            {
                idProduto = Guid.NewGuid().ToString();
            }

            Produto = produto;
            Descricao = descricao;
            Valor = valor;
            Barcode = barcode;
            ETag = HashMD5.Hash(JsonSerializer.Serialize(this));
        }

        /// <summary>
        /// Método Construtor
        /// </summary>
        /// <param name="produto">Produto</param>
        /// <param name="descricao">Descrição</param>
        /// <param name="valor">Valor</param>
        /// <param name="barcode">Barcode</param>
        /// <param name="_ETag">Etag</param>
        /// <param name="_idProduto">Id do produto</param>
        public Produtos(string? produto, string? descricao, decimal? valor, string? barcode, string? _ETag = null, string? _idProduto = null)
        {
            if (_idProduto != null)
            {
                idProduto = _idProduto;
                isNewRecord = false;
            }
            else
            {
                idProduto = Guid.NewGuid().ToString();
            }

            Produto = produto;
            Descricao = descricao;
            Valor = valor;
            Barcode = barcode;
            ETag = _ETag;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        public Produtos()
        {
            idProduto = Guid.NewGuid().ToString();
        }
    }

    public enum FiltroProdutos
    {
        PorProduto,
        PorDescricao,
        PorBarcode
    }

    public class ProdutosDTO : Produtos
    {
        public new string? idProduto { get; set; }

        public ProdutosDTO()
        {

        }

        public ProdutosDTO(string? produto, string? descricao, decimal? valor, string? barcode, string? _ETag = null, string? _idProduto = null)
        {
            if (_idProduto != null)
            {
                idProduto = _idProduto;
                isNewRecord = false;
            }
            else
            {
                idProduto = Guid.NewGuid().ToString();
            }

            Produto = produto;
            Descricao = descricao;
            Valor = valor;
            Barcode = barcode;
            ETag = _ETag;
        }
    }
}
