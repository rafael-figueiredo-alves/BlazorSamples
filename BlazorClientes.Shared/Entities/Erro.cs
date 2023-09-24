namespace BlazorClientes.Shared.Entities
{
    /// <summary>
    /// Classe para enviar mensagens de erro
    /// </summary>
    public class Erro
    {
        /// <summary>
        /// Campo destinado a mensagem de erro a exibir
        /// </summary>
        public string? Mensagem { get; set; }
        
        /// <summary>
        /// Informações detalhadas sobre o erro e o que fazer a respeito
        /// </summary>
        public string? Info { get; set; }
        
        /// <summary>
        /// Método Construtor da classe erro
        /// </summary>
        /// <param name="_msg">Mensagem</param>
        /// <param name="_info">Informação extra</param>
        public Erro(string? _msg, string? _info)
        {
            Mensagem = _msg;
            Info = _info;
        }
    }
}
