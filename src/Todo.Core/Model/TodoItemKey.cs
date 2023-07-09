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

        public override bool Equals(object obj)
        {
            if (obj is not TodoItemKey other)
                return false;
            return title == other.title
                && createdDateTime == other.createdDateTime;
        }
    }

    // HACK_KEY2
    public class TodoItemKey2
    {
        public DateTime createdDateTime { get; }

        public TodoItemKey2(TodoItem item)
        {
            createdDateTime = item.createdDateTime;
        }

        public TodoItemKey2(TodoItemKey key1)
        {
            createdDateTime = key1.createdDateTime;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = 17;
                hashcode += createdDateTime.GetHashCode();
                return hashcode;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not TodoItemKey other)
                return false;
            return createdDateTime == other.createdDateTime;
        }
    }
}
