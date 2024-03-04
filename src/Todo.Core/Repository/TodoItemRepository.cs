using Microsoft.Graph;
using Todo.Core.Model;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Todo.Core.Repository;

using TaskStatus = Microsoft.Graph.Models.TaskStatus;

internal class TodoItemRepository : RepositoryBase, ITodoItemRepository
{
    public TodoItemRepository(IAuthenticationProvider authenticationProvider)
        : base(authenticationProvider)
    {
    }

    public async Task AddAsync(TodoItem item)
    {
        if (string.IsNullOrEmpty(item.ListId))
            throw new InvalidOperationException("item needs a ListId to identify the list to add it to.");

        var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
        _ = await graphServiceClient.Me.Todo.Lists[item.ListId].Tasks.PostAsync(new TodoTask()
        {
            Title = item.Subject,
            Status = item.IsCompleted
                ? TaskStatus.Completed
                : TaskStatus.NotStarted
        });
    }

    public async Task CompleteAsync(TodoItem item)
    {
        if (string.IsNullOrEmpty(item.ListId) || string.IsNullOrEmpty(item.Id))
            throw new InvalidOperationException("item needs a ListId and an Id.");

        var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
        await graphServiceClient.Me.Todo.Lists[item.ListId].Tasks[item.Id].PatchAsync(new TodoTask()
        {
            Status = TaskStatus.Completed
        });
    }

    public async Task DeleteAsync(TodoItem item)
    {
        if (string.IsNullOrEmpty(item.ListId) || string.IsNullOrEmpty(item.Id))
            throw new InvalidOperationException("item needs a ListId and an Id.");

        var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
        await graphServiceClient.Me.Todo.Lists[item.ListId].Tasks[item.Id].DeleteAsync();
    }

    public async Task<IEnumerable<TodoItem>> ListAllAsync(bool includeCompleted)
    {
        var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
        var lists = await graphServiceClient.Me.Todo.Lists.GetAsync();
        if (lists?.Value is null) return [];
        var tasks = lists.Value
            .AsParallel()
            .Select(list => (list, tasks: graphServiceClient.Me.Todo.Lists[list.Id].Tasks.GetAsync()))
            .Select((input, _) => (input.list, tasks: input.tasks.GetAwaiter().GetResult()?.Value))
            .Where(l => l.tasks is not null)
            .SelectMany(l => l.tasks!.Select(t => (l.list, task: t)));

        if (!includeCompleted)
        {
            tasks = tasks.Where(t => t.task.Status is not TaskStatus.Completed);
        }

        return tasks.Select(input => input.task.ToModel(input.list.Id));
    }

    public async IAsyncEnumerable<TodoItem> EnumerateAllAsync(bool includeCompleted)
    {
        var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
        var lists = await graphServiceClient.Me.Todo.Lists.GetAsync();
        if (lists?.Value is null) yield break;
        var tasks = lists.Value
            .Select(l => Task.Run(async () =>
                (ListId: l.Id, Tasks: await graphServiceClient.Me.Todo.Lists[l.Id].Tasks.GetAsync())))
            .ToList();

        while (tasks.Count > 0)
        {
            var task = await Task.WhenAny(tasks);
            tasks.Remove(task);
            var (listId, taskResponse) = await task;
            IEnumerable<TodoTask> items = taskResponse!.Value!;
            if (includeCompleted)
                items = items.Where(t => t.Status is not TaskStatus.Completed);
            foreach (var todoTask in items)
            {
                yield return todoTask.ToModel(listId);
            }
        }
    }

    public async Task<IEnumerable<TodoItem>> ListByListIdAsync(string listId, bool includeCompleted)
    {
        ArgumentException.ThrowIfNullOrEmpty(listId);

        var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
        var todoTaskCollectionResponse = (await graphServiceClient.Me.Todo.Lists[listId].Tasks.GetAsync());
        IEnumerable<TodoTask>? tasks = todoTaskCollectionResponse?.Value;
        if (tasks is null)
            return new List<TodoItem>(0);

        // if there are more pages, get them
        while (todoTaskCollectionResponse?.OdataNextLink != null)
        {
            todoTaskCollectionResponse = await graphServiceClient.Me.Todo.Lists[listId].Tasks.WithUrl(todoTaskCollectionResponse.OdataNextLink).GetAsync();
            tasks = tasks.Concat(todoTaskCollectionResponse?.Value!);
        }

        if (!includeCompleted)
        {
            tasks = tasks.Where(t => t.Status is not TaskStatus.Completed);
        }

        return tasks.Select(t => t.ToModel(listId));
    }

    public async Task<IEnumerable<TodoItem>> ListByListNameAsync(string listName, bool includeCompleted)
    {
        ArgumentException.ThrowIfNullOrEmpty(listName);

        var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
        var lists = await graphServiceClient.Me.Todo.Lists.GetAsync();
        var list = lists?.Value?.FirstOrDefault(l => l.DisplayName == listName);
        return list is null ? new List<TodoItem>(0) : await ListByListIdAsync(list.Id!, includeCompleted);
    }
}