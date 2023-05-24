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
        //public List<Tarefa>? ListaPaginada { get; set; }

        protected Msg? msg;

        //protected int QuantidadeTotalPaginas { get; set; }
        //protected double TotalPaginas { get; set; }
        //protected int QtdTarefasPorPagina { get; set; } = 5;
        //protected int paginaAtual { get; set; } = 1;

        private Guid id;

        protected override async void OnInitialized()
        {
            Lista = await MyTasks!.GetTarefas();
            StateHasChanged();
            //TotalPaginas = Math.Ceiling((double)(Lista!.Count / QtdTarefasPorPagina));
            //QuantidadeTotalPaginas = Convert.ToInt32(TotalPaginas);
        }

        //protected override void OnAfterRender(bool firstRender)
        //{
        //    CarregarPagina();
        //    StateHasChanged();
        //}

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

        //protected void PaginaSelecionada(int pagina)
        //{
        //    paginaAtual = pagina;
        //    CarregarPagina(pagina);
        //    StateHasChanged();
        //}

        //protected void CarregarPagina(int pagina = 1)
        //{
        //    ListaPaginada = Lista!
        //        .Skip((pagina - 1) * QtdTarefasPorPagina)
        //        .Take(QtdTarefasPorPagina).ToList();
        //    StateHasChanged();
        //}
    }
}
