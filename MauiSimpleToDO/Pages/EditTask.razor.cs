using Microsoft.AspNetCore.Components;
using MauiSimpleToDO.Entities;
using MauiSimpleToDO.Services;

namespace MauiSimpleToDO.Pages
{
    public class EditTaskBase : ComponentBase
    {
        [Inject] private NavigationManager nav { get; set; }
        [Inject] private ITarefas MyTasks { get; set; }
        [Parameter] public Guid id { get; set; }

        public Tarefa tarefa;

        protected override void OnParametersSet()
        {
            tarefa = MyTasks!.GetTarefa(id);
        }

        protected void EditarTarefa()
        {
            MyTasks!.UpdateTarefa(tarefa!);
            nav!.NavigateTo("/");
        }
    }
}
