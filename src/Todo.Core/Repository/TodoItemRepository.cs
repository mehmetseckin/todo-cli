using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Todo.Core.Interfaces;
using Todo.Core.Model;
using static Microsoft.Graph.Constants;

namespace Todo.Core.Repository
{
    public class TodoItemRepository 
    {
        public IGraphClient Client { get; }

        public List<TodoList> Lists { get; private set; }

        public TodoItemRepository(IServiceProvider serviceProvider)
        {
            Client = serviceProvider.GetService<IGraphClient>();
        }

        private class ListTasksResponse
        {
            [JsonPropertyName("@odata.nextLink")]
            public string NextLink { get; set; }

            public List<TodoItem> value { get; set; }
        }

        // TODO: support just taking id
        // TODO: store in Repo
        public async IAsyncEnumerable<TodoItem> GetListTasksAsyncEnumerable(TodoList list)
        {
            var id = list.id;

            var uri = $"lists/{id}/tasks";
            while (uri != null)
            {
                var response = await Client.RequestAsync(uri);

                var responseObject = JsonSerializer.Deserialize<ListTasksResponse>(response);
                foreach (var item in responseObject.value)
                    yield return item;
                uri = responseObject.NextLink;
            }
        }


        public async Task PopulateListsAsync()
        {
            // only run once
            // ASNEEDED: support refreshing
            if (Lists != null)
                return;

            Lists ??= new();
            var uri = "lists";
            while (uri != null)
            {
                var response = await Client.RequestAsync(uri);
                // this parses the json and populates the repo
                uri = AddListsFromSerialized(response);
            }
        }

        private class ListsResponse
        {
            [JsonPropertyName("@odata.nextLink")]
            public string NextLink { get; set; }

            public List<TodoList> value { get; set; }
        }

        /// <summary>
        /// Deserialize the given json and add the given lists to the repo.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>NextLink Uri</returns>
        protected string AddListsFromSerialized(string json)
        {
            var jsonLists = JsonSerializer.Deserialize<ListsResponse>(json);
            Lists.AddRange(jsonLists.value);
            return jsonLists.NextLink;
        }
    }
}
