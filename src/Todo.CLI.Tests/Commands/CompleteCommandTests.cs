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
    public async Task Complete_WithValidId_ShouldCompleteItem()
    {
        // Arrange
        var itemId = "1";
        var items = new List<TodoItem>
        {
            new() { Id = itemId, Subject = "Test Item", IsCompleted = false },
            new() { Id = "2", Subject = "Other Item", IsCompleted = false }
        };

        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(items);

        var handler = Todo.CLI.Handlers.CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(itemId);

        // Assert
        Assert.Equal(0, result);
        _mockRepository.Verify(r => r.CompleteAsync(It.Is<TodoItem>(i => i.Id == itemId)), Times.Once);
        _mockRepository.Verify(r => r.CompleteAsync(It.Is<TodoItem>(i => i.Id != itemId)), Times.Never);
    }

    [Fact]
    public async Task Complete_WithInvalidId_ShouldShowError()
    {
        // Arrange
        var itemId = "999";
        var items = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Test Item", IsCompleted = false },
            new() { Id = "2", Subject = "Other Item", IsCompleted = false }
        };

        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(items);

        var handler = Todo.CLI.Handlers.CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(itemId);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError($"Item with ID {itemId} not found."), Times.Once);
        _mockRepository.Verify(r => r.CompleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task Complete_WithEmptyId_ShouldShowError()
    {
        // Arrange
        var handler = Todo.CLI.Handlers.CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler("");

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("Please provide an item ID to complete."), Times.Once);
        _mockRepository.Verify(r => r.CompleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task Complete_WhenRepositoryThrowsException_ShouldShowError()
    {
        // Arrange
        var itemId = "1";
        var items = new List<TodoItem>
        {
            new() { Id = itemId, Subject = "Test Item", IsCompleted = false }
        };

        _mockRepository.Setup(r => r.ListAllAsync(false))
            .ReturnsAsync(items);
        _mockRepository.Setup(r => r.CompleteAsync(It.IsAny<TodoItem>()))
            .ThrowsAsync(new Exception("Test error"));

        var handler = Todo.CLI.Handlers.CompleteCommandHandler.Create(_serviceProvider);

        // Act
        var result = await handler(itemId);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("Error completing item: Test error"), Times.Once);
    }
} 