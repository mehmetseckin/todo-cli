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
        Task<IEnumerable<TodoItem>> ListAsync();
    }
}
