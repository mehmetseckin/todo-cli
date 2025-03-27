namespace Todo.CLI.Handlers;

using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Todo.CLI.UI;

public class ListCommandHandler
{
    private const char TodoBullet = '-';
    private const char CompletedBullet = '\u2713'; // Sqrt - check mark

    public static Func<bool, bool, DateTime?, string, Task> Create(IServiceProvider serviceProvider)
    {
        return (all, noStatus, olderThan, listName) => Execute(serviceProvider, all, noStatus, olderThan, listName);
    }

    private static async Task Execute(IServiceProvider sp, bool all, bool noStatus, DateTime? olderThan, string listName)
    {
        var userInteraction = sp.GetRequiredService<IUserInteraction>();
        var outputFormatter = userInteraction.OutputFormatter;

        if (!string.IsNullOrWhiteSpace(listName))
        {
            var listRepo = sp.GetRequiredService<ITodoListRepository>();
            var list = await listRepo.GetByNameAsync(listName);
            if (list?.Id is null)
            {
                outputFormatter.FormatError($"No list found with the name '{listName}'.");
            }
            else
            {
                var itemRepo = sp.GetRequiredService<ITodoItemRepository>();
                var tasksCall = await itemRepo.ListByListIdAsync(list.Id, all);
                if(olderThan.HasValue)
                    tasksCall = tasksCall.Where(item => item.IsCompleted && item.Completed < olderThan);
                list.Tasks = tasksCall.ToList();
                outputFormatter.FormatList(list, noStatus);
            }
            return;
        }

        var taskRepo = sp.GetRequiredService<ITodoItemRepository>();
        var tasks = taskRepo.EnumerateAllAsync(all).ToBlockingEnumerable();
        if (olderThan.HasValue)
            tasks = tasks.Where(item => item.IsCompleted && item.Completed < olderThan);
        outputFormatter.FormatItems(tasks, noStatus);
    }
}
