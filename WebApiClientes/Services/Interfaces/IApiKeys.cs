namespace WebApiClientes.Services.Interfaces
{
    /// <summary>
    /// Interface para funções referentes a tabela APIKeys
    /// </summary>
    public interface IApiKeys
    {
        /// <summary>
        /// Método para validar se a API Key informada é ou não válida
        /// </summary>
        /// <param name="apiKey">Chave de API</param>
        /// <returns>Verdadeiro se for válida e falso se não for</returns>
        public Task<bool> isValidApiKey(string? apiKey);

        /// <summary>
        /// Método para pegar o limite de requesições aceitas pela API Key
        /// </summary>
        /// <param name="apiKey">Chave de API</param>
        /// <returns>Retorna quantidade de requesições permitidas por segundo. Zero representa que não há limite</returns>
        public Task<int> GetRequestLimit(string? apiKey);
    }
}
