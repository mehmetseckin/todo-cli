using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using Todo.CLI.Handlers;

namespace Todo.CLI.Commands
{
    public class AddCommand : Command
    {
        public AddCommand(IServiceProvider serviceProvider) : base("add")
        {
            Description = "Adds a to do item.";

            AddArgument(GetSubjectArgument());

            Handler = AddCommandHandler.Create(serviceProvider);
        }

        private Argument GetSubjectArgument()
        {
            return new Argument("subject")
            {
                Description = "The subject of the new to do item.",
                ArgumentType = typeof(string)
            };
        }
    }
}
