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
        private string? Dados { get; set; }
        private List<Tarefa>? _tarefas { get; set; }

        private async string LerBD()
        {
           Dados = await (js.InvokeAsync<string>("localStorage.getItem", DBKey));
        }

        async void GravarBD()
        {
            await js.InvokeVoidAsync("localStorage.setItem", DBKey, JsonSerializer.Serialize(_tarefas));
        }

        public Tarefas(IJSRuntime js) 
        {
            this.js = js;
            //if (Dados == null)
            //{
            //    _tarefas = new List<Tarefa>() { };
            //}
            //else
            //{
            //    if (Dados.Length != 0)
            //    {
            //        _tarefas = JsonSerializer.Deserialize<List<Tarefa>>(Dados);
            //    }
            //    else
            //    {
            //        _tarefas = new List<Tarefa> { };
            //    }
            //}
        }
        public void AddTarefa(Tarefa _tarefa)
        {
            _tarefas!.Add(_tarefa);
            GravarBD();
        }

        public Tarefa GetTarefa(Guid _id)
        {
            return _tarefas!.FirstOrDefault(_task => _task.id == _id)!;
        }

        public List<Tarefa> GetTarefas()
        {
            LerBD();
            if (Dados == null)
            {
                _tarefas = new List<Tarefa>() { };
            }
            else
            {
                if (Dados.Length != 0)
                {
                    _tarefas = JsonSerializer.Deserialize<List<Tarefa>>(Dados);
                }
                else
                {
                    _tarefas = new List<Tarefa> { };
                }
            }
            return _tarefas!;
        }

        public bool RemoveTarefa(Guid _tarefaId)
        {
            bool result = _tarefas!.Remove(GetTarefa(_tarefaId));
            GravarBD();
            return result;
        }

        public void UpdateTarefa(Tarefa _tarefa)
        {
            RemoveTarefa(_tarefa.id);
            AddTarefa(_tarefa);
        }

        public void SetTaskDone(Guid _id, bool Done)
        {
            _tarefas!.FirstOrDefault(_task => _task.id == _id)!.feito = Done;
            GravarBD();
        }
    }
}
