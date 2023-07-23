using MauiSimpleToDO.Entities;
using Microsoft.AspNetCore.Components;

namespace MauiSimpleToDO.Shared
{
    public class FormularioBase : ComponentBase
    {
        [CascadingParameter]
        protected UITheming? Theme { get; set; }
        [Parameter] public Tarefa? TaskModel { get; set; }
        [Parameter] public EventCallback OnValidSubmit { get; set; }
        [Inject] protected NavigationManager? nav { get; set; }
        protected int QtdCaracteres { get; set; }

        protected override void OnInitialized()
        {
            QtdCaracteres = TaskModel!.descricao.Length;
        }

        protected void Alterando(ChangeEventArgs e)
        {
            QtdCaracteres = e.Value!.ToString()!.Length;
        }
    }
}
