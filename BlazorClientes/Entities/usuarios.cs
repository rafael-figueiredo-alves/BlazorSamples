﻿using System.ComponentModel.DataAnnotations;

namespace BlazorClientes.Entities
{
    /// <summary>
    /// Classe entidade Usuários
    /// </summary>
    public class Usuarios
    {
        /// <summary>
        /// Campo de identificação única
        /// </summary>
        public int ID { get; }
        /// <summary>
        /// Nome de display do usuário
        /// </summary>
        [Required(ErrorMessage = "Não é possível criar uma conta sem informar o seu nome.")]
        public string? Nome { get; set; }
        /// <summary>
        /// Campo de e-mail do usuário
        /// </summary>
        [Required(ErrorMessage = "Não é possível criar uma conta sem informar o seu e-mail.")]
        [EmailAddress]
        public string? Email { get; set; }
        /// <summary>
        /// Campo de senha do usuário
        /// </summary>
        [Required(ErrorMessage = "Não é possível criar uma conta sem informar uma senha de no mínimo 6 dígitos.")]
        [MinLength(6, ErrorMessage = "Não é possível criar uma conta sem informar uma senha de no mínimo 6 dígitos.")]
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
        public Usuarios(int iD, string? nome, string? email, string? senha, string? tipoConta)
        {
            ID = iD;
            Nome = nome;
            Email = email;
            Senha = senha;
            TipoConta = tipoConta;
        }

        public Usuarios() 
        {
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
        [Required(ErrorMessage = "Campo e-mail é obrigatório patra efetuar login no sistema!")]
        [EmailAddress]
        public string? Email { get; set; }
        /// <summary>
        /// Campo senha
        /// </summary>
        [Required(ErrorMessage = "Campo senha é obrigatório patra efetuar login no sistema!")]
        public string? Senha { get; set; }
    }
}
