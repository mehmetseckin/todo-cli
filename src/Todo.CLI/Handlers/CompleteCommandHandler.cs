using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Model;
using Todo.Core.Repository;
using Todo.CLI.UI;
using System.Collections.Generic;

namespace Todo.CLI.Handlers;

public class CompleteCommandHandler
{
    private const string PromptMessage = "Which item(s) would you like to complete?";
    private const string UIHelpMessage = "Use arrow keys to navigate between options. [SPACEBAR] to mark the options, and [ENTER] to confirm your input.";
    private readonly IUserInteraction _userInteraction;
    private readonly ITodoItemRepository _todoItemRepository;

    private CompleteCommandHandler(ITodoItemRepository todoItemRepository, IUserInteraction userInteraction)
    {
        _todoItemRepository = todoItemRepository;
        _userInteraction = userInteraction;
    }

    public static Func<string[], string, DateTime?, bool, Task<int>> Create(IServiceProvider serviceProvider)
    {
        var todoItemRepository = serviceProvider.GetRequiredService<ITodoItemRepository>();
        var userInteraction = serviceProvider.GetRequiredService<IUserInteraction>();
        var handler = new CompleteCommandHandler(todoItemRepository, userInteraction);
        return handler.HandleAsync;
    }

    private async Task<int> HandleAsync(string[] ids, string listName, DateTime? olderThan, bool completeAll)
    {
        if (ids.Length > 0)
        {
            try
            {
                var items = await _todoItemRepository.ListAllAsync(includeCompleted: false);
                await CompleteItems(items.Where(i => ids.Contains(i.Id)));
                return 0;
            }
            catch (Exception ex)
            {
                _userInteraction.ShowError($"Error completing one or more items: {ex.Message}");
                return 1;
            }
        }
        else
        {
            // Retrieve items
            var items = (string.IsNullOrEmpty(listName)
                ? await _todoItemRepository.ListAllAsync(includeCompleted: false)
                : await _todoItemRepository.ListByListNameAsync(listName, includeCompleted: false)).ToList();

            if (olderThan.HasValue)
            {
                items = items.Where(item =>
                        item.Created.HasValue &&
                        item.Created.Value < olderThan.Value
                    )
                    .ToList();
            }

            if (items.Count == 0)
            {
                _userInteraction.ShowError("No items found.");
                return 0;
            }

            return !completeAll
                ? await CompleteSpecificItems(items)
                : await CompleteAllItems(items);
        }
    }

    private async Task<int> CompleteSpecificItems(List<TodoItem> items)
    {
        // Ask user which item to delete
        var message = PromptMessage
                      + Environment.NewLine
                      + Environment.NewLine
                      + UIHelpMessage;

        try
        {
            var selectedItems = _userInteraction.SelectItems(message, items);
            await CompleteItems(selectedItems);
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

    private async Task<int> CompleteAllItems(List<TodoItem> items)
    {
        var message = items.Count < 50
            ? "Are you sure you want to complete all items?" + Environment.NewLine +
              string.Join(Environment.NewLine, items)
            : $"Are you sure you want to complete all {items.Count} items?";

        if (_userInteraction.Confirm(message))
        {
            await CompleteItems(items);
            return 0;
        }

        _userInteraction.ClearScreen();
        return 0;
    }

    private async Task CompleteItems(IEnumerable<TodoItem> selectedItems)
    {
        var itemsToComplete = selectedItems.ToList();
        var done = false;
        var completedCount = 0;
        do
        {
            try
            {
                await Task.WhenAll(itemsToComplete.Select(async item =>
                {
                    await _todoItemRepository.CompleteAsync(item);
                    completedCount++;
                }).ToArray());
                done = true;
            }
            catch (AggregateException agg)
            {
                var exc = agg.InnerExceptions.First();
                if (exc.Message.Contains("Too many retries performed"))
                {
                    _userInteraction.ShowError($"Too many requests, rate limit hit, {itemsToComplete.Count - completedCount} left, waiting a second and trying again...");
                    await Task.Delay(1000);
                }
                else
                {
                    throw;
                }
            }
        } while (!done);

        if(done) {
            _userInteraction.ShowSuccess($"Completed {completedCount} items.");
        }
    }
}