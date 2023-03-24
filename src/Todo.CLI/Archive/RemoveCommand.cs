using System;
using System.CommandLine;
using Todo.CLI.Handlers;
using Todo.Core;
using Todo.Core.Model;

namespace MSTTool.Archive
{
    public class RemoveCommand : Command
    {
        public RemoveCommand(IServiceProvider serviceProvider) : base("remove")
        {
            Description = "Deletes a to do item.";
            Handler = RemoveCommandHandler.Create(serviceProvider);
        }
    }
}