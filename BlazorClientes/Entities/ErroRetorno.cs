using System.Text.Json.Serialization;

namespace BlazorClientes.Entities
{
    /// <summary>
    /// Classe para enviar mensagens de erro
    /// </summary>
    public class ErroRetorno
    {
        /// <summary>
        /// Campo destinado a mensagem de erro a exibir
        /// </summary>
        [JsonPropertyName("mensagem")]
        public string? Mensagem { get; set; }

        /// <summary>
        /// Informações detalhadas sobre o erro e o que fazer a respeito
        /// </summary>
        [JsonPropertyName("info")]
        public string? Info { get; set; }
        
        /// <summary>
        /// Método Construtor da classe erro
        /// </summary>
        /// <param name="_msg">Mensagem</param>
        /// <param name="_info">Informação extra</param>
        public ErroRetorno(string? _msg, string? _info)
        {
            Mensagem = _msg;
            Info = _info;
        }
    }
}
