using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Todo.MSTTool.Commands
{
    public class SyncCommand : ExportCommand
    {
        public SyncCommand(IServiceProvider serviceProvider) : base("sync", serviceProvider)
        {
            Description = "Exports ToDo items to JSON files, and deletes files from a previous export where the item no longer exists. Optionally takes target list name.";
        }

        public override Task RunFolderCommandAsync(string listName, string folder)
        {
            //return base.RunFolderCommandAsync(listName, folder);
            throw new NotImplementedException();
        }
    }
}
