using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Model;
using Todo.Core.Repository;

namespace Todo.MSTTool.Commands
{
    public abstract class TargetListFolderCommandBase : TargetListCommandBase
    {
        public DirectoryInfo ExportRoot { get; private set; }

        // USAGE: derived classes set this to true if they want the TargetFolder to be created if it does not exist.
        public bool CreateExportRoot { get; protected set; }


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

        // POST: ExportRoot is set; if failed, then logs out
        protected bool TrySetupExportRoot(string targetFolder)
        {
            Console.WriteLine("TargetFolder: {0}", Path.GetFullPath(targetFolder));

            ExportRoot = new DirectoryInfo(targetFolder);
            if (ExportRoot.Exists)
            {
                Console.WriteLine("TargetFolder found: {0}", ExportRoot);
                return true;
            }

            if (!CreateExportRoot)
            {
                Console.WriteLine("ERROR: TargetFolder not found: {0}", ExportRoot);
                return false;
            }

            try
            {
                Console.WriteLine("Creating TargetFolder: {0}", ExportRoot);
                ExportRoot.Create();
                Console.WriteLine("Created TargetFolder: {0}", ExportRoot);
                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine("ERROR: TargetFolder creation failed: {0}", ex.Message);
                return false;
            }
        }

        // listName: null means all
        public virtual async Task RunFolderCommandAsync(string listName, string targetFolder)
        {
            // drp070823 - allow "all" to work as well
            if (listName == Config.TargetListAllAlias)
                listName = null;

            Console.WriteLine("RunFolderCommand:{0} TargetList:{1} TargetFolder:{2}", Name, listName ?? "(all)", targetFolder);

            // check for conflict with reserved name
            // TODO: drp040323 - rename conflicting this to a non-clashing name. Better solutions:
            // B) treat "__" prefix as reserved and always store lists with that prefix in another folder
            // C) store all exported tasks one level down in "{TargetFolder}/Tasks/". 
            if (listName == Config.DeletedSubFolder)
                throw new NotImplementedException("Reserved name handling not implemented");

            // CODEP: if fails, then assume it already emit the error
            if (!TrySetupExportRoot(targetFolder))
                return;

            await base.RunCommandAsync(listName);

        }

        //public abstract Task RunCommandAsync(TodoList list, string targetFolder);
    }
}
