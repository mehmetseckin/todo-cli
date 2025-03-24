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
    private readonly Mock<ITodoItemRepository> _mockRepository;
    private readonly Mock<IUserInteraction> _mockUserInteraction;
    private readonly IServiceProvider _serviceProvider;

    public RemoveCommandTests()
    {
        _mockRepository = new Mock<ITodoItemRepository>();
        _mockUserInteraction = new Mock<IUserInteraction>();
        var services = new ServiceCollection();
        services.AddSingleton(_mockRepository.Object);
        services.AddSingleton(_mockUserInteraction.Object);
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

        _mockRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(null, null, false, true);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.IsCompleted)), Times.Exactly(2));
        _mockRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && !i.IsCompleted)), Times.Never);
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

        _mockRepository.Setup(r => r.ListByListNameAsync(listName, true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(listName, null, false, false);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.ListId == listName)), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.ListId != listName)), Times.Never);
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

        _mockRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(null, olderThan, false, false);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.Completed < olderThan)), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.Is<TodoItem>(i => i != null && i.Completed >= olderThan)), Times.Never);
    }

    [Fact]
    public async Task RemoveCommand_WithNoItems_ShouldReturnSuccess()
    {
        // Arrange
        var items = new List<TodoItem>();
        _mockUserInteraction.Setup(ui => ui.SelectItems(It.IsAny<string>(), It.IsAny<IEnumerable<TodoItem>>()))
            .Returns(items);

        _mockRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(null, null, false, false);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Never);
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

        _mockRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(null, null, true, false);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Exactly(3));
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

        _mockRepository.Setup(r => r.ListAllAsync(true))
            .ReturnsAsync(items);

        var command = new RemoveCommand(_serviceProvider);
        var handler = RemoveCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(null, null, true, false);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }
} 