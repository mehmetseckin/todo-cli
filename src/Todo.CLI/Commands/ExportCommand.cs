using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using Todo.CLI.Handlers;

namespace MSTTool.Commands
{
    public class ExportCommand : Command
    {
        public ExportCommand(IServiceProvider serviceProvider) : base("export")
        {
            Description = "Exports ToDo items to JSON files";

            /*fnord - specific list, all
            AddOption(GetAllOption());
            AddOption(GetNoStatusOption());
            */

            //Handler = ListCommandHandler.Create(serviceProvider);
        }

        /*fnord
        private Option GetAllOption()
        {
            return new Option(new string[] { "-a", "--all" }, "Lists all to do items including the completed ones.");
        }

        private Option GetNoStatusOption()
        {
            return new Option(new string[] { "--no-status" }, "Suppresses the bullet indicating whether the item is completed or not.");
        }
        */

    }
}
