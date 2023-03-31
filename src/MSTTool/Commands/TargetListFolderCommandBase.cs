using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Model;
using Todo.Core.Repository;

namespace Todo.MSTTool.Commands
{
    public abstract class TargetListFolderCommandBase : TargetListCommandBase
    {

        static TargetListFolderCommandBase()
        {
        }

        public TargetListFolderCommandBase(string commandName, IServiceProvider serviceProvider) : base(commandName, serviceProvider)
        {
            AddOption(TodoRootCommand.FolderOption);

            this.SetHandler<string, string>((listName, folder) => RunFolderCommandAsync(listName, folder ?? Config.TargetFolder),
                ListArgument,
                TodoRootCommand.FolderOption);
        }

        public virtual async Task RunFolderCommandAsync(string listName, string targetFolder)
        {
            Console.WriteLine("RunFolderCommand:{0} TargetList:{1} TargetFolder:", Name, listName, targetFolder);

            await base.RunCommandAsync(listName);

        }

        //public abstract Task RunCommandAsync(TodoList list, string targetFolder);
    }
}
