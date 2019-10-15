using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using Todo.Core.Model;

namespace Todo.Core.Repository
{
    public class TodoItemRepository : RepositoryBase, ITodoItemRepository
    {
        public TodoItemRepository(IAuthenticationProvider authenticationProvider)
            : base(authenticationProvider)
        {
        }

        public async Task<IEnumerable<TodoItem>> ListAsync()
        {
            var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
            var tasks = await graphServiceClient.Me.Outlook.Tasks.Request().GetAsync();
            return tasks.Select(task => new TodoItem() 
            { 
                Subject = task.Subject
            });
        }
    }
}
