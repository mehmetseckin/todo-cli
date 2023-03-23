
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Model;

namespace Todo.Core
{
    public interface ITodoItemRepository
    {
        Task AddAsync(TodoItem item);
        IAsyncEnumerable<TodoItem> ListAsyncEnumerable(bool listAll);
        Task<IEnumerable<TodoItem>> ListAsync(bool listAll);
        Task CompleteAsync(TodoItem item);
        Task DeleteAsync(TodoItem item);
    }
}
