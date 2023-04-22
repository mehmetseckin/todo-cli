using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Core.Model
{
    public class TodoItemKey 
    {
        public string title { get; }
        public DateTime createdDateTime { get; }

        public TodoItemKey(TodoItem item)
        {
            title = item.title;
            createdDateTime = item.createdDateTime;
        }

        public override int GetHashCode()
        {
            unchecked 
            {
                int hashcode = 17;
                hashcode += title.GetHashCode();
                hashcode += createdDateTime.GetHashCode();
                return hashcode;
            }
        }
    }
}
