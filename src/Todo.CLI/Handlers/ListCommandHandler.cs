namespace Todo.CLI.Handlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;

public class ListCommandHandler
{
    private const char TodoBullet = '-';
    private const char CompletedBullet = '\u2713'; // Sqrt - check mark

    public static Func<bool, bool, string, Task> Create(IServiceProvider serviceProvider)
    {
        return (all, noStatus, listName) => Execute(serviceProvider, all, noStatus, listName);
    }

    private static async Task Execute(IServiceProvider sp, bool all, bool noStatus, string listName)
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
                list.Tasks = (await itemRepo.ListByListIdAsync(list.Id, all)).ToList();
                Render(list);
            }

            return;
        }

        var taskRepo = sp.GetRequiredService<ITodoItemRepository>();
        await foreach (var item in taskRepo.EnumerateAllAsync(all))
        {
            if (!noStatus)
            {
                RenderBullet(item);
                Console.Write(" ");
            }

            Render(item);
        }
    }

    private static void Render(TodoList list)
    {
        Console.WriteLine($"{list.Name} ({list.Count}):");
        foreach (var item in list.Tasks) Render(item);
    }

    private static void Render(TodoItem item)
    {
        Console.WriteLine(item);
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
