// TODO:
// 1) TODO_EXCLUDECOMPLETED: drp033123 - add Option<bool> to exclude Completed tasks

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Model;
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

        public virtual async Task RunCommandAsync(string listName)
        {
            if (listName != null)
            {
                var list = await Repo.GetListAsync(listName);
                await RunCommandAsync(list);
            }
            else // export all
            {
                var lists = await Repo.PopulateListsAsync();

                // OPTIMIZE: drp033023 - parallelize
                foreach (var list in Repo.Lists)
                    await RunCommandAsync(list);
            }
        }

        public abstract Task RunCommandAsync(TodoList list);

    }
}
