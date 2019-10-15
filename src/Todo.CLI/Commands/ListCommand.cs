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

            AddOption(GetAllOption());

            Handler = ListCommandHandler.Create(serviceProvider);
        }

        private Option GetAllOption()
        {
            return new Option(new string[] { "-a", "--all" }, "Lists all to do items including the completed ones.");
        }
    }
}