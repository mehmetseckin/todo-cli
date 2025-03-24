using InquirerCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Core.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.CLI.Handlers;

using Core.Repository;

public class RemoveCommandHandler
{
    private const string PromptMessage = "Which item(s) would you like to delete?";
    private const string UIHelpMessage = "Use arrow keys to navigate between options. [SPACEBAR] to mark the options, and [ENTER] to confirm your input.";

    public static Func<string, DateTime?, bool, bool, Task<int>> Create(IServiceProvider serviceProvider)
    {
        return async (listName, olderThan, removeAll, completedOnly) =>
        {
            var todoItemRepository = serviceProvider.GetRequiredService<ITodoItemRepository>();

            // Retrieve items
            var items = (string.IsNullOrEmpty(listName)
                ? await todoItemRepository.ListAllAsync(includeCompleted: true)
                : await todoItemRepository.ListByListNameAsync(listName, includeCompleted: true)).ToList();

            if (completedOnly)
            {
                items = items.Where(item => item.IsCompleted).ToList();
            }

            if (olderThan.HasValue)
            {
                items = items.Where(item =>
                    item.IsCompleted && item.Completed.HasValue && 
                    item.Completed.Value < olderThan.Value
                    )
                    .ToList();
            }

            if (items.Count == 0)
            {
                Console.WriteLine("No items found.");
                return 0;
            }

            return !removeAll
                ? await RemoveSpecificItems(items, todoItemRepository)
                : await RemoveAllItems(items, todoItemRepository);
        };
    }

    private static async Task<int> RemoveSpecificItems(List<TodoItem> items, ITodoItemRepository todoItemRepository)
    {
        // Ask user which item to delete
        var message = PromptMessage
                      + Environment.NewLine
                      + Environment.NewLine
                      + UIHelpMessage;

        try
        {
            var selectedItems = Question
                .Checkbox(message, items)
                .Page(50)
                .Prompt();

            await DeleteItems(todoItemRepository, selectedItems);
            return 0;
        }
        catch (ArgumentOutOfRangeException exc)
        {
            if (exc.ParamName == "top" &&
                exc.Message.Contains(
                    "The value must be greater than or equal to zero and less than the console's buffer size in that dimension.",
                    StringComparison.Ordinal))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                await Console.Error.WriteLineAsync(
                    $"Too many tasks ({items.Count}) to display on the current console. Filter tasks by passing a specific list using the --list parameter, or increase buffer size of the console.");
                Console.ResetColor();
                return 1;
            }

            throw;
        }
    }

    private static async Task<int> RemoveAllItems(List<TodoItem> items, ITodoItemRepository todoItemRepository)
    {
        var message = items.Count < 50
            ? "Are you sure you want to delete all items?" + Environment.NewLine +
              string.Join(Environment.NewLine, items)
            : $"Are you sure you want to delete all {items.Count} items?";

        if (Question.Confirm(message).Prompt())
        {
            await DeleteItems(todoItemRepository, items);
            return 0;
        }

        Console.Clear();
        return 0;
    }

    private static async Task DeleteItems(ITodoItemRepository todoItemRepository, IEnumerable<TodoItem> selectedItems)
    {
        var items = selectedItems.ToList();
        var done = false;
        do
        {
            try
            {
                await Task.WhenAll(items.Select(async item =>
                {
                    await todoItemRepository.DeleteAsync(item);
                    items.Remove(item);
                }).ToArray());
                done = true;
            }
            catch (AggregateException agg)
            {
                var exc = agg.InnerExceptions.First();
                if(exc.Message.Contains("Too many retries performed"))
                {
                    await Console.Out.WriteLineAsync($"Too many requests, rate limit hit, {items.Count} left, waiting a second and trying again...");
                    await Task.Delay(1000);
                }
                else
                {
                    throw;
                }
            }
        } while (!done);
        Console.Clear();
    }
}