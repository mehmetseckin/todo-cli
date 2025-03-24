using System;
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

public class AddCommandTests
{
    private readonly Mock<ITodoListRepository> _mockListRepository;
    private readonly Mock<ITodoItemRepository> _mockItemRepository;
    private readonly Mock<IUserInteraction> _mockUserInteraction;
    private readonly IServiceProvider _serviceProvider;

    public AddCommandTests()
    {
        _mockListRepository = new Mock<ITodoListRepository>();
        _mockItemRepository = new Mock<ITodoItemRepository>();
        _mockUserInteraction = new Mock<IUserInteraction>();
        var services = new ServiceCollection();
        services.AddSingleton(_mockListRepository.Object);
        services.AddSingleton(_mockItemRepository.Object);
        services.AddSingleton(_mockUserInteraction.Object);
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task AddList_WithValidName_ShouldCreateList()
    {
        // Arrange
        var listName = "Test List";
        var command = new AddCommand(_serviceProvider);
        var handler = AddCommandHandler.List.Create(_serviceProvider);

        // Act
        var result = await handler(listName);

        // Assert
        Assert.Equal(0, result);
        _mockListRepository.Verify(r => r.AddAsync(It.Is<TodoList>(l => l.Name == listName)), Times.Once);
    }

    [Fact]
    public async Task AddList_WithEmptyName_ShouldShowError()
    {
        // Arrange
        var command = new AddCommand(_serviceProvider);
        var handler = AddCommandHandler.List.Create(_serviceProvider);

        // Act
        var result = await handler(string.Empty);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("Name is required to add a list."), Times.Once);
        _mockListRepository.Verify(r => r.AddAsync(It.IsAny<TodoList>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_WithValidListAndSubject_ShouldCreateItem()
    {
        // Arrange
        var listName = "Test List";
        var subject = "Test Item";
        var listId = "list-123";
        var list = new TodoList { Id = listId, Name = listName };

        _mockListRepository.Setup(r => r.GetByNameAsync(listName))
            .ReturnsAsync(list);

        var command = new AddCommand(_serviceProvider);
        var handler = AddCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(listName, subject);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(i => 
            i.Subject == subject && 
            i.ListId == listId)), Times.Once);
    }

    [Fact]
    public async Task AddItem_WithNonExistentList_ShouldShowError()
    {
        // Arrange
        var listName = "Non-existent List";
        var subject = "Test Item";

        _mockListRepository.Setup(r => r.GetByNameAsync(listName))
            .ReturnsAsync((TodoList)null);

        var command = new AddCommand(_serviceProvider);
        var handler = AddCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(listName, subject);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError($"No list found with the name '{listName}'."), Times.Once);
        _mockItemRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_WithEmptyListName_ShouldShowError()
    {
        // Arrange
        var subject = "Test Item";
        var command = new AddCommand(_serviceProvider);
        var handler = AddCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(string.Empty, subject);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("List name is required to add an item."), Times.Once);
        _mockItemRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_WithEmptySubject_ShouldShowError()
    {
        // Arrange
        var listName = "Test List";
        var command = new AddCommand(_serviceProvider);
        var handler = AddCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(listName, string.Empty);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("Subject is required to add an item."), Times.Once);
        _mockItemRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Never);
    }
} 