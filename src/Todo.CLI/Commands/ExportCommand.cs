using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
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
    public class ExportCommand : Command
    {
        public TodoItemRepository Repo { get; }

        public ExportCommand(IServiceProvider serviceProvider) : base("export")
        {
            Repo = serviceProvider.GetService<TodoItemRepository>();

            Description = "Exports ToDo items to JSON files";

            /* TODO_LISTARGUMENT - specific list optional argument
            */

            this.SetHandler(() => ExportAllAsync());
        }

        public async Task ExportAllAsync()
        {
            // TODO_LISTARGUMENT: only do this if we need to lookup a list id, or need to export all lists
            await Repo.PopulateListsAsync();

            // TODO: root folder to Config
            var dir = Directory.CreateDirectory("export");

            //fnord export all vs not completed only

            foreach (var list in Repo.Lists)
            {
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
                //fnord duplicates
                var fileName = TodoUtil.NormalizeFileName(task.title) + ".json";
                var path = Path.Combine(subdir.FullName, fileName);
                var serialized = task.OriginalSerialized;
                await File.WriteAllTextAsync(path, serialized);
                tasksCount++;
            }

            Console.WriteLine("Exported List: {0} [{1} tasks]", list.displayName, tasksCount);
        }
    }
}
