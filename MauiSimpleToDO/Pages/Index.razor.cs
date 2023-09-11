using Microsoft.AspNetCore.Components;
using MauiSimpleToDO.Entities;
using MauiSimpleToDO.Services;
using MauiSimpleToDO.Shared;
using Blazored.Toast.Services;

namespace MauiSimpleToDO.Pages
{
    public class IndexBase : ComponentBase
    {
        [CascadingParameter]
        protected UITheming Theme { get; set; }
        [Inject] private ITarefas MyTasks { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        public List<Tarefa> Lista { get; set; }

        protected Msg msg;

        protected int QuantidadeTotalPaginas { get; set; }
        protected int PaginaAtual { get; set; } = 1;

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
            ToastService.ShowSuccess("Tarefa removida com sucesso!");
        }

        protected void SetarTarefa(Guid id, bool feito)
        {
            MyTasks!.SetTaskDone(id, feito);
            if (feito)
            {
                ToastService.ShowSuccess("Tarefa concluída com sucesso!");
            }
            else
            {
                ToastService.ShowInfo("Esta tarefa precisa ser concluída! Atenção!!!");
            }
        }

        protected void PaginaSelecionada(int pagina)
        {
            PaginaAtual = pagina;
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
                    ToastService.ShowWarning("Não há tarefas para o termo buscado.");
                }
            }
        }

        protected void Pesquisar(string Termo)
        {
            if (!string.IsNullOrEmpty(Termo))
            {
                PaginaAtual = 1;
                CarregarPagina(PaginaAtual, Termo);
            }
        }

        protected void LimparPesquisa()
        {
            PaginaAtual = 1;
            CarregarPagina();
        }
    }
}
