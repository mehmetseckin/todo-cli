using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Todo.CLI.Commands;
using Todo.CLI.Handlers;
using Todo.CLI.UI;
using Todo.Core.Model;
using Todo.Core.Repository;
using Xunit;

namespace Todo.CLI.Tests.Commands;

public class CompleteCommandTests
{
    private readonly Mock<ITodoItemRepository> _mockRepository;
    private readonly Mock<IUserInteraction> _mockUserInteraction;
    private readonly IServiceProvider _serviceProvider;

    public CompleteCommandTests()
    {
        _mockRepository = new Mock<ITodoItemRepository>();
        _mockUserInteraction = new Mock<IUserInteraction>();
        var services = new ServiceCollection();
        services.AddSingleton(_mockRepository.Object);
        services.AddSingleton(_mockUserInteraction.Object);
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Complete_WithValidIds_ShouldCompleteItems()
    {
        // Arrange
        var itemIds = new[] { "1", "2" };
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Test Item 1", IsCompleted = false },
            new() { Id = "2", Subject = "Test Item 2", IsCompleted = false },
            new() { Id = "3", Subject = "Other Item", IsCompleted = false }
        };

        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(items);

        var handler = CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(itemIds, null, null, false);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.CompleteAsync(It.Is<TodoItem>(i => i.Id == "1")), Times.Once);
        _mockRepository.Verify(r => r.CompleteAsync(It.Is<TodoItem>(i => i.Id == "2")), Times.Once);
        _mockRepository.Verify(r => r.CompleteAsync(It.Is<TodoItem>(i => i.Id == "3")), Times.Never);
    }

    [Fact]
    public async Task Complete_WithListName_ShouldFilterByList()
    {
        // Arrange
        var listName = "Test List";
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Test Item 1", IsCompleted = false },
            new() { Id = "2", Subject = "Test Item 2", IsCompleted = false }
        };

        _mockRepository.Setup(r => r.ListByListNameAsync(listName, false))
            .ReturnsAsync(items);
        _mockUserInteraction.Setup(ui => ui.SelectItems(It.IsAny<string>(), It.IsAny<IEnumerable<TodoItem>>()))
            .Returns(items);

        var handler = CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), listName, null, false);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.ListByListNameAsync(listName, false), Times.Once);
        _mockRepository.Verify(r => r.CompleteAsync(It.IsAny<TodoItem>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Complete_WithOlderThan_ShouldFilterByDate()
    {
        // Arrange
        var olderThan = DateTime.UtcNow.AddDays(-7);
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Old Item", IsCompleted = false, Created = olderThan.AddDays(-1) },
            new() { Id = "2", Subject = "New Item", IsCompleted = false, Created = olderThan.AddDays(1) }
        };

        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(items);
        _mockUserInteraction.Setup(ui => ui.SelectItems(It.IsAny<string>(), It.IsAny<IEnumerable<TodoItem>>()))
            .Returns(new[] { items[0] });

        var handler = CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, olderThan, false);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.CompleteAsync(It.Is<TodoItem>(i => i.Id == "1")), Times.Once);
        _mockRepository.Verify(r => r.CompleteAsync(It.Is<TodoItem>(i => i.Id == "2")), Times.Never);
    }

    [Fact]
    public async Task Complete_WithAllOption_ShouldCompleteAllItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Test Item 1", IsCompleted = false },
            new() { Id = "2", Subject = "Test Item 2", IsCompleted = false }
        };

        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(items);
        _mockUserInteraction.Setup(ui => ui.Confirm(It.IsAny<string>()))
            .Returns(true);

        var handler = CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, null, true);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.CompleteAsync(It.IsAny<TodoItem>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Complete_WithAllOption_WhenUserCancels_ShouldNotCompleteItems()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Test Item 1", IsCompleted = false },
            new() { Id = "2", Subject = "Test Item 2", IsCompleted = false }
        };

        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(items);
        _mockUserInteraction.Setup(ui => ui.Confirm(It.IsAny<string>()))
            .Returns(false);

        var handler = CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, null, true);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.CompleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task Complete_WhenNoItemsFound_ShouldShowError()
    {
        // Arrange
        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(new List<TodoItem>());

        var handler = CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, null, false);

        // Assert
        Assert.Equal(0, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("No items found."), Times.Once);
        _mockRepository.Verify(r => r.CompleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task Complete_WhenRepositoryThrowsException_ShouldShowError()
    {
        // Arrange
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Test Item", IsCompleted = false }
        };

        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(items);
        _mockRepository.Setup(r => r.CompleteAsync(It.IsAny<TodoItem>()))
            .ThrowsAsync(new Exception("Test error"));

        var handler = CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(new[] { "1" }, null, null, false);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("Error completing one or more items: Test error"), Times.Once);
    }

    [Fact]
    public async Task Complete_WhenTooManyItems_ShouldShowError()
    {
        // Arrange
        var items = new List<TodoItem>();
        for (int i = 0; i < 100; i++)
        {
            items.Add(new TodoItem { Id = i.ToString(), Subject = $"Item {i}", IsCompleted = false });
        }

        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(items);
        _mockUserInteraction.Setup(ui => ui.SelectItems(It.IsAny<string>(), It.IsAny<IEnumerable<TodoItem>>()))
            .Throws(new ArgumentOutOfRangeException("top", 
                "The value must be greater than or equal to zero and less than the console's buffer size in that dimension."));

        var handler = CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(Array.Empty<string>(), null, null, false);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError(
            $"Too many tasks ({items.Count}) to display on the current console. Filter tasks by passing a specific list using the --list parameter, or increase buffer size of the console."), 
            Times.Once);
        _mockRepository.Verify(r => r.CompleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }
} 