using System.ComponentModel.DataAnnotations;

namespace SimpleToDOApp.Entities
{
    public class Tarefa
    {
        public Guid Id { get; set; }
       
        [Required(ErrorMessage = "Não é possível criar ou editar uma tarefa e deixar o campo Tarefa em branco!")]
        [MaxLength(100, ErrorMessage = "Esse campo não pode exceder 100 caracteres.")]
#pragma warning disable IDE1006 // Estilos de Nomenclatura
        public string tarefa { get; set; } = string.Empty;
#pragma warning restore IDE1006 // Estilos de Nomenclatura
        [Required(ErrorMessage = "Não é possível criar ou editar uma tarefa e deixar o campo Descrição em branco!")]
        [MaxLength(250, ErrorMessage = "Esse campo não pode exceder 250 caracteres.")]
        public string Descricao { get; set; } = string.Empty;
        public bool Feito { get; set; }

        public Tarefa() 
        {
            Id = Guid.NewGuid();
            Feito = false;
        }
        public Tarefa(string _tarefa, string _desc) 
        {
            Id = Guid.NewGuid();
            tarefa = _tarefa;
            Descricao = _desc;
            Feito = false;
        }
    }

    public class PaginaTarefas
    {
        public List<Tarefa>? Tarefas { get; set; }
        public int TotalPaginas { get; set; }
    }
}
