﻿using MauiSimpleToDO.Entities;

namespace MauiSimpleToDO.Services
{
    public interface ITarefas
    {
        public Task<List<Tarefa>> GetTarefas();
        public Tarefa GetTarefa(Guid _id);
        public void AddTarefa(Tarefa _tarefa);
        public bool RemoveTarefa(Guid _tarefaId);
        public void UpdateTarefa(Tarefa _tarefa);
        public void SetTaskDone(Guid _id, bool Done);
        public Task<PaginaTarefas> GetTarefasPage(int pageIndex = 1, string SearchTask = "");
    }
}
