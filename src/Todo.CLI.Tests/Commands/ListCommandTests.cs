using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Todo.CLI.Commands;
using Todo.CLI.Handlers;
using Todo.CLI.UI;
using Todo.Core.Model;
using Todo.Core.Repository;
using Xunit;

namespace Todo.CLI.Tests.Commands;

public class ListCommandTests
{
    private readonly Mock<ITodoItemRepository> _mockItemRepository;
    private readonly Mock<ITodoListRepository> _mockListRepository;
    private readonly IServiceProvider _serviceProvider;

    public ListCommandTests()
    {
        _mockItemRepository = new Mock<ITodoItemRepository>();
        _mockListRepository = new Mock<ITodoListRepository>();
        var services = new ServiceCollection();
        services.AddSingleton(_mockItemRepository.Object);
        services.AddSingleton(_mockListRepository.Object);
        _serviceProvider = services.BuildServiceProvider();
    }

    private static async IAsyncEnumerable<TodoItem> GetTestItems(IEnumerable<TodoItem> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }
    }

    [Fact]
    public async Task List_WithNoParameters_ShouldListAllIncompleteItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1", IsCompleted = false },
            new() { Id = "2", Subject = "Task 2", IsCompleted = true },
            new() { Id = "3", Subject = "Task 3", IsCompleted = false }
        };

        _mockItemRepository.Setup(r => r.EnumerateAllAsync(false))
            .Returns(GetTestItems(items));

        var command = new ListCommand(_serviceProvider);
        var handler = ListCommandHandler.Create(_serviceProvider);

        // Act
        await handler(false, false, null, null);

        // Assert
        _mockItemRepository.Verify(r => r.EnumerateAllAsync(false), Times.Once);
    }

    [Fact]
    public async Task List_WithAllOption_ShouldListAllItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1", IsCompleted = false },
            new() { Id = "2", Subject = "Task 2", IsCompleted = true },
            new() { Id = "3", Subject = "Task 3", IsCompleted = false }
        };

        _mockItemRepository.Setup(r => r.EnumerateAllAsync(true))
            .Returns(GetTestItems(items));

        var command = new ListCommand(_serviceProvider);
        var handler = ListCommandHandler.Create(_serviceProvider);

        // Act
        await handler(true, false, null, null);

        // Assert
        _mockItemRepository.Verify(r => r.EnumerateAllAsync(true), Times.Once);
    }

    [Fact]
    public async Task List_WithListName_ShouldListItemsFromSpecificList()
    {
        // Arrange
        var listName = "Test List";
        var listId = "list-123";
        var list = new TodoList { Id = listId, Name = listName };
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1", IsCompleted = false },
            new() { Id = "2", Subject = "Task 2", IsCompleted = true }
        };

        _mockListRepository.Setup(r => r.GetByNameAsync(listName))
            .ReturnsAsync(list);
        _mockItemRepository.Setup(r => r.ListByListIdAsync(listId, false))
            .ReturnsAsync(items);

        var command = new ListCommand(_serviceProvider);
        var handler = ListCommandHandler.Create(_serviceProvider);

        // Act
        await handler(false, false, null, listName);

        // Assert
        _mockListRepository.Verify(r => r.GetByNameAsync(listName), Times.Once);
        _mockItemRepository.Verify(r => r.ListByListIdAsync(listId, false), Times.Once);
    }

    [Fact]
    public async Task List_WithOlderThanFilter_ShouldOnlyShowOlderItems()
    {
        // Arrange
        var olderThan = DateTime.Now.AddDays(-1);
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1", IsCompleted = true, Completed = DateTime.Now.AddDays(-2) },
            new() { Id = "2", Subject = "Task 2", IsCompleted = true, Completed = DateTime.Now },
            new() { Id = "3", Subject = "Task 3", IsCompleted = false }
        };

        _mockItemRepository.Setup(r => r.EnumerateAllAsync(true))
            .Returns(GetTestItems(items));

        var command = new ListCommand(_serviceProvider);
        var handler = ListCommandHandler.Create(_serviceProvider);

        // Act
        await handler(true, false, olderThan, null);

        // Assert
        _mockItemRepository.Verify(r => r.EnumerateAllAsync(true), Times.Once);
    }

    [Fact]
    public async Task List_WithNonExistentList_ShouldHandleGracefully()
    {
        // Arrange
        var listName = "Non-existent List";

        _mockListRepository.Setup(r => r.GetByNameAsync(listName))
            .ReturnsAsync((TodoList)null);

        var command = new ListCommand(_serviceProvider);
        var handler = ListCommandHandler.Create(_serviceProvider);

        // Act & Assert
        await handler(false, false, null, listName);

        _mockListRepository.Verify(r => r.GetByNameAsync(listName), Times.Once);
        _mockItemRepository.Verify(r => r.ListByListIdAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }
} 