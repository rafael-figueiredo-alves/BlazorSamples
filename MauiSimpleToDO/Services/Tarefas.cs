using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MauiSimpleToDO.Entities;
using System.Text.Json;

namespace MauiSimpleToDO.Services
{
    public class Tarefas : ITarefas
    {
        IJSRuntime js { get; set; }

        private readonly int QtdTarefasPorPagina = 5;

        const string DBKey = "SimpleToDOAppBD";
        private string Dados { get; set; }
        private List<Tarefa> _tarefas { get; set; }

        private async Task LerBD()
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
            _tarefas = new List<Tarefa>();
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

        public async Task<List<Tarefa>> GetTarefas()
        {
            await LerBD();
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
            _tarefas!.FirstOrDefault(_task => _task.id == _tarefa.id)!.tarefa = _tarefa.tarefa;
            _tarefas!.FirstOrDefault(_task => _task.id == _tarefa.id)!.descricao = _tarefa.descricao;
            GravarBD();
        }

        public void SetTaskDone(Guid _id, bool Done)
        {
            _tarefas!.FirstOrDefault(_task => _task.id == _id)!.feito = Done;
            GravarBD();
        }

        public async Task<PaginaTarefas> GetTarefasPage(int pageIndex = 1, string SearchTask = "")
        {
            List<Tarefa> _result;

            await LerBD();
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

            if (!string.IsNullOrEmpty(SearchTask))
            {
                _result = _tarefas!.Where(_task => _task.tarefa.IndexOf(SearchTask, StringComparison.OrdinalIgnoreCase) >= 0 || _task.descricao.IndexOf(SearchTask, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            else
            {
                _result = _tarefas!.ToList();
            }

            double TotalPaginas = Math.Ceiling((double)_result!.Count / QtdTarefasPorPagina);
            PaginaTarefas paginaTarefas = new PaginaTarefas();
            paginaTarefas.tarefas = _result.Skip((pageIndex - 1) * QtdTarefasPorPagina).Take(QtdTarefasPorPagina).ToList();
            paginaTarefas.totalPaginas = Convert.ToInt16(TotalPaginas);

            return paginaTarefas;
        }
    }
}
