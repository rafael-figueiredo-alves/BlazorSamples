using Microsoft.AspNetCore.Components;
using SimpleToDOApp.Entities;
using SimpleToDOApp.Services;
using SimpleToDOApp.Shared;

namespace SimpleToDOApp.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] private ITarefas? MyTasks { get; set; }
        public List<Tarefa>? Lista { get; set; }

        protected Msg? msg;

        private Guid id;

        protected override async void OnInitialized()
        {
            Lista = await MyTasks!.GetTarefas();
            StateHasChanged();
        }

        protected void RemoveTarefa(Guid _id)
        {
            id = _id;
            msg!.Exibe();
        }

        protected void ApagarTask()
        {
            MyTasks!.RemoveTarefa(id);
            msg!.Oculta();
        }

        protected void SetarTarefa(Guid id, bool feito)
        {
            MyTasks!.SetTaskDone(id, feito);
        }
    }
}
