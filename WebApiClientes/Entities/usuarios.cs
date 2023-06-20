using System.ComponentModel.DataAnnotations;

namespace WebApiClientes.Entities
{
    /// <summary>
    /// Classe entidade Usuários
    /// </summary>
    public class usuarios
    {
        /// <summary>
        /// Campo de identificação única
        /// </summary>
        public int ID { get; }
        /// <summary>
        /// Nome de display do usuário
        /// </summary>
        public string? Nome { get; set; }
        /// <summary>
        /// Campo de e-mail do usuário
        /// </summary>
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        /// <summary>
        /// Campo de senha do usuário
        /// </summary>
        public string? Senha { get; set; }
        /// <summary>
        /// Campo tipo de senha do usuário
        /// </summary>
        public string? TipoConta { get; set; }

        /// <summary>
        /// Método construtor
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="nome"></param>
        /// <param name="email"></param>
        /// <param name="senha"></param>
        /// <param name="tipoConta"></param>
        public usuarios(int iD, string? nome, string? email, string? senha, string? tipoConta)
        {
            ID = iD;
            Nome = nome;
            Email = email;
            Senha = senha;
            TipoConta = tipoConta;
        }
    }

    /// <summary>
    /// Classe para login
    /// </summary>
    public class LoginUser
    {
        /// <summary>
        /// Campo Email
        /// </summary>
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        /// <summary>
        /// Campo senha
        /// </summary>
        [Required]
        public string? Senha { get; set; }
    }
}
