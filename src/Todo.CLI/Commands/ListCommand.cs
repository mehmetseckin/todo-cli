using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Todo.CLI.Commands;

namespace MSTTool.Commands
{
    public class ListCommand : TargetListCommandBase
    {
        public ListCommand(IServiceProvider serviceProvider) : base("list", serviceProvider)
        {
            Description = "Retrieves a list of the ToDo items.";
        }

        public override async Task RunCommandAsync(string listName)
        {
            throw new NotImplementedException();
        }

    }
}