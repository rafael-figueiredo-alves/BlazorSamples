using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using SimpleToDOApp.Entities;
using SimpleToDOApp.Services;

namespace SimpleToDOApp.Pages
{
    public class NovaTarefaBase : ComponentBase
    {
        [Inject] private NavigationManager? nav { get; set; }
        [Inject] private ITarefas? MyTasks { get; set; }
        [Inject] private IToastService? toastService { get; set; }

        public Tarefa? tarefa;

        protected override void OnParametersSet()
        {
            tarefa = new Tarefa();
        }

        protected void InserirTarefa()
        {
            MyTasks!.AddTarefa(tarefa!);
            toastService!.ShowSuccess("Tarefa adicionada com sucesso!");
            nav!.NavigateTo("/");
        }
    }
}
