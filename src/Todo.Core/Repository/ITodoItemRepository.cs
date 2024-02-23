namespace Todo.Core.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Core.Model;

public interface ITodoItemRepository
{
    Task AddAsync(TodoItem item);
    Task<IEnumerable<TodoItem>> ListAllAsync(bool includeCompleted);
    IAsyncEnumerable<TodoItem> EnumerateAllAsync(bool includeCompleted);
    Task<IEnumerable<TodoItem>> ListByListIdAsync(string listId, bool includeCompleted);
    Task<IEnumerable<TodoItem>> ListByListNameAsync(string listName, bool includeCompleted);
    Task CompleteAsync(TodoItem item);
    Task DeleteAsync(TodoItem item);
}