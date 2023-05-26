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
            CarregarPagina();
            msg!.Oculta();
        }

        protected void SetarTarefa(Guid id, bool feito)
        {
            MyTasks!.SetTaskDone(id, feito);
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
