using Microsoft.AspNetCore.Components;
using MauiSimpleToDO.Entities;
using MauiSimpleToDO.Services;

namespace MauiSimpleToDO.Pages
{
    public class NovaTarefaBase : ComponentBase
    {
        [Inject] private NavigationManager nav { get; set; }
        [Inject] private ITarefas MyTasks { get; set; }

        public Tarefa tarefa;

        protected override void OnParametersSet()
        {
            tarefa = new Tarefa();
        }

        protected void InserirTarefa()
        {
            MyTasks!.AddTarefa(tarefa!);
            nav!.NavigateTo("/");
        }
    }
}
