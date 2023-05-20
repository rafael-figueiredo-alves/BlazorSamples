using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SimpleToDOApp.Entities;
using System.Text.Json;

namespace SimpleToDOApp.Services
{
    public class Tarefas : ITarefas
    {
        [Inject]
        IJSRuntime js { get; set; }

        const string DBKey = "SimpleToDOAppBD";
        private List<Tarefa> _tarefas;

        async void LerBD()
        {
            var Dados = await js.InvokeAsync<string>("localStorage.getItem", DBKey);
            if (Dados != null)
            {
                _tarefas = JsonSerializer.Deserialize<List<Tarefa>>(Dados,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            else
            _tarefas = new List<Tarefa>()
                {
                    new Tarefa("Teste", "Testando...")
                };
        }

        public Tarefas(IJSRuntime js) 
        {
            this.js = js;
            LerBD();
            //_tarefas = new List<Tarefa>()
            //    {
            //        new Tarefa("Teste", "Testando...")
            //    };
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
