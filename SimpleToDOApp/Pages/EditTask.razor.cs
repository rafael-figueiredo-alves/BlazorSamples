using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using SimpleToDOApp.Entities;
using SimpleToDOApp.Services;

namespace SimpleToDOApp.Pages
{
    public class EditTaskBase : ComponentBase
    {
        [Inject] private NavigationManager? Nav { get; set; }
        [Inject] private ITarefas? MyTasks { get; set; }
        [Inject] private IToastService? ToastService { get; set; }
        [Parameter] public Guid Id { get; set; }

        public Tarefa? tarefa;

        protected override void OnParametersSet()
        {
            tarefa = MyTasks!.GetTarefa(Id);
        }

        protected void EditarTarefa()
        {
            MyTasks!.UpdateTarefa(tarefa!);
            ToastService!.ShowSuccess("Tarefa atualizada com sucesso!");
            Nav!.NavigateTo("/");
        }
    }
}
