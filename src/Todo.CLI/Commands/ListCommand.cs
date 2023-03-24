using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Todo.CLI.Handlers;
using Todo.Core;
using Todo.Core.Model;

namespace MSTTool.Commands
{
    //fnordim - need to map name to id
    public class ListCommand : Command
    {
        public ListCommand(IServiceProvider serviceProvider) : base("list")
        {
            Description = "Retrieves a list of the ToDo items.";

            var targetListArg = new Argument<string>("listName");
            AddArgument(targetListArg);

            this.SetHandler<string>((a) =>
            {
                Console.WriteLine("List Argument:{0}", a);
                throw new NotImplementedException("TODO");
            },
            targetListArg);
        }

    }
}