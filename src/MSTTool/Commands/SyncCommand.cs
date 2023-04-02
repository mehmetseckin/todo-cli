using AWEngine.Linq;
using AWEngine.Util;
using Microsoft.Graph.DeviceManagement.WindowsInformationProtectionAppLearningSummaries.Item;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Model;

namespace Todo.MSTTool.Commands
{
    public class SyncCommand : ExportCommand
    {
        // TECH: as "export" writes out tasks, they are removed from here.
        // TECH: we would like to store FileName, but FileInfo.Equals() doesn't work, so instead we need to use the fi.Name as the key
        public Dictionary<string, Dictionary<string, FileInfo>> RemainingFiles { get; } = new();

        public SyncCommand(IServiceProvider serviceProvider) : base("sync", serviceProvider)
        {
            Description = "Exports ToDo items to JSON files, and deletes files from a previous export where the item no longer exists. Optionally takes target list name.";
        }

        public override async Task RunFolderCommandAsync(string listName, string folder)
        {
            // first, build a list of existing files by list
            UpdateExportedTaskList(listName, folder);

            await base.RunFolderCommandAsync(listName, folder);
        }

        private void UpdateExportedTaskList(string listName, string folder)
        {
            if (listName == null)
            {
                foreach (var dir in AWUtil.FindAllSubFolders(folder))
                {
                    listName = dir.Name;
                    UpdateExportedTaskList(listName, folder);
                }
            }
            else
            {
                Console.WriteLine("UpdateExportedTasks:{0}", listName);
                var listFolder = Path.Combine(folder, listName);
                var files = AWUtil.FindAllFiles(listFolder, "*.json")
                    .ToDictionary(fi => fi.Name, fi => fi);
                RemainingFiles[listName] = files;
                Console.WriteLine("UpdateExportedTasks:{0} Files:{1}", listName, files.Count);
            }
        }

        protected override void OnTodoItemExported(TodoList list, TodoItem item, FileInfo fi)
        {
            base.OnTodoItemExported(list, item, fi);

            var listName = list.displayName;
            // if list folder didn't previously exist, nothing to update
            if (RemainingFiles.TryGetValue(listName, out var listRemainingFiles))
            {
                bool existing = listRemainingFiles.Remove(fi.Name);
                /* DEBUG
                if (existing)
                    Console.WriteLine("Sync.Match:{0}", item.title); 
                else
                    Console.WriteLine("Sync.New:{0}", item.title);
                */
            }
        }

        protected override void OnTodoListExported(TodoList list)
        {
            base.OnTodoListExported(list);

            Console.WriteLine("RemoveDeletedTasks:{0}", list.displayName);
            if (RemainingFiles.TryGetValue(list.displayName, out var listRemainingFiles))
            {
                foreach (var fi in listRemainingFiles.Values)
                {
                    // TODO: deserialize the file to get the task.title
                    Console.WriteLine("Sync.Delete:{0}", fi.Name);
                    fi.Delete();
                }

                Console.WriteLine("RemoveDeletedTasks:{0} Removed:{1}", list.displayName, listRemainingFiles.Count);
            }
        }
    }
}
