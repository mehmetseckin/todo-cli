using System;
using System.CommandLine;
using Todo.CLI.Handlers;
using Todo.Core;

namespace Todo.CLI.Commands
{
    public class ListCommand : Command
    {
        public ListCommand(IServiceProvider serviceProvider) : base("list")
        {
            Description = "Retrieves a list of the to do items.";

            Handler = ListCommandHandler.Create(serviceProvider);
        }
    }
}