// HIST:
// drp070823 - TECH_HANDLE_MOVED: HandleMovedItem() - if a TodoItem is "moved" to another List, recognize that, and don't call HandleDeletedItem()

// TODO:
// 1) TODO: drp070823 - should Sync only delete if run "all" lists? (HandleRemainingFilesAsync)


using AWEngine.Linq;
using AWEngine.Util;
using Azure;
using Microsoft.Graph.DeviceManagement.WindowsInformationProtectionAppLearningSummaries.Item;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Core.Model;
using Microsoft.Graph.Reports.GetPrinterArchivedPrintJobsWithPrinterIdWithStartDateTimeWithEndDateTime;

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

        protected override void LogTodoListExported(TodoList list, int tasksCount)
        {
            if (RemainingFiles.TryGetValue(list.displayName, out var listRemainingFiles))
                Console.WriteLine("Exported List: {0} [{1} tasks] [{2} Missing]", list.displayName, tasksCount, listRemainingFiles.Count);
            else
                base.LogTodoListExported(list, tasksCount);
        }

        /* TECH_HANDLE_MOVED: removed this code - now done in HandleRemainingFiles
        protected override void OnTodoListExported(TodoList list)
        {
            base.OnTodoListExported(list);

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
        */
        private async Task HandleRemainingFilesAsync()
        {
            /* TECH_HANDLE_MOVED: foreach GlobalRemainingFiles
             *     item = Deserialize(file)
             *     if FindInRepo(Name+createdDateTime)
             *        EDeleteHandling.HardDelete
             *     else
             *        apply defined DeleteHandling (e.g. MoveToDeletedFolder)
             * 
             * TODO: drp070823 - if DeleteHandling==HardDelete, then there's no need actually do this, can just HandleDeletedItem()
             */
            foreach (var (listName, filesMap) in RemainingFiles)
            {
                var list = await Repo.GetListSafeAsync(listName);

                foreach (var fi in filesMap.Values)
                {
                    TodoItem item;
                    using (var stream = fi.OpenRead())
                        item = JsonSerializer.Deserialize<TodoItem>(stream);

                    // PRE: item.OnDeserialized() was run
                    // ASNEEDED: item pair with fi
                    var key = item.Key;
                    var matches = Repo.FindTodoItems(key);
                    if (matches != null)
                    {
                        // item was (probably) "Moved", so HardDelete (unless Preview)
                        // LOW: drp070823 - mark matches as "Moved"?
                        // PRE: matches.Count>0
                        HandleMovedItem(list, fi);
                    }
                    else
                    {
                        // item was (probably) not "Moved", so apply DeleteHandling
                        // TECH: item.List is not set 
                        HandleDeletedItem(list, fi);
                    }
                }

            }
        }

        public override async Task FinishCommandAsync(string listName)
        {
            await base.FinishCommandAsync(listName);

            await HandleRemainingFilesAsync();     
        }

        protected virtual void HandleMovedItem(TodoList list, FileInfo fi)
        {
            Console.Write("Sync.Moved:{0}", fi.Name);

            // TODO: deserialize the file to get the task.title
            switch (DeleteHandling)
            {
                case EDeleteHandling.Preview:
                    Console.WriteLine(" [Preview]");
                    break;
                case EDeleteHandling.MoveToDeletedFolder:
                case EDeleteHandling.HardDelete:
                    Console.WriteLine(" [Delete]");
                    fi.Delete();
                    break;
                default:
                    throw new NotImplementedException();
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
