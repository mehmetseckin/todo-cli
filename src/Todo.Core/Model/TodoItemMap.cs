using DotNet;
using DotNet.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Core.Model
{
    /* TECH_TODOITEMMAP: drp042123
     * When a TodoItem is moved to a new List, it gets a new id, but retains the same "title" and "createdDateTime". We use
     * the TodoItemMap to find these matches. Things this helps fix:
     * 1) when a task is "undeleted", it gets a new id. We currently don't recognize it as being recovered.
     * 2) when a task changes Lists, it gets a new id. We currently treat that as a new task, and "delete" the old one.
     * 
     * Fixes
     * 1) when a task is "undeleted", we can recognize and remove it from "__Deleted".
     * 2) when a task changes Lists, we can recognize and not move the old to "__Deleted"
     * 
     * TODO:
     * 1) will not work if the title is edited. We could first start with a createdDateTime match
     * 2) if we run a sync with a subset of Lists, it will incorrectly still treat a List change as a Delete.
     */
    public class TodoItemMap
    {
        // TECH: is not thread safe, so we use _mutex
        private MultiMapList<TodoItemKey, TodoItem> _map = new();
        private MultiMapList<TodoItemKey2, TodoItem> _map2 = new();
        private object _mutex = new();

        public void AddTodoItem(TodoItem item)
        {
            if (item.Key == null)
                throw new InvalidOperationException();
            lock (_mutex)
            {
                _map.TryToAddMapping(item.Key, item);
                _map2.TryToAddMapping(item.Key2, item);
            }
        }

        public List<TodoItem> GetTodoItemsSafe(TodoItemKey key)
        {
            lock (_mutex)
            {
                // TECH: if not found, returns null
                _map.TryGetValue(key, out var todoItems);
                // HACK_KEY2: this is untested. If we do hit on Key2, we should log noisily.
                if (todoItems == null)
                {
                    var key2 = new TodoItemKey2(key);
                    _map2.TryGetValue(key2, out todoItems);
                    if (todoItems != null && todoItems.Count > 1)
                        return null;
                }
                return todoItems;
            }
        }

    }
}
