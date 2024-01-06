namespace BlazorClientes.Shared.Entities
{
    /// <summary>
    /// Classe dos itens do pedido
    /// </summary>
    public class ItensPedido
    {
        /// <summary>
        /// Campo Id do tipo Auto incremento
        /// </summary>
        public int Indice { get; set; }

        /// <summary>
        /// Campo Id do Pedido
        /// </summary>
        public string? idPedido { get; set; }

        /// <summary>
        /// Campo Id do Vendedor
        /// </summary>
        public string? idProduto { get; set; }

        /// <summary>
        /// Campo Descrição do produto comprado
        /// </summary>
        public string? Descricao { get; set; }

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
        public ItensPedido(int indice, string? _idPedido, string? _idProduto, string? _Descricao, int quantidade, decimal valorUnitario, int _pDesconto, decimal? valor = null)
        {
            Indice = indice;
            idPedido = _idPedido;
            idProduto = _idProduto;
            Descricao = _Descricao;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            pDesconto = _pDesconto;
            if(valor != null)
            {
                Valor = (decimal)valor!;
            }
            else
            {
                Valor = CalculaTotal();
            }
        }

        public ItensPedido(string? _idPedido, string? _idProduto, string? _Descricao, int quantidade, decimal valorUnitario, int _pDesconto, decimal? valor = null)
        {
            idPedido = _idPedido;
            idProduto = _idProduto;
            Descricao = _Descricao;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            pDesconto = _pDesconto;
            if (valor != null)
            {
                Valor = (decimal)valor!;
            }
            else
            {
                Valor = CalculaTotal();
            }
        }

        private decimal CalculaTotal()
        {
            return (ValorUnitario * Quantidade) * (1 - (pDesconto / 100M));
        }

        public ItensPedido()
        {

        }
    }
}
