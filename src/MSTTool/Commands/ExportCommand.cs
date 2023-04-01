using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Models;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Core.Model;
using Todo.Core.Repository;
using Todo.Core.Util;

namespace Todo.MSTTool.Commands
{
    /// <summary>
    /// Example usage "todo export" or "todo export LIST" or "todo export LIST -folder FOLDER".
    /// </summary>
    public class ExportCommand : TargetListFolderCommandBase
    {
        public ExportCommand(IServiceProvider serviceProvider) : this("export", serviceProvider)
        {
            Description = "Exports ToDo items to JSON files. Optionally takes target list name.";
        }

        protected ExportCommand(string commandName, IServiceProvider serviceProvider) : base(commandName, serviceProvider)
        {
            CreateExportRoot = true;
        }

        public override Task RunFolderCommandAsync(string listName, string folder)
        {
            return base.RunFolderCommandAsync(listName, folder);
        }

        public override async Task RunCommandAsync(TodoList list)
        {
            Console.WriteLine("Export List: {0}", list.displayName);

            var listFolderName = TodoUtil.NormalizeFileName(list.displayName);
            var subdir = ExportRoot.CreateSubdirectory(listFolderName);
            var tasksAsync = Repo.GetListTasksAsyncEnumerable(list);
            int tasksCount = 0;
            await foreach (var task in tasksAsync)
            {
                // TECH: write out filename with full id, in case task is renamed or there are duplicate display Names
                //var fileName = TodoUtil.NormalizeFileName(task.title) + ".json";
                var fileName = TodoUtil.NormalizeFileName(task.id) + ".json";
                var path = Path.Combine(subdir.FullName, fileName);
                var serialized = task.OriginalSerialized;
                await File.WriteAllTextAsync(path, serialized);
                tasksCount++;
            }

            Console.WriteLine("Exported List: {0} [{1} tasks]", list.displayName, tasksCount);
        }
    }
}
