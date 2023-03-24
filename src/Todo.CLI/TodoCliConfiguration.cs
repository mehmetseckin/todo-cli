using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Todo.CLI
{
    public class TodoCliConfiguration
    {
        public string ClientId { get; set; }

        public IEnumerable<string> Scopes { get; set; }

        public bool SupportsWrite { get; set; }

        [DefaultValue("https://graph.microsoft.com/v1.0/me/tasks")]
        public string BaseUri { get; set; }
    }
}
