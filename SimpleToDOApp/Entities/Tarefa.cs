namespace SimpleToDOApp.Entities
{
    public class Tarefa
    {
        public Guid id { get; set; }
        public string tarefa { get; set; }
        public string descricao { get; set; }
        public bool feito { get; set; }

        public Tarefa(string _tarefa, string _desc) 
        {
            Id = Guid.NewGuid();
            tarefa = _tarefa;
            descricao = _desc;
            feito = false;
        }

        public void AlterarStatus()
        {
            feito = !feito;
        }
    }
}
