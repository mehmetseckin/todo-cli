using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public class TodoItemRepository : RepositoryBase, ITodoItemRepository
    {
        public GraphServiceClient GraphClient { get; }

        public TodoItemRepository(IAuthenticationProvider authenticationProvider)
            : base(authenticationProvider)
        {
            var client = Microsoft.Graph.GraphClientFactory.Create();
            
            GraphClient = new GraphServiceClient(AuthenticationProvider);
        }

        public async Task AddAsync(TodoItem item)
        {
            /*fnordwip
            var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
            await graphServiceClient.Me.Outlook.Tasks.Request().AddAsync(new OutlookTask()
            {
                Subject = item.Subject
            });
            */
        }

        public async Task CompleteAsync(TodoItem item)
        {
            /*fnordwip
            var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
            var requestUrl = graphServiceClient.Me.Outlook.Tasks.AppendSegmentToRequestUrl($"{item.Id}/complete");
            var builder = new OutlookTaskCompleteRequestBuilder(requestUrl, graphServiceClient);
            await builder.Request().PostAsync();
            */
        }

        public async Task DeleteAsync(TodoItem item)
        {
            /*fnordwip
            var graphServiceClient = new GraphServiceClient(AuthenticationProvider);
            var requestUrl = graphServiceClient.Me.Outlook.Tasks.AppendSegmentToRequestUrl($"{item.Id}");
            var builder = new OutlookTaskRequestBuilder(requestUrl, graphServiceClient);
            await builder.Request().DeleteAsync();
            */
        }

        public async Task<IEnumerable<TodoItem>> ListAsync(bool listAll)
        {
            var listAsync = ListAsyncEnumerable(listAll);

            // TODO: dpr032223 - ToListAsync() not in .NETCore 3.0
            var list = new List<TodoItem>();
            await foreach (var item in listAsync)
                list.Add(item);
            return list;
        }

        public async IAsyncEnumerable<TodoItem> ListAsyncEnumerable(bool listAll)
        {
            //fnordwip
            //Microsoft.Graph.Planner.Tasks.TasksRequestBuilder.TasksRequestBuilderGetRequestConfiguration config;

            // ASNEEDED: RequestConfiguration, CancellationToken
            var tasksResponse = await GraphClient.Planner.Tasks.GetAsync(null);
            var tasks = tasksResponse.Value;
            foreach (var task in tasks)
            {
                yield return new TodoItem()
                {
                    Id = task.Id,
                    Subject = task.Title,
                    IsCompleted = task.CompletedDateTime.HasValue
                };
            }

            /*fnordwip
            var graphServiceClient = new GraphServiceClient(AuthenticationProvider);

            var request = graphServiceClient.Me.Outlook.Tasks.Request();
            if(!listAll)
            {
                request.Filter($"status ne '{TaskStatus.Completed.ToString().ToLower()}'");
            }
            // drp032223 - only pulls 10, so need to loop on each page
            while (request != null)
            {
                var tasksPage = await request.GetAsync();
                foreach (var task in tasksPage)
                {
                    yield return new TodoItem()
                    {
                        Id = task.Id,
                        Subject = task.Subject,
                        IsCompleted = task.Status == TaskStatus.Completed
                    };
                }
                request = tasksPage.NextPageRequest;
            }
            */
        }
    }
}
