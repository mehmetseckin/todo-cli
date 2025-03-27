using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Todo.CLI.Commands;
using Todo.CLI.Handlers;
using Todo.CLI.Tests.Handlers;
using Todo.CLI.UI;
using Todo.Core.Model;
using Todo.Core.Repository;
using Xunit;

namespace Todo.CLI.Tests.Commands;

public class RemoveCommandTests
{
    private readonly Mock<ITodoItemRepository> _mockItemRepository;
    private readonly Mock<ITodoListRepository> _mockListRepository;
    private readonly Mock<IUserInteraction> _mockUserInteraction;
    private readonly IServiceProvider _serviceProvider;

    public RemoveCommandTests()
    {
        _mockItemRepository = new Mock<ITodoItemRepository>();
        _mockListRepository = new Mock<ITodoListRepository>();
        _mockUserInteraction = new Mock<IUserInteraction>();
        _mockUserInteraction.Setup(x => x.OutputFormatter).Returns(new InteractiveOutputFormatter());

        var services = new ServiceCollection();
        services.AddSingleton(_mockItemRepository.Object);
        services.AddSingleton(_mockListRepository.Object);
        services.AddSingleton<IUserInteraction>(_mockUserInteraction.Object);
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task RemoveCommand_WithCompletedFilter_ShouldOnlyRemoveCompletedItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1", IsCompleted = true },
            new() { Id = "2", Subject = "Task 2", IsCompleted = false },
            new() { Id = "3", Subject = "Task 3", IsCompleted = true }
        };

        var completedItems = items.Where(i => i.IsCompleted).ToList();
        _mockUserInteraction.Setup(ui => ui.SelectItems(It.IsAny<string>(), It.IsAny<IEnumerable<TodoItem>>()))
            .Returns(completedItems);

        _mockItemRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, null, false, true);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.IsCompleted)), Times.Exactly(2));
        _mockItemRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && !i.IsCompleted)), Times.Never);
    }

    [Fact]
    public async Task RemoveCommand_WithListName_ShouldOnlyRemoveFromSpecifiedList()
    {
        // Arrange
        var listName = "Test List";
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1", ListId = listName },
            new() { Id = "2", Subject = "Task 2", ListId = "Other List" }
        };

        var listItems = items.Where(i => i.ListId == listName).ToList();
        _mockUserInteraction.Setup(ui => ui.SelectItems(It.IsAny<string>(), It.IsAny<IEnumerable<TodoItem>>()))
            .Returns(listItems);

        _mockItemRepository.Setup(r => r.ListByListNameAsync(listName, true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), listName, null, false, false);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.ListId == listName)), Times.Once);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.ListId != listName)), Times.Never);
    }

    [Fact]
    public async Task RemoveCommand_WithOlderThanFilter_ShouldOnlyRemoveCompletedItemsBeforeDate()
    {
        // Arrange
        var olderThan = DateTime.Now.AddDays(-1);
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1", IsCompleted = true, Completed = DateTime.Now.AddDays(-2) },
            new() { Id = "2", Subject = "Task 2", IsCompleted = true, Completed = DateTime.Now },
            new() { Id = "3", Subject = "Task 3", IsCompleted = false }
        };

        var oldItems = items.Where(i => i.Completed < olderThan).ToList();
        _mockUserInteraction.Setup(ui => ui.SelectItems(It.IsAny<string>(), It.IsAny<IEnumerable<TodoItem>>()))
            .Returns(oldItems);

        _mockItemRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, olderThan, false, false);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.Completed < olderThan)), Times.Once);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.Completed >= olderThan)), Times.Never);
    }

    [Fact]
    public async Task RemoveCommand_WithNoItems_ShouldReturnSuccess()
    {
        // Arrange
        var items = new List<TodoItem>();
        _mockUserInteraction.Setup(ui => ui.SelectItems(It.IsAny<string>(), It.IsAny<IEnumerable<TodoItem>>()))
            .Returns(items);

        _mockItemRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, null, false, false);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task RemoveCommand_WithAllOption_ShouldRemoveAllItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1" },
            new() { Id = "2", Subject = "Task 2" },
            new() { Id = "3", Subject = "Task 3" }
        };

        _mockUserInteraction.Setup(ui => ui.Confirm(It.IsAny<string>()))
            .Returns(true);

        _mockItemRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, null, true, false);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Exactly(3));
    }

    [Fact]
    public async Task RemoveCommand_WithAllOptionAndUserCancellation_ShouldNotRemoveItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1" },
            new() { Id = "2", Subject = "Task 2" },
            new() { Id = "3", Subject = "Task 3" }
        };

        _mockUserInteraction.Setup(ui => ui.Confirm(It.IsAny<string>()))
            .Returns(false);

        _mockItemRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, null, true, false);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task RemoveCommand_WithIds_ShouldRemoveItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1" },
            new() { Id = "2", Subject = "Task 2" },
            new() { Id = "3", Subject = "Task 3" }
        };

        _mockUserInteraction.Setup(ui => ui.Confirm(It.IsAny<string>()))
            .Returns(true);

        _mockItemRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(new[] { "1", "2" }, null, null, false, false);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RemoveCommand_WithIdsAndUserCancellation_ShouldNotRemoveItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Task 1" },
            new() { Id = "2", Subject = "Task 2" },
            new() { Id = "3", Subject = "Task 3" }
        };

        _mockUserInteraction.Setup(ui => ui.Confirm(It.IsAny<string>()))
            .Returns(false);

        _mockItemRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(new[] { "1", "2" }, null, null, false, false);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task RemoveList_WithValidListName_ShouldRemoveList()
    {
        // Arrange
        var listName = "Test List";
        var list = new TodoList { Id = "list-123", Name = listName };

        _mockListRepository.Setup(r => r.GetByNameAsync(listName))
            .ReturnsAsync(list);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.List.Create(_serviceProvider);

        // Act
        await handler(listName);

        // Assert
        _mockListRepository.Verify(r => r.GetByNameAsync(listName), Times.Once);
        _mockListRepository.Verify(r => r.DeleteAsync(list), Times.Once);
        _mockUserInteraction.Verify(ui => ui.ShowSuccess($"List '{listName}' removed successfully."), Times.Once);
    }

    [Fact]
    public async Task RemoveList_WithNonExistentList_ShouldShowError()
    {
        // Arrange
        var listName = "Non-existent List";

        _mockListRepository.Setup(r => r.GetByNameAsync(listName))
            .ReturnsAsync((TodoList)null);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.List.Create(_serviceProvider);

        // Act
        await handler(listName);

        // Assert
        _mockListRepository.Verify(r => r.GetByNameAsync(listName), Times.Once);
        _mockListRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoList>()), Times.Never);
        _mockUserInteraction.Verify(ui => ui.ShowError($"No list found with the name '{listName}'."), Times.Once);
    }

    [Fact]
    public async Task RemoveList_WithEmptyListName_ShouldShowError()
    {
        // Arrange
        var listName = "";

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.List.Create(_serviceProvider);

        // Act
        await handler(listName);

        // Assert
        _mockListRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
        _mockListRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoList>()), Times.Never);
        _mockUserInteraction.Verify(ui => ui.ShowError("List name is required to remove a list."), Times.Once);
    }

    [Fact]
    public async Task RemoveList_WhenDeletionFails_ShouldShowError()
    {
        // Arrange
        var listName = "Test List";
        var list = new TodoList { Id = "list-123", Name = listName };
        var errorMessage = "Failed to delete list";

        _mockListRepository.Setup(r => r.GetByNameAsync(listName))
            .ReturnsAsync(list);
        _mockListRepository.Setup(r => r.DeleteAsync(list))
            .ThrowsAsync(new Exception(errorMessage));

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.List.Create(_serviceProvider);

        // Act
        await handler(listName);

        // Assert
        _mockListRepository.Verify(r => r.GetByNameAsync(listName), Times.Once);
        _mockListRepository.Verify(r => r.DeleteAsync(list), Times.Once);
        _mockUserInteraction.Verify(ui => ui.ShowError($"Error removing list: {errorMessage}"), Times.Once);
    }
} 