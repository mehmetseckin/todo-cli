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
        private MultiMapList<TodoItemKey, TodoItem> _map = new();
        private object _mutex = new();

        public void AddTodoItem(TodoItem item)
        {
            if (item.Key == null)
                throw new InvalidOperationException();
            // TODO: is MultiMap thread safe?
            lock (_mutex)
                _map.TryToAddMapping(item.Key, item);
        }

        public List<TodoItem> GetTodoItems(TodoItemKey key)
        {
            //fnord not thread safe.
            lock (_mutex)
            {
                _map.TryGetValue(key, out var todoItems);
                return todoItems;
            }
        }

        //fnord
        public void LoadDeletedItems()
        {

        }

        public 
    }
}
