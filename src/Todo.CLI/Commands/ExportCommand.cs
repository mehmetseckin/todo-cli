using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.CLI.Handlers;
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

            /* TODO_EXPORTLIST - specific list optional argument
            */

            this.SetHandler(() => ExportAllAsync());
        }

        public async Task ExportAllAsync()
        {
            // TODO_EXPORTLIST: only do this if we need to lookup a list id, or need to export all lists
            await Repo.PopulateListsAsync();

            // TODO: root folder to Config
            var dir = Directory.CreateDirectory("export");

            foreach (var list in Repo.Lists)
            {
                await ExportListAsync(list, dir);

            }
        }

        public async Task ExportListAsync(TodoList list, DirectoryInfo dir)
        {
            var listFolderName = TodoUtil.NormalizeFileName(list.displayName);
            var subdir = dir.CreateSubdirectory(listFolderName);
            var tasksAsync = Repo.GetListTasksAsyncEnumerable(list);
            await foreach (var task in tasksAsync)
            {
                //fnord more efficient if just use JsonReader
                //fnord duplicates
                var fileName = TodoUtil.NormalizeFileName(task.title) + ".json";
                var path = Path.Combine(subdir.FullName, fileName);
                var serialized = JsonSerializer.Serialize(task);
                await File.WriteAllTextAsync(path, serialized);
            }
        }
    }
}
