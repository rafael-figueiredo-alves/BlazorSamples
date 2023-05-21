using System.ComponentModel.DataAnnotations;

namespace SimpleToDOApp.Entities
{
    public class Tarefa
    {
        public Guid id { get; set; }
       
        [Required(ErrorMessage = "Não é possível criar ou editar uma tarefa e deixar o campo Tarefa em branco!")]
        [MaxLength(100, ErrorMessage = "Esse campo não pode exceder 100 caracteres.")]
        public string tarefa { get; set; } = string.Empty;
        [Required(ErrorMessage = "Não é possível criar ou editar uma tarefa e deixar o campo Descrição em branco!")]
        [MaxLength(250, ErrorMessage = "Esse campo não pode exceder 250 caracteres.")]
        public string descricao { get; set; } = string.Empty;
        public bool feito { get; set; }

        public Tarefa() 
        {
            id = Guid.NewGuid();
            feito = false;
        }
        public Tarefa(string _tarefa, string _desc) 
        {
            id = Guid.NewGuid();
            tarefa = _tarefa;
            descricao = _desc;
            feito = false;
        }
    }
}
