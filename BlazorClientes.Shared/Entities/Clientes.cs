using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorClientes.Shared.Entities
{
    /// <summary>
    /// Entidade Clientes
    /// </summary>
    public class Clientes
    {
        /// <summary>
        /// Campo Id do tipo GUID
        /// </summary>
        public string idCliente { get; private set; }
        
        /// <summary>
        /// Campo nome
        /// </summary>
        [Required]
        [StringLength(75, ErrorMessage = "Campo Nome aceita apenas 75 digitos. Verifique se colocou algum espaço ou caracter a mais")]
        public string? Nome { get; set; }
        
        /// <summary>
        /// Campo Endereço
        /// </summary>
        [Required]
        [StringLength(120, ErrorMessage = "Campo Endereco aceita apenas 120 digitos. Verifique se colocou algum espaço ou caracter a mais")]
        public string? Endereco { get; set;}
        
        /// <summary>
        /// Campo Telefone
        /// </summary>
        [StringLength(15, ErrorMessage ="Campo Telefone aceita apenas 15 digitos. Verifique se colocou algum espaço ou caracter a mais")]
        public string? Telefone { get; set; }
       
        /// <summary>
        /// Campo Celular
        /// </summary>
        [StringLength(15, ErrorMessage = "Campo Celular aceita apenas 15 digitos. Verifique se colocou algum espaço ou caracter a mais")]
        public string? Celular { get; set;}

        /// <summary>
        /// Campo e-mail
        /// </summary>
        [EmailAddress]
        [StringLength(150, ErrorMessage = "Campo Email aceita apenas 150 digitos. Verifique se colocou algum espaço ou caracter a mais")]
        public string? Email { get; set; }

        /// <summary>
        /// Propriedade para identificar que registro é novo ou não
        /// </summary>
        public bool isNewRecord { get; set; } = true;

        /// <summary>
        /// Método construtor da classe/entidade
        /// </summary>
        /// <param name="_nome"></param>
        /// <param name="_endereco"></param>
        /// <param name="_telefone"></param>
        /// <param name="_celular"></param>
        /// <param name="_email"></param>
        public Clientes(string _nome,  string _endereco, string _telefone, string _celular, string _email, string? id = null)
        {
            if(id != null)
            {
                idCliente = id!;
                isNewRecord = false;
            }
            else
            {
                idCliente = Guid.NewGuid().ToString();
            }
            
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
            idCliente = Guid.NewGuid().ToString();
        }
    }

    public enum FiltrosCliente
    {
        PorNome,
        PorEndereco
    }

    public class ClientesDTO : Clientes
    {
        /// <summary>
        /// Campo Id do tipo GUID
        /// </summary>
        public new string? idCliente { get; set; }

        public ClientesDTO() { }
    }
}
