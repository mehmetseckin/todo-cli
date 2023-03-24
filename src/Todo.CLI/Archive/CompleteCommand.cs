using System;
using System.CommandLine;
using Todo.CLI.Handlers;
using Todo.Core;
using Todo.Core.Model;

namespace MSTTool.Archive
{
    public class CompleteCommand : Command
    {
        public CompleteCommand(IServiceProvider serviceProvider) : base("complete")
        {
            Description = "Completes a to do item.";

            AddOption(GetItemOption());

            Handler = CompleteCommandHandler.Create(serviceProvider);
        }

        private Option GetItemOption()
        {
            return new Option(new string[] { "id", "item-id" }, "The unique identifier of the todo item to complete.");
        }
    }
}