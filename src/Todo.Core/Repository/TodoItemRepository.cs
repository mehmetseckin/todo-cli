using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Todo.Core.Interfaces;
using Todo.Core.Model;

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

            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

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
                    todoItem.OriginalSerialized = jsonTask.ToJsonString(serializerOptions);
                    yield return todoItem;
                }

                uri = jsonObject["@odata.nextLink"]?.ToString();
            }
        }

        /// <summary>
        /// Return the TodoList with the given displayName.
        /// </summary>
        /// <param name="name">the displayName to match</param>
        /// <returns>the TodoList</returns>
        /// <remarks>TODO_LISTARGUMENT: add Dictionary cache</remarks>
        public async Task<TodoList> GetListSafeAsync(string name)
        {
            var lists = await PopulateListsAsync();

            var list = lists.FirstOrDefault(l => l.displayName == name);
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
            // BUG_LISTS: drp060923 - using "lists/delta" as workaround since "lists" fails to return all
            var uri = "lists/delta";
            while (uri != null)
            {
                var response = await Client.RequestAsync(uri);
                // this parses the json and populates the repo
                uri = AddListsFromSerialized(response);
            }

            Console.WriteLine("Lists Retrieved: {0}", Lists.Count);

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

        #region Exported

        private TodoItemMap _exportedMap = new TodoItemMap();

        // RETURN: may be null
        public List<TodoItem> FindTodoItems(TodoItemKey key)
        {
            return _exportedMap.GetTodoItemsSafe(key);
        }

        public void OnTodoItemExported(TodoList list, TodoItem item, FileInfo fi)
        {
            // TODO: drp070823 - may not need
            item.OnTodoItemExported(list, fi);
            _exportedMap.AddTodoItem(item);
        }

        #endregion
    }
}
