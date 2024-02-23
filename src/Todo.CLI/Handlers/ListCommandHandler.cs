namespace Todo.CLI.Handlers;

using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;

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
        if (!string.IsNullOrWhiteSpace(listName))
        {
            var listRepo = sp.GetRequiredService<ITodoListRepository>();
            var list = await listRepo.GetByNameAsync(listName);
            if (list?.Id is null)
                Console.WriteLine($"No list found with the name '{listName}'.");
            else
            {
                var itemRepo = sp.GetRequiredService<ITodoItemRepository>();
                var tasksCall = await itemRepo.ListByListIdAsync(list.Id, all);
                if(olderThan.HasValue)
                    tasksCall = tasksCall.Where(item => item.IsCompleted && item.Completed < olderThan);
                list.Tasks = tasksCall.ToList();
                Render(list, noStatus);
            }

            return;
        }

        var taskRepo = sp.GetRequiredService<ITodoItemRepository>();
        var tasks = taskRepo.EnumerateAllAsync(all).ToBlockingEnumerable();
        if (olderThan.HasValue)
            tasks = tasks.Where(item => item.IsCompleted && item.Completed < olderThan);
        foreach (var item in tasks)
        {
            if (!noStatus)
            {
                RenderBullet(item);
                Console.Write(" ");
            }

            Render(item, noStatus);
        }
    }

    private static void Render(TodoList list, bool noStatus)
    {
        Console.WriteLine($"{list.Name} ({list.Count}):");
        foreach (var item in list.Tasks) Render(item, noStatus);
    }

    private static void Render(TodoItem item, bool noStatus)
    {
        Console.WriteLine(item.ToString(noStatus));
    }

    private static void RenderBullet(TodoItem item)
    {
        ConsoleColor bulletColor;
        char bullet;

        if (item.IsCompleted)
        {
            bulletColor = ConsoleColor.Green;
            bullet = CompletedBullet;
        }
        else
        {
            bulletColor = ConsoleColor.Red;
            bullet = TodoBullet;
        }

        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = bulletColor;
        Console.Write(bullet);
        Console.ForegroundColor = previousColor;
    }
}
