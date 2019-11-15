using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Core.Model
{
    public class TodoItem
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public override string ToString() => Subject;
    }
}
