using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.CLI
{
    public class TodoCliConfiguration
    {
        public string ClientId { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public bool SupportsWrite { get; set; }
    }
}
