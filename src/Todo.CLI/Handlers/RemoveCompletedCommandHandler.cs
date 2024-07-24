using System.Collections.Generic;

namespace Todo.CLI.Handlers;

using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;

public class RemoveCompletedCommandHandler
{
    public static Func<string, Task> Create(IServiceProvider serviceProvider)
    {
        return (listName) => Execute(serviceProvider, listName);
    }

    private static async Task Execute(IServiceProvider sp, string listName)
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


                list.Tasks = (await itemRepo.ListByListIdAsync(list.Id, true)).ToList();
                while(list.Tasks.Any(t => t.IsCompleted)){
                  DeleteItems(itemRepo, list.Tasks.Where(t => t.IsCompleted));
                  list.Tasks = (await itemRepo.ListByListIdAsync(list.Id, true)).ToList();
                }
            }

            return;
        }

        Console.WriteLine("No list name provided.");
    }


    private static void DeleteItems(ITodoItemRepository todoItemRepository, IEnumerable<TodoItem> selectedItems)
    {

        foreach (var item in selectedItems)
        {
            Console.WriteLine($"Attempt to delete `{item.Subject}`");
            todoItemRepository.DeleteAsync(item).GetAwaiter().GetResult();
        }

        Console.Clear();
    }
}
