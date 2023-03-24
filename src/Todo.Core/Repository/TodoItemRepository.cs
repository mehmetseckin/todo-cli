using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Todo.Core.Model;
//fnordwip
//using TaskStatus = Microsoft.Graph.TaskStatus;

namespace Todo.Core.Repository
{
    /*
    public class TodoItemRepository2 : ITodoItemRepository
    {
        public GraphServiceClient GraphClient { get; }

        public TodoItemRepository(Task<AuthenticationResult> authenticationProvider)
            : base(authenticationProvider)
        {
            new Microsoft.Graph.GraphServiceClient()
            GraphClient = new GraphServiceClient(AuthenticationProvider);
            new GraphClient
        }
    }
    */

    public class TodoItemRepository 
    {
        public List<TodoList> Lists { get; private set; }

        public TodoItemRepository(IServiceProvider serviceProvider)
        {
        }

        private class ListsResponse
        {
            public List<TodoList> value { get; set; }
        }

        // fnord need to combine command and repo. Need to follow nextLink
        public void AddLists(string json)
        {
            Lists ??= new();
            var jsonLists = JsonSerializer.Deserialize<ListsResponse>(json);
        }
    }
}
