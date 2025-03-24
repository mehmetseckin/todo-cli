namespace Todo.CLI.Handlers;

using System;
using System.Threading.Tasks;
using Core.Model;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Todo.CLI.UI;

public class AddCommandHandler
{
    private readonly IUserInteraction _userInteraction;
    private readonly ITodoListRepository _todoListRepository;
    private readonly ITodoItemRepository _todoItemRepository;

    private AddCommandHandler(
        ITodoListRepository todoListRepository,
        ITodoItemRepository todoItemRepository,
        IUserInteraction userInteraction)
    {
        _todoListRepository = todoListRepository;
        _todoItemRepository = todoItemRepository;
        _userInteraction = userInteraction;
    }

    public class List
    {
        public static Func<string, Task<int>> Create(IServiceProvider serviceProvider)
        {
            var todoListRepository = serviceProvider.GetRequiredService<ITodoListRepository>();
            var userInteraction = serviceProvider.GetRequiredService<IUserInteraction>();
            var handler = new AddCommandHandler(todoListRepository, null, userInteraction);
            return handler.HandleListAsync;
        }
    }

    public class Item
    {
        public static Func<string, string, Task<int>> Create(IServiceProvider serviceProvider)
        {
            var todoListRepository = serviceProvider.GetRequiredService<ITodoListRepository>();
            var todoItemRepository = serviceProvider.GetRequiredService<ITodoItemRepository>();
            var userInteraction = serviceProvider.GetRequiredService<IUserInteraction>();
            var handler = new AddCommandHandler(todoListRepository, todoItemRepository, userInteraction);
            return handler.HandleItemAsync;
        }
    }

    private async Task<int> HandleListAsync(string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                _userInteraction.ShowError("Name is required to add a list.");
                return 1;
            }

            await _todoListRepository.AddAsync(new TodoList
            {
                Name = name
            });
            _userInteraction.ShowSuccess($"List '{name}' created successfully.");
            return 0;
        }
        catch (Exception ex)
        {
            _userInteraction.ShowError($"Error creating list: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> HandleItemAsync(string listName, string subject)
    {
        try
        {
            if (string.IsNullOrEmpty(listName))
            {
                _userInteraction.ShowError("List name is required to add an item.");
                return 1;
            }

            if (string.IsNullOrEmpty(subject))
            {
                _userInteraction.ShowError("Subject is required to add an item.");
                return 1;
            }

            var list = await _todoListRepository.GetByNameAsync(listName);
            if (list == null)
            {
                _userInteraction.ShowError($"No list found with the name '{listName}'.");
                return 1;
            }

            await _todoItemRepository.AddAsync(new TodoItem
            {
                Subject = subject,
                ListId = list.Id
            });
            _userInteraction.ShowSuccess($"Item '{subject}' added to list '{listName}' successfully.");
            return 0;
        }
        catch (Exception ex)
        {
            _userInteraction.ShowError($"Error adding item: {ex.Message}");
            return 1;
        }
    }
}