namespace WebApiClientes.Entities
{
    /// <summary>
    /// Classe dos itens do pedido
    /// </summary>
    public class ItensPedido
    {
        /// <summary>
        /// Campo Id do tipo Auto incremento
        /// </summary>
        public int Indice { get; }

        /// <summary>
        /// Campo Id do Pedido
        /// </summary>
        public string? idPedido { get; set; }

        /// <summary>
        /// Campo Id do Vendedor
        /// </summary>
        public string? idProduto { get; set; }

        /// <summary>
        /// Campo Quantidade
        /// </summary>
        public int Quantidade { get; set; } = 0;

        /// <summary>
        /// Campo Valor Unitario
        /// </summary>
        public decimal ValorUnitario { get; set; } = decimal.Zero;

        /// <summary>
        /// Campo % Desconto
        /// </summary>
        public int pDesconto { get; set; } = 0;

        /// <summary>
        /// Campo Valor Total
        /// </summary>
        public decimal Valor { get; set; } = decimal.Zero;

        /// <summary>
        /// Método construtor
        /// </summary>
        /// <param name="indice">Indice</param>
        /// <param name="_idPedido">Id Pedido</param>
        /// <param name="_idProduto">Id Produto</param>
        /// <param name="quantidade">Quantidade</param>
        /// <param name="valorUnitario">Valor Unitário</param>
        /// <param name="_pDesconto">% desconto</param>
        /// <param name="valor">Valor Total</param>
        public ItensPedido(int indice, string? _idPedido, string? _idProduto, int quantidade, decimal valorUnitario, int _pDesconto, decimal valor)
        {
            Indice = indice;
            idPedido = _idPedido;
            idProduto = _idProduto;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            pDesconto = pDesconto;
            Valor = valor;
        }
    }
}
