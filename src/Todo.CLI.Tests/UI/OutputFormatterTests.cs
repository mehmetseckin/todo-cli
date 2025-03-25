using System;
using System.Collections.Generic;
using System.Text.Json;
using Todo.Core.Model;
using Todo.CLI.UI;
using Xunit;

namespace Todo.CLI.Tests.UI;

public class OutputFormatterTests
{
    private readonly TodoItem _incompleteItem = new()
    {
        Id = "1",
        Subject = "Test Item",
        IsCompleted = false,
        Status = "NotStarted",
        ListId = "list-1"
    };

    private readonly TodoItem _completedItem = new()
    {
        Id = "2",
        Subject = "Completed Item",
        IsCompleted = true,
        Status = "Completed",
        Completed = DateTime.Now,
        ListId = "list-1"
    };

    private readonly TodoList _testList = new()
    {
        Id = "list-1",
        Name = "Test List",
        Tasks = new List<TodoItem>
        {
            new() { Id = "1", Subject = "Item 1", IsCompleted = false },
            new() { Id = "2", Subject = "Item 2", IsCompleted = true }
        }
    };

    [Fact]
    public void InteractiveFormatter_FormatSuccess_ReturnsMessage()
    {
        // Arrange
        var formatter = new InteractiveOutputFormatter();
        var message = "Success message";

        // Act
        var result = formatter.FormatSuccess(message);

        // Assert
        Assert.Equal(message, result);
    }

    [Fact]
    public void InteractiveFormatter_FormatError_ReturnsMessage()
    {
        // Arrange
        var formatter = new InteractiveOutputFormatter();
        var message = "Error message";

        // Act
        var result = formatter.FormatError(message);

        // Assert
        Assert.Equal(message, result);
    }

    [Fact]
    public void InteractiveFormatter_FormatItem_WithStatus_IncludesBullet()
    {
        // Arrange
        var formatter = new InteractiveOutputFormatter();

        // Act
        var result = formatter.FormatItem(_incompleteItem, noStatus: false);

        // Assert
        Assert.Contains(_incompleteItem.Subject, result);
    }

    [Fact]
    public void InteractiveFormatter_FormatItem_WithoutStatus_ExcludesBullet()
    {
        // Arrange
        var formatter = new InteractiveOutputFormatter();

        // Act
        var result = formatter.FormatItem(_incompleteItem, noStatus: true);

        // Assert
        Assert.Contains(_incompleteItem.Subject, result);
        Assert.DoesNotContain("-", result);
    }

    [Fact]
    public void InteractiveFormatter_FormatList_IncludesNameAndCount()
    {
        // Arrange
        var formatter = new InteractiveOutputFormatter();

        // Act
        var result = formatter.FormatList(_testList, noStatus: false);

        // Assert
        Assert.Contains(_testList.Name, result);
        Assert.Contains(_testList.Count.ToString(), result);
    }

    [Fact]
    public void JsonFormatter_FormatSuccess_ReturnsValidJson()
    {
        // Arrange
        var formatter = new JsonOutputFormatter();
        var message = "Success message";

        // Act
        var result = formatter.FormatSuccess(message);

        // Assert
        var jsonDoc = JsonDocument.Parse(result);
        Assert.True(jsonDoc.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(message, jsonDoc.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public void JsonFormatter_FormatError_ReturnsValidJson()
    {
        // Arrange
        var formatter = new JsonOutputFormatter();
        var message = "Error message";

        // Act
        var result = formatter.FormatError(message);

        // Assert
        var jsonDoc = JsonDocument.Parse(result);
        Assert.False(jsonDoc.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(message, jsonDoc.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public void JsonFormatter_FormatItem_ReturnsValidJson()
    {
        // Arrange
        var formatter = new JsonOutputFormatter();

        // Act
        var result = formatter.FormatItem(_incompleteItem, noStatus: false);

        // Assert
        var jsonDoc = JsonDocument.Parse(result);
        var root = jsonDoc.RootElement;
        Assert.Equal(_incompleteItem.Id, root.GetProperty("Id").GetString());
        Assert.Equal(_incompleteItem.Subject, root.GetProperty("Subject").GetString());
        Assert.False(root.GetProperty("IsCompleted").GetBoolean());
        Assert.Equal(_incompleteItem.Status, root.GetProperty("Status").GetString());
        Assert.Equal(_incompleteItem.ListId, root.GetProperty("ListId").GetString());
        Assert.Equal($"{_incompleteItem.Subject} - {_incompleteItem.Status} ", root.GetProperty("DisplayText").GetString());
    }

    [Fact]
    public void JsonFormatter_FormatList_ReturnsValidJson()
    {
        // Arrange
        var formatter = new JsonOutputFormatter();

        // Act
        var result = formatter.FormatList(_testList, noStatus: false);

        // Assert
        var jsonDoc = JsonDocument.Parse(result);
        var root = jsonDoc.RootElement;
        Assert.Equal(_testList.Name, root.GetProperty("Name").GetString());
        Assert.Equal(_testList.Count, root.GetProperty("Count").GetInt32());
        Assert.Equal(2, root.GetProperty("Tasks").GetArrayLength());
    }

    [Fact]
    public void JsonFormatter_FormatItems_ReturnsValidJsonArray()
    {
        // Arrange
        var formatter = new JsonOutputFormatter();
        var items = new[] { _incompleteItem, _completedItem };

        // Act
        var result = formatter.FormatItems(items, noStatus: false);

        // Assert
        var jsonDoc = JsonDocument.Parse(result);
        var root = jsonDoc.RootElement;
        Assert.Equal(2, root.GetArrayLength());
        
        var firstItem = root[0];
        Assert.Equal(_incompleteItem.Id, firstItem.GetProperty("Id").GetString());
        Assert.Equal(_incompleteItem.Subject, firstItem.GetProperty("Subject").GetString());
        
        var secondItem = root[1];
        Assert.Equal(_completedItem.Id, secondItem.GetProperty("Id").GetString());
        Assert.Equal(_completedItem.Subject, secondItem.GetProperty("Subject").GetString());
    }
} 