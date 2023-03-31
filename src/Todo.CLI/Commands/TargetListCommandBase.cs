using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Repository;

namespace MSTTool.Commands
{
    public abstract class TargetListCommandBase : Command
    {
        static TargetListCommandBase()
        {
            ListArgument = new("list", "Specific list to target (using DisplayName)");
            ListArgument.SetDefaultValue(null);
        }

        // common argument for most commands.
        public static Argument<string> ListArgument { get; } 

        public TodoItemRepository Repo { get; }

        public TargetListCommandBase(string commandName, IServiceProvider serviceProvider) : base(commandName)
        {
            Repo = serviceProvider.GetService<TodoItemRepository>();

            this.AddArgument(ListArgument);

            this.SetHandler<string>(listName => RunCommandAsync(listName), ListArgument);
        }

        public abstract Task RunCommandAsync(string listName); 

    }
}
