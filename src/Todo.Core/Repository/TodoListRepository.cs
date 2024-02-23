namespace Todo.Core.Repository;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Model;

internal class TodoListRepository : RepositoryBase, ITodoListRepository
{
    public TodoListRepository(IAuthenticationProvider authenticationProvider) : base(authenticationProvider)
    {
    }

    public async Task AddAsync(TodoList list)
    {
        var client = new GraphServiceClient(AuthenticationProvider);
        await client.Me.Todo.Lists.PostAsync(new TodoTaskList
        {
            DisplayName = list.Name,
            Tasks = list.Tasks?.Select(t => new TodoTask
            {
                Title = t.Subject
            }).ToList()
        });
    }

    public async Task<IEnumerable<TodoList>> GetAllAsync()
    {
        var client = new GraphServiceClient(AuthenticationProvider);
        var lists = await client.Me.Todo.Lists.GetAsync();
        return lists?.Value?.Select(list => new TodoList
        {
            Id = list.Id,
            Name = list.DisplayName,
            Tasks = list.Tasks?.Select(t => new TodoItem
            {
                Id = t.Id,
                Subject = t.Title,
                IsCompleted = t.Status == Microsoft.Graph.Models.TaskStatus.Completed,
                ListId = list.Id,
                Completed = t.CompletedDateTime?.ToDateTime(),
                Status = t.Status?.ToString() ?? "Unknown"
            }).ToList() ?? new()
        }) ?? Array.Empty<TodoList>();
    }

    public async Task<TodoList?> GetByNameAsync(string name)
    {
        if(string.IsNullOrEmpty(name))
            throw new InvalidOperationException("name is required to get a list by name.");

        var client = new GraphServiceClient(AuthenticationProvider);
        var lists = await client.Me.Todo.Lists.GetAsync();
        var list = lists?.Value?.FirstOrDefault(l => l.DisplayName == name);
        return list is null ? null : new TodoList
        {
            Id = list.Id,
            Name = list.DisplayName,
            Tasks = list.Tasks?.Select(t => new TodoItem
            {
                Id = t.Id,
                Subject = t.Title,
                IsCompleted = t.Status == Microsoft.Graph.Models.TaskStatus.Completed,
                ListId = list.Id,
                Completed = t.CompletedDateTime?.ToDateTime(),
                Status = t.Status?.ToString() ?? "Unknown"
            }).ToList() ?? new()
        };
    }

    public async Task DeleteAsync(TodoList list)
    {
        if (string.IsNullOrEmpty(list.Id))
            throw new InvalidOperationException("list needs an Id to be deleted.");

        var client = new GraphServiceClient(AuthenticationProvider);
        await client.Me.Todo.Lists[list.Id].DeleteAsync();
    }
}
