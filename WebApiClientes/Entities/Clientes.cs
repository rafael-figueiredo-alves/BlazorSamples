using System.ComponentModel.DataAnnotations;

namespace WebApiClientes.Entities
{
    /// <summary>
    /// Entidade Clientes
    /// </summary>
    public class Clientes
    {
        /// <summary>
        /// Campo Id do tipo numérico e auto-incrementável
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Campo nome
        /// </summary>
        [Required]
        [StringLength(75)]
        public string? Nome { get; set; }
        /// <summary>
        /// Campo Endereço
        /// </summary>
        [Required]
        [StringLength(120)]
        public string? Endereco { get; set;}
        /// <summary>
        /// Campo Telefone
        /// </summary>
        [StringLength(15)]
        public string? Telefone { get; set; }
        /// <summary>
        /// Campo Celular
        /// </summary>
        [StringLength(15)]
        public string? Celular { get; set;}
        /// <summary>
        /// Campo e-mail
        /// </summary>
        [StringLength(150)]
        public string? Email { get; set;}

        /// <summary>
        /// Método construtor da classe/entidade
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_nome"></param>
        /// <param name="_endereco"></param>
        /// <param name="_telefone"></param>
        /// <param name="_celular"></param>
        /// <param name="_email"></param>
        public Clientes(int _id, string _nome,  string _endereco, string _telefone, string _celular, string _email)
        {
            Id = _id;
            Nome = _nome;
            Endereco = _endereco;
            Telefone = _telefone;
            Celular = _celular;
            Email = _email;
        }

        /// <summary>
        /// Método construtor da classe/entidade
        /// </summary>
        public Clientes()
        {

        }
    }
}
