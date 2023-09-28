using System.ComponentModel.DataAnnotations;

namespace BlazorClientes.Shared.Entities
{
    /// <summary>
    /// Entidade Pedidos
    /// </summary>
    public class Pedidos
    {
        /// <summary>
        /// Campo Id do tipo GUID
        /// </summary>
        public string idPedido { get; private set; }

        /// <summary>
        /// Campo Id do Cliente
        /// </summary>
        public string? idCliente { get; set; }

        /// <summary>
        /// Campo nome do cliente
        /// </summary>
        public string? Cliente { get; set; } = null;

        /// <summary>
        /// Campo Id do Vendedor
        /// </summary>
        public string? idVendedor { get; set; }

        /// <summary>
        /// Campo Nome do Vendedor
        /// </summary>
        public string? Vendedor { get; set; } = null;

        /// <summary>
        /// Campo % de Comissão do vendedor
        /// </summary>
        public int pComissao { get; set; } = 0;

        /// <summary>
        /// Campo Valor Total da comissão
        /// </summary>
        public decimal? vComissao { get; set; } = 0;

        /// <summary>
        /// Campo Valor Total do Pedido
        /// </summary>
        public decimal? ValorTotal { get; set; } = 0;

        /// <summary>
        /// Data de emissão do pedido
        /// </summary>
        public DateTime DataEmissao { get; set; } = DateTime.Now;

        /// <summary>
        /// Data de entrega do pedido
        /// </summary>
        public DateTime DataEntrega { get; set; } = DateTime.Now.AddDays(10);

        /// <summary>
        /// Status do Pedido
        /// </summary>
        public string Status { get; set; } = "Emitido";

        /// <summary>
        /// Itens do Pedido
        /// </summary>
        public List<ItensPedido> Itens {  get; set; }

        /// <summary>
        /// Método Construtor
        /// </summary>
        /// <param name="_idCliente">id Cliente</param>
        /// <param name="_idVendedor">Id VBendedor</param>
        /// <param name="_vComissao">Valor Comissão</param>
        /// <param name="valorTotal">Total do Pedido</param>
        /// <param name="dataEmissao">Data de emissão do pedido</param>
        /// <param name="dataEntrega">Data de entrega do pedido</param>
        /// <param name="status">Status do Pedido</param>
        /// <param name="_idPedido">Id do Pedido</param>
        public Pedidos(string _idCliente, string _idVendedor, decimal? _vComissao, int? _pComissao, decimal? valorTotal, DateTime dataEmissao, DateTime dataEntrega, string status, string? _idPedido = null)
        {
            if(_idPedido != null)
            {
                idPedido = _idPedido;
            }
            else
            {
                idPedido = Guid.NewGuid().ToString();
            }

            idPedido = idPedido;
            idCliente = _idCliente;
            idVendedor = _idVendedor;
            vComissao = _vComissao;
            if(_pComissao != null)
                pComissao = (int)_pComissao; 
            ValorTotal = valorTotal;
            DataEmissao = dataEmissao;
            DataEntrega = dataEntrega;
            Status = status;
            Itens = new List<ItensPedido>();
        }

        /// <summary>
        /// Construtor
        /// </summary>
        public Pedidos()
        {
            idPedido = Guid.NewGuid().ToString();
            Itens = new List<ItensPedido>();
        }
    }

    public enum FiltrosPedidoPeriodo
    {
        PorDataEmissao,
        PorDataEntrega
    }

    public enum FiltrosPedido
    {
        PorCliente,
        PorVendedor
    }
}
