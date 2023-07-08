using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using SimpleToDOApp.Entities;
using SimpleToDOApp.Services;

namespace SimpleToDOApp.Pages
{
    public class EditTaskBase : ComponentBase
    {
        [Inject] private NavigationManager? nav { get; set; }
        [Inject] private ITarefas? MyTasks { get; set; }
        [Inject] private IToastService? toastService { get; set; }
        [Parameter] public Guid id { get; set; }

        public Tarefa? tarefa;

        protected override void OnParametersSet()
        {
            tarefa = MyTasks!.GetTarefa(id);
        }

        protected void EditarTarefa()
        {
            MyTasks!.UpdateTarefa(tarefa!);
            toastService!.ShowSuccess("Tarefa atualizada com sucesso!");
            nav!.NavigateTo("/");
        }
    }
}
