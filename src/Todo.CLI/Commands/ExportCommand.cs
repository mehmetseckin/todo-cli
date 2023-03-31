using Microsoft.Extensions.DependencyInjection;
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

namespace MSTTool.Commands
{
    public class ExportCommand : TargetListCommandBase
    {
        public ExportCommand(IServiceProvider serviceProvider) : base("export", serviceProvider)
        {
            Description = "Exports ToDo items to JSON files. Optionally takes target list name.";
        }

        // listName: null means all
        public override async Task RunCommandAsync(string listName)
        {
            // TODO: root folder to Config
            var dir = Directory.CreateDirectory("export");

            if (listName != null)
            {
                var list = await Repo.GetListAsync(listName);
                await ExportListAsync(list, dir);
            }
            else // export all
            {
                var lists = await Repo.PopulateListsAsync();

                // TODO_EXCLUDECOMPLETED
                // OPTIMIZE: drp033023 - parallelize
                foreach (var list in Repo.Lists)
                    await ExportListAsync(list, dir);
            }
        }

        public async Task ExportListAsync(TodoList list, DirectoryInfo dir)
        {
            Console.WriteLine("Export List: {0}", list.displayName);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var listFolderName = TodoUtil.NormalizeFileName(list.displayName);
            var subdir = dir.CreateSubdirectory(listFolderName);
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
