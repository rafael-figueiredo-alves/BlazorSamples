using SimpleToDOApp.Entities;

namespace SimpleToDOApp.Services
{
    public class Tarefas : ITarefas
    {
        private List<Tarefa> _tarefas;
        public Tarefas() 
        {
            _tarefas = new List<Tarefa>();
        }
        public void AddTarefa(Tarefa _tarefa)
        {
            _tarefas.Add(_tarefa);
        }

        public Tarefa GetTarefa(Guid _id)
        {
            return _tarefas.FirstOrDefault(_task => _task.id == _id)!;
        }

        public List<Tarefa> GetTarefas()
        {
            return _tarefas;
        }

        public bool RemoveTarefa(Guid _tarefaId)
        {
            return _tarefas.Remove(GetTarefa(_tarefaId));
        }

        public void UpdateTarefa(Tarefa _tarefa)
        {
            RemoveTarefa(_tarefa.id);
            AddTarefa(_tarefa);
        }
    }
}
