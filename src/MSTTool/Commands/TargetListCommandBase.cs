// TODO:
// 1) TODO_EXCLUDECOMPLETED: drp033123 - add Option<bool> to exclude Completed tasks

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Todo.MSTTool;
using Todo.Core.Model;
using Todo.Core.Repository;
using System.Linq;

namespace Todo.MSTTool.Commands
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

        public TodoItemRepository Repo { get; set; }

        public MSTConfiguration Config { get; set; }

        public TargetListCommandBase(string commandName, IServiceProvider serviceProvider) : base(commandName)
        {
            Repo = serviceProvider.GetService<TodoItemRepository>();

            Config = serviceProvider.GetService<MSTConfiguration>();

            // TODO: drp033123 - cleaner to AddGlobalArgument() to RootCommand?
            this.AddArgument(ListArgument);

            this.SetHandler<string>((listName) => RunCommandAsync(listName),
                ListArgument);
        }

        public virtual async Task RunCommandAsync(string listName)
        {
            Console.WriteLine("RunCommand:{0} TargetList:{1}", Name, listName ?? "(all)");

            if (listName != null)
            {
                var list = await Repo.GetListSafeAsync(listName);
                if (list == null)
                {
                    // TODO_ERRORHANDLING: right now InvokeAsync will report as Unhandled Exception. Need prettier display
                    throw new KeyNotFoundException("Unrecognized List: " + listName);
                }
                await RunCommandAsync(list);
            }
            else // export all
            {
                var lists = await Repo.PopulateListsAsync();

                // OPTIMIZE: drp033023 - parallelize
                foreach (var list in Repo.Lists)
                    await RunCommandAsync(list);

            }

            await FinishCommandAsync(listName);
        }

        public abstract Task RunCommandAsync(TodoList list);

        public virtual Task FinishCommandAsync(string listName)
        {
            return Task.CompletedTask;
        }
    }
}
