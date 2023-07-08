using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using SimpleToDOApp.Entities;
using SimpleToDOApp.Services;
using SimpleToDOApp.Shared;

namespace SimpleToDOApp.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] private ITarefas? MyTasks { get; set; }
        [Inject] private IToastService toastService { get; set; }
        public List<Tarefa>? Lista { get; set; }

        protected Msg? msg;

        protected int QuantidadeTotalPaginas { get; set; }
        protected int paginaAtual { get; set; } = 1;

        private Guid id;

        protected override void OnInitialized()
        {
            CarregarPagina();
        }

        protected void RemoveTarefa(Guid _id)
        {
            id = _id;
            msg!.Exibe();
        }

        protected void ApagarTask()
        {
            MyTasks!.RemoveTarefa(id);
            Lista!.RemoveAt(Lista.IndexOf(Lista!.Where(item => item.id == id).FirstOrDefault()!));
            msg!.Oculta();
            toastService.ShowSuccess("Tarefa removida com sucesso!");
        }

        protected void SetarTarefa(Guid id, bool feito)
        {
            MyTasks!.SetTaskDone(id, feito);
            if (feito)
            {
                toastService.ShowSuccess("Tarefa concluída com sucesso!");
            }
            else
            {
                toastService.ShowInfo("Esta tarefa precisa ser concluída! Atenção!!!");
            }
        }

        protected void PaginaSelecionada(int pagina)
        {
            paginaAtual = pagina;
            CarregarPagina(pagina);
        }

        protected async void CarregarPagina(int pagina = 1, string SearchTask = "")
        {
            PaginaTarefas page = await MyTasks!.GetTarefasPage(pagina, SearchTask);
            Lista = page.tarefas.ToList();
            QuantidadeTotalPaginas = page.totalPaginas;
            StateHasChanged();
            if (!string.IsNullOrEmpty(SearchTask))
            {
                if (!Lista!.Any())
                {
                    toastService.ShowWarning("Não há tarefas para o termo buscado.");
                }
            }
        }

        protected void Pesquisar(string Termo)
        {
            if (!string.IsNullOrEmpty(Termo))
            {
                paginaAtual = 1;
                CarregarPagina(paginaAtual, Termo);
            }
        }

        protected void LimparPesquisa()
        {
            paginaAtual = 1;
            CarregarPagina();
        }
    }
}
