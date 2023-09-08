using System.ComponentModel.DataAnnotations;

namespace WebApiClientes.Entities
{
    /// <summary>
    /// Classe que manipula o perfil do usuário
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Campo de identificação única
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Nome de display do usuário
        /// </summary>
        public string? Nome { get; set; }
        /// <summary>
        /// Campo de e-mail do usuário
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress]
        public string? Email { get; set; }
        /// <summary>
        /// Primeiro Campo
        /// </summary>
        public string? PrimeiroNome { get; set; }
        /// <summary>
        /// Último Nome
        /// </summary>
        public string? UltimoNome { get; set; }
        /// <summary>
        /// Celular
        /// </summary>
        public string? Celular {  get; set; }
        /// <summary>
        /// Endereço
        /// </summary>
        public string? Endereco { get; set; }
        /// <summary>
        /// Complemento
        /// </summary>
        public string? Complemento { get; set; }
        /// <summary>
        /// Cep
        /// </summary>
        public string? CEP { get; set; }
        /// <summary>
        /// Bairro
        /// </summary>
        public string? Bairro {  get; set; }
        /// <summary>
        /// Cidade
        /// </summary>
        public string? Cidade { get; set; }
        /// <summary>
        /// País
        /// </summary>
        public string? Pais { get; set; }
        /// <summary>
        /// Estado
        /// </summary>
        public string? Estado { get; set; }


        /// <summary>
        /// Método construtor
        /// </summary>
        public UserProfile()
        {
        }
    }
}
