namespace Todo.Core.Repository;

using System.Threading.Tasks;
using Todo.Core.Model;

public interface ITodoListRepository
{
    Task AddAsync(TodoList list);
    Task<IEnumerable<TodoList>> GetAllAsync();

    /// <summary>
    /// Finds a list by name.
    /// </summary>
    /// <param name="name">Name of the list.</param>
    /// <returns>A <see cref="TodoList"/> object including all its items, or <see langword="null"/> if no list was found under the given name.</returns>
    Task<TodoList?> GetByNameAsync(string name);
    Task DeleteAsync(TodoList list);
}