using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace MSTTool.Commands
{
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
                throw new NotImplementedException("TODO_LISTARGUMENT");
            },
            targetListArg);
        }

    }
}