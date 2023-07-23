# BlazorSamples

![Linguagem](https://img.shields.io/badge/Linguagem-C%20Sharp-purple)
![Framework](https://img.shields.io/badge/Framework-Blazor-purple)
![Status](https://img.shields.io/badge/Status-In%20Progress-brightgreen)
 
 Repositório reservado para amostras/experimentos com a Framework Blazor e C# para aprimoramento profissional, colocando conhecimentos aprendidos no curso Blazor.Essential do José Carlos Macoratti

 # Simple To-Do App / Maui Simple To-Do App

Utilizando praticamente o mesmo código, o primeiro aplicativo fruto dos experimentos para colocar os conhecimentos de Blazor em prática é um gerenciador bem simples de tarefas onde é possível criar nova tarefa, editar uma tarefa cadastrada e apagar também. As tarefas podem ser marcadas como concluídas e são listadas com recurso de paginação, sendo exibidas 5 tarefas por página, filtragem baseada tanto no nome da tarefa (coluna tarefa) como na sua descrição (coluna descrição). Os dados são salvos localmente usando o LocalStorage. O Simple To-Do App é um Blazor WebAssembly que roda no browser e o Maui Simple To-Do app é uma versão do mesmo aplicativo que roda em múltiplas plataformas, tendo já sido compilado e testado no Windows e no Android. Um detalhe que gostei muito foi a pouquíssima alteração realizada para obter um aplicativo da web rodando em diversas plataformas de forma nativa. Pretendo evoluir esses dois projetos, adicionando tema escuro e mais responsividade e mensagens toast.

## Recursos Blazor aprendidos e usados:
- [X] Componentização
- [X] Interoperabilidade com JavaScript
- [X] Paginação e filtragem de dados
- [X] Manipulação de listas
- [X] Responsividade (com bootstrap)
- [X] Delegates
- [X] RenderFragment
- [X] Parameters
- [X] Programação Assíncrona
- [X] Code-behind
- [X] Classes
- [X] DataBinding
- [X] Laços de repetição e condicionais
- [X] Diretivas
- [X] EventCallback
- [X] Ciclo de vida dos componentes
- [X] Injeção de Dependência
- [X] Layouts
- [X] Referência de Componentes (@Ref)
- [X] Roteamento e navegação
- [X] Formulários
- [X] NavigationManager
- [X] Validação de formulários
- [X] Modo Claro e Escuro
- [X] Implementado gravação e leitura do tema diretamente do LocalStorage
- [X] Mensagens Toast
- [X] Aplicar alterações de Modo Claro/Escuro ao Maui Simple to-do app


## Imagem da tela principal do projeto Simple To-do App (aplicativo para gerenciamento de tarefas):

 ![Simple To-do App](https://github.com/rafael-figueiredo-alves/BlazorSamples/blob/main/images/Tela_Inicial_SimpleToDoApp.jpeg)
 ![Maui Simple To-do App - Windows - light mode](https://github.com/rafael-figueiredo-alves/BlazorSamples/blob/main/images/Windows-light.png)
 ![Maui Simple To-do App - Windows - dark mode](https://github.com/rafael-figueiredo-alves/BlazorSamples/blob/main/images/Windows-dark.png)
 ![Maui Simple To-do App - Android - light mode](https://github.com/rafael-figueiredo-alves/BlazorSamples/blob/main/images/Mobile-light.png)
 ![Maui Simple To-do App - Android - dark mode](https://github.com/rafael-figueiredo-alves/BlazorSamples/blob/main/images/Mobile-dark.png)

 # WebApi - Clientes

 Criei uma webapi usando o C# e Asp.Net Core, usando MySQL com direito a autenticação, autorização e endpoints para CRUD de dados simples de Clientes que é usado no BlazorClientes. Em construção...

 # BlazorClientes

 Exemplo de sistema com login, logout, criação de contas, e CRUD de clientes. Em construção...