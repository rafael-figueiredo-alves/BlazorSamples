namespace BlazorClientes.Entities
{
    /// <summary>
    /// Classe para fornecer o Token do usuário
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// Token
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Quando expira o Token
        /// </summary>
        public DateTime ExpiraEm { get; set; }
    }
}
