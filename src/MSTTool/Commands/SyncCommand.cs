using AWEngine.Linq;
using AWEngine.Util;
using Microsoft.Graph.DeviceManagement.WindowsInformationProtectionAppLearningSummaries.Item;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Model;

namespace Todo.MSTTool.Commands
{
    public class SyncCommand : ExportCommand
    {
        public enum EDeleteHandling
        {
            // specify "-preview"
            Preview,
            // default behavior
            MoveToDeletedFolder,
            // ASNEEDED: drp040323 - option to set this behavior. E.g. "-delete" or "-force"
            HardDelete
        }

        public static Option<bool> PreviewOption = new ("-preview", "Don't actually delete files.");

        // map from listName
        // TECH: as "export" writes out tasks, they are removed from here.
        // TECH: we would like to store FileName, but FileInfo.Equals() doesn't work, so instead we need to use the fi.Name as the key
        public Dictionary<string, Dictionary<string, FileInfo>> RemainingFiles { get; } = new();

        public EDeleteHandling DeleteHandling { get; set; } = EDeleteHandling.MoveToDeletedFolder;

        public bool IsPreviewMode => DeleteHandling == EDeleteHandling.Preview;

        public DirectoryInfo DeletedStagingFolder { get; private set; }

        public SyncCommand(IServiceProvider serviceProvider) : base("sync", serviceProvider)
        {
            Description = "Exports ToDo items to JSON files, and deletes files from a previous export where the item no longer exists. Optionally takes target list name.";

            AddOption(PreviewOption);

            // HACK_OPTIONS+TODO: drp040223 - need InvocationContext to be passed to our base class handler so we can pull out arbitrary parameters in derived classes.
            //  Meantime, we'll just store the parameter in the command class.
            this.SetHandler((l, f, p) => RunSyncCommandAsync(l, f ?? Config.TargetFolder, p),
                ListArgument,
                TodoRootCommand.FolderOption,
                PreviewOption);

            /* WIP
            Handler = new AnonymousCommandHandler(
            context =>
            {
                var value1 = GetValueForHandlerParameter(symbol1, context);
                var value2 = GetValueForHandlerParameter(symbol2, context);

                return handle(value1!, value2!);
            });
            */
        }

        // HACK_OPTIONS: modifying object state for both isPreview and targetFolder
        public virtual Task RunSyncCommandAsync(string listName, string targetFolder, bool isPreview)
        {
            if (isPreview)
                DeleteHandling = EDeleteHandling.Preview;
            if (IsPreviewMode)
                Console.WriteLine("Sync.DeleteHandling = {0}", DeleteHandling);

            DeletedStagingFolder = new DirectoryInfo(Path.Combine(targetFolder, Config.DeletedSubFolder));

            return RunFolderCommandAsync(listName, targetFolder);
        }


        public override Task RunCommandAsync(TodoList list)
        {
            UpdateExportedTaskList(list);

            return base.RunCommandAsync(list);
        }

        private void UpdateExportedTaskList(TodoList list)
        {
            var listName = list.displayName;
            //Console.WriteLine("UpdateExportedTasks:{0}", listName);
            var listFolder = Path.Combine(ExportRoot.FullName, list.displayName);
            var files = AWUtil.FindAllFiles(listFolder, "*.json")
                .ToDictionary(fi => fi.Name, fi => fi);
            RemainingFiles[listName] = files;
            if (files.Count > 0)
                Console.WriteLine("PreviouslyExportedTasks:{0} Files:{1}", listName, files.Count);
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

            // fnord don't delete now?
            //Console.WriteLine("RemoveDeletedTasks:{0}", list.displayName);
            if (RemainingFiles.TryGetValue(list.displayName, out var listRemainingFiles))
            {
                try
                {
                    foreach (var fi in listRemainingFiles.Values)
                        HandleDeletedItem(list, fi);
                    if (listRemainingFiles.Count > 0)
                        Console.WriteLine("RemovedDeletedTasks:{0} Removed:{1}", list.displayName, listRemainingFiles.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: {0}", ex.ToString());
                }
            }
        }

        protected virtual void HandleDeletedItem(TodoList list, FileInfo fi)
        {
            Console.Write("Sync.Delete:{0}", fi.Name);

            // TODO: deserialize the file to get the task.title
            switch (DeleteHandling)
            {
                case EDeleteHandling.Preview:
                    Console.WriteLine(" [Preview]");
                    break;
                case EDeleteHandling.MoveToDeletedFolder:
                    var deletedListFolder = new DirectoryInfo(Path.Combine(DeletedStagingFolder.FullName, list.displayName));
                    Console.WriteLine(" [{0}]", deletedListFolder);

                    // CODEP: FileInfo.MoveTo() requires the folder to exist
                    if (!deletedListFolder.Exists)
                        deletedListFolder.Create();

                    // TODO: drp040323 - if file already exists, either we can specify the overwrite flag, or  we create a unique filename
                    var deletedItemFileName = Path.Combine(deletedListFolder.FullName, fi.Name);
                    fi.MoveTo(deletedItemFileName, overwrite: true);
                    break;
                case EDeleteHandling.HardDelete:
                    Console.WriteLine(" [Delete]");
                    fi.Delete();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
