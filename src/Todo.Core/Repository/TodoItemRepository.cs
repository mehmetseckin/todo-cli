using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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

                // TECH: access each serialized task individually, so we don't lose anything going Deserialize => Serialize
                // OPTIMIZE: we could change these to be Deserialized on demand
                var jsonObject = JsonSerializer.Deserialize<JsonObject>(response);
                var jsonTasksArray = jsonObject["value"].AsArray();
                foreach (var jsonTask in jsonTasksArray)
                {
                    var todoItem = JsonSerializer.Deserialize<TodoItem>(jsonTask);
                    todoItem.OriginalSerialized = jsonTask.ToJsonString();
                    yield return todoItem;
                }

                uri = jsonObject["@odata.nextLink"]?.ToString();
            }
        }

        // TODO_LISTARGUMENT: add Dictionary
        public async Task<TodoList> GetListAsync(string name)
        {
            var lists = await PopulateListsAsync();
            // TODO_ERRORHANDLING: right now InvokeAsync will report as Unhandled Exception. Need prettier display
            var list = lists.FirstOrDefault(l => l.displayName == name);
            if (list == null)
                throw new KeyNotFoundException("Unrecognized List: " + name);
            return list;
        }

        public async Task<IEnumerable<TodoList>> PopulateListsAsync()
        {
            // only run once
            // ASNEEDED: support refreshing
            // TODO_LISTARGUMENT: support awaiting a pending PopulateLists task. Use IObservable cold stream?
            if (Lists != null)
                return Lists;

            Lists ??= new();
            var uri = "lists";
            while (uri != null)
            {
                var response = await Client.RequestAsync(uri);
                // this parses the json and populates the repo
                uri = AddListsFromSerialized(response);
            }

            return Lists;
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
