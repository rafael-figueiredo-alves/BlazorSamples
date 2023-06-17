using System.ComponentModel.DataAnnotations;

namespace WebApiClientes.Entities
{
    public class ClienteDTO
    {
        [Required]
        [StringLength(75)]
        public string? Nome { get; set; }
        /// <summary>
        /// Campo Endereço
        /// </summary>
        [Required]
        [StringLength(120)]
        public string? Endereco { get; set; }
        /// <summary>
        /// Campo Telefone
        /// </summary>
        [StringLength(15)]
        public string? Telefone { get; set; }
        /// <summary>
        /// Campo Celular
        /// </summary>
        [StringLength(15)]
        public string? Celular { get; set; }
        /// <summary>
        /// Campo e-mail
        /// </summary>
        [StringLength(150)]
        public string? Email { get; set; }
    }
}
