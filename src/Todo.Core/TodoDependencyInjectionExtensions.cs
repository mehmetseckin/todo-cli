namespace Todo.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Models;
using Model;
using Repository;

public static class TodoDependencyInjectionExtensions
{
    /// <summary>
    /// Adds repositories for lists and items to the service collection. Depends on <see cref="Microsoft.Kiota.Abstractions.Authentication.IAuthenticationProvider"/> to be present in the DI container.
    /// </summary>
    /// <param name="services">The service collection to add the repositories to.</param>
    /// <returns>The service collection itself for chaining.</returns>
    public static IServiceCollection AddTodoRepositories(this IServiceCollection services)
    {
        return services
            .AddTransient<ITodoListRepository, TodoListRepository>()
            .AddTransient<ITodoItemRepository, TodoItemRepository>();
    }

    internal static TodoItem ToModel(this Microsoft.Graph.Models.TodoTask task, string? listId)
    {
        return new TodoItem
        {
            Id = task.Id,
            Subject = task.Title,
            IsCompleted = task.Status == Microsoft.Graph.Models.TaskStatus.Completed,
            Status = task.Status?.ToString() ?? "Unknown",
            Completed = task.CompletedDateTime?.ToDateTime(),
            ListId = listId
        };
    }
}