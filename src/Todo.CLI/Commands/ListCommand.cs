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
            AddOption(GetNoStatusOption());

            Handler = ListCommandHandler.Create(serviceProvider);
        }

        private Option GetAllOption()
        {
            return new Option(new string[] { "-a", "--all" }, "Lists all to do items including the completed ones.");
        }

        private Option GetNoStatusOption()
        {
            return new Option(new string[] { "--no-status" }, "Suppresses the bullet indicating whether the item is completed or not.");
        }

    }
}