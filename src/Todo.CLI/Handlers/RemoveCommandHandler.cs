using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Core.Model;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Repository;
using Todo.CLI.UI;

namespace Todo.CLI.Handlers;

public class RemoveCommandHandler
{
    private const string PromptMessage = "Which item(s) would you like to delete?";
    private const string UIHelpMessage = "Use arrow keys to navigate between options. [SPACEBAR] to mark the options, and [ENTER] to confirm your input.";
    private readonly IUserInteraction _userInteraction;
    private readonly ITodoItemRepository _todoItemRepository;

    private RemoveCommandHandler(ITodoItemRepository todoItemRepository, IUserInteraction userInteraction)
    {
        _todoItemRepository = todoItemRepository;
        _userInteraction = userInteraction;
    }

    public static Func<string[], string, DateTime?, bool, bool, Task<int>> Create(IServiceProvider serviceProvider)
    {
        var todoItemRepository = serviceProvider.GetRequiredService<ITodoItemRepository>();
        var userInteraction = serviceProvider.GetRequiredService<IUserInteraction>();
        var handler = new RemoveCommandHandler(todoItemRepository, userInteraction);
        return handler.HandleAsync;
    }

    private async Task<int> HandleAsync(string[] ids, string listName, DateTime? olderThan, bool removeAll, bool completedOnly)
    {
        if(ids.Length > 0)
        {
            var items = await _todoItemRepository.ListAllAsync(includeCompleted: true);
            return await RemoveAllItems(items.Where(i => ids.Contains(i.Id)).ToList());
        }
        else {
            // Retrieve items
            var items = (string.IsNullOrEmpty(listName)
                ? await _todoItemRepository.ListAllAsync(includeCompleted: true)
                : await _todoItemRepository.ListByListNameAsync(listName, includeCompleted: true)).ToList();

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
                _userInteraction.ShowError("No items found.");
                return 0;
            }

            return !removeAll
                ? await RemoveSpecificItems(items)
                : await RemoveAllItems(items);
        }
    }

    private async Task<int> RemoveSpecificItems(List<TodoItem> items)
    {
        // Ask user which item to delete
        var message = PromptMessage
                      + Environment.NewLine
                      + Environment.NewLine
                      + UIHelpMessage;

        try
        {
            var selectedItems = _userInteraction.SelectItems(message, items);
            await DeleteItems(selectedItems);
            return 0;
        }
        catch (ArgumentOutOfRangeException exc)
        {
            if (exc.ParamName == "top" &&
                exc.Message.Contains(
                    "The value must be greater than or equal to zero and less than the console's buffer size in that dimension.",
                    StringComparison.Ordinal))
            {
                _userInteraction.ShowError(
                    $"Too many tasks ({items.Count}) to display on the current console. Filter tasks by passing a specific list using the --list parameter, or increase buffer size of the console.");
                return 1;
            }

            throw;
        }
    }

    private async Task<int> RemoveAllItems(List<TodoItem> items)
    {
        var message = items.Count < 50
            ? "Are you sure you want to delete all items?" + Environment.NewLine +
              string.Join(Environment.NewLine, items)
            : $"Are you sure you want to delete all {items.Count} items?";

        if (_userInteraction.Confirm(message))
        {
            await DeleteItems(items);
            return 0;
        }

        return 0;
    }

    private async Task DeleteItems(IEnumerable<TodoItem> selectedItems)
    {
        var itemsToDelete = selectedItems.ToList();
        var done = false;
        var deletedCount = 0;
        do
        {
            try
            {
                await Task.WhenAll(itemsToDelete.Select(async item =>
                {
                    await _todoItemRepository.DeleteAsync(item);
                    deletedCount++;
                }).ToArray());
                done = true;
            }
            catch (AggregateException agg)
            {
                var exc = agg.InnerExceptions.First();
                if(exc.Message.Contains("Too many retries performed"))
                {
                    _userInteraction.ShowError($"Too many requests, rate limit hit, {itemsToDelete.Count - deletedCount} left, waiting a second and trying again...");
                    await Task.Delay(1000);
                }
                else
                {
                    throw;
                }
            }
        } while (!done);

        if(done) {
            _userInteraction.ShowSuccess($"Deleted {deletedCount} items.");
        }
    }
}