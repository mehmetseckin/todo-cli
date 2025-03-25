using System.Collections.Generic;

namespace Todo.CLI;

public class TodoCliConfiguration
{
    public string ClientId { get; set; } = "63323d6c-7f31-4fa2-bca8-eec656888e97";
    public IEnumerable<string> Scopes { get; set; } = new[] 
    { 
        "user.read",
        "tasks.readwrite"
    };
}