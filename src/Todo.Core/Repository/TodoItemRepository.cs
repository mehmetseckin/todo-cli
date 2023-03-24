using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Todo.Core.Model;

namespace Todo.Core.Repository
{
    public class TodoItemRepository 
    {
        public List<TodoList> Lists { get; private set; }

        public TodoItemRepository(IServiceProvider serviceProvider)
        {
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
        public string AddLists(string json)
        {
            Lists ??= new();
            var jsonLists = JsonSerializer.Deserialize<ListsResponse>(json);
            Lists.AddRange(jsonLists.value);
            return jsonLists.NextLink;
        }
    }
}
