using System.Collections.Generic;

namespace Todo.CLI;

public class TodoCliConfiguration
{
    public string ClientId { get; set; }
    public IEnumerable<string> Scopes { get; set; }
}