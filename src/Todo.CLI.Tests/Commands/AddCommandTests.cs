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
    public async Task AddItem_WithStarOption_ShouldCreateImportantItem()
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
        var result = await handler(subject, listName, true, null);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(i =>
            i.Subject == subject &&
            i.ListId == listId &&
            i.IsImportant)), Times.Once);
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
        var result = await handler(subject, listName, false, null);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(i =>
            i.Subject == subject &&
            i.ListId == listId &&
            !i.IsImportant)), Times.Once);
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
        var result = await handler(subject, listName, false, null);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError($"No list found with the name '{listName}'."), Times.Once);
        _mockItemRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_WithoutListName_ShouldUseDefaultList()
    {
        // Arrange
        var subject = "Test Item";
        var defaultListId = "default-list-123";
        var defaultList = new TodoList { Id = defaultListId, Name = "Default List" };

        _mockListRepository.Setup(r => r.GetDefaultListAsync())
            .ReturnsAsync(defaultList);

        var command = new AddCommand(_serviceProvider);
        var handler = AddCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(subject, null, false, null);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(i =>
            i.Subject == subject &&
            i.ListId == defaultListId &&
            !i.IsImportant)), Times.Once);
    }

    [Fact]
    public async Task AddItem_WithoutListName_AndNoDefaultList_ShouldShowError()
    {
        // Arrange
        var subject = "Test Item";

        _mockListRepository.Setup(r => r.GetDefaultListAsync())
            .ReturnsAsync((TodoList)null);

        var command = new AddCommand(_serviceProvider);
        var handler = AddCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(subject, null, false, null);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("Default list not found. Please specify a list."), Times.Once);
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
        var result = await handler(string.Empty, listName, false, null);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("Subject is required to add an item."), Times.Once);
        _mockItemRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public void ParseDueDate_Null_ReturnsNull()
    {
        // Act
        var result = AddCommandHandler.ParseDueDate(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseDueDate_EmptyString_ReturnsNull()
    {
        // Act
        var result = AddCommandHandler.ParseDueDate(string.Empty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseDueDate_Whitespace_ReturnsNull()
    {
        // Act
        var result = AddCommandHandler.ParseDueDate("   ");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseDueDate_ValidFullDate_ReturnsParsedDate()
    {
        // Arrange
        var dueDateString = "2026-12-25";

        // Act
        var result = AddCommandHandler.ParseDueDate(dueDateString);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2026, result!.Value.Year);
        Assert.Equal(12, result.Value.Month);
        Assert.Equal(25, result.Value.Day);
    }

    [Fact]
    public void ParseDueDate_ValidShortDate_ReturnsCurrentYear()
    {
        // Arrange
        var dueDateString = "12-25";
        var currentYear = DateTime.Now.Year;

        // Act
        var result = AddCommandHandler.ParseDueDate(dueDateString);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(currentYear, result!.Value.Year);
        Assert.Equal(12, result.Value.Month);
        Assert.Equal(25, result.Value.Day);
    }

    [Fact]
    public void ParseDueDate_InvalidFormat_ReturnsNull()
    {
        // Act
        var result = AddCommandHandler.ParseDueDate("not-a-date");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseDueDate_WrongFormat_ReturnsNull()
    {
        // Act
        var result = AddCommandHandler.ParseDueDate("25-12-2026");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddItem_WithValidDueDate_ShouldCreateItemWithDueDate()
    {
        // Arrange
        var listName = "Test List";
        var subject = "Test Item";
        var listId = "list-123";
        var list = new TodoList { Id = listId, Name = listName };
        var dueDateString = "2026-12-25";

        _mockListRepository.Setup(r => r.GetByNameAsync(listName))
            .ReturnsAsync(list);

        var handler = AddCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(subject, listName, false, dueDateString);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(i =>
            i.Subject == subject &&
            i.ListId == listId &&
            i.DueDate != null &&
            i.DueDate!.Value.Year == 2026 &&
            i.DueDate.Value.Month == 12 &&
            i.DueDate.Value.Day == 25)), Times.Once);
    }

    [Fact]
    public async Task AddItem_WithInvalidDueDate_ShouldShowError()
    {
        // Arrange
        var listName = "Test List";
        var subject = "Test Item";
        var dueDateString = "invalid-date";

        var handler = AddCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(subject, listName, false, dueDateString);

        // Assert
        Assert.Equal(1, result);
        _mockUserInteraction.Verify(ui => ui.ShowError("Invalid due date format. Use yyyy-MM-dd or MM-dd."), Times.Once);
        _mockItemRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_WithoutDueDate_ShouldCreateItemWithNullDueDate()
    {
        // Arrange
        var listName = "Test List";
        var subject = "Test Item";
        var listId = "list-123";
        var list = new TodoList { Id = listId, Name = listName };

        _mockListRepository.Setup(r => r.GetByNameAsync(listName))
            .ReturnsAsync(list);

        var handler = AddCommandHandler.Item.Create(_serviceProvider);

        // Act
        var result = await handler(subject, listName, false, null);

        // Assert
        Assert.Equal(0, result);
        _mockItemRepository.Verify(r => r.AddAsync(It.Is<TodoItem>(i =>
            i.Subject == subject &&
            i.ListId == listId &&
            i.DueDate == null)), Times.Once);
    }
} 