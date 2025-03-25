using System.Collections.Generic;
using Todo.Core.Model;
using Todo.CLI.UI;

namespace Todo.CLI.Tests.Handlers;

public class MockUserInteraction : IUserInteraction
{
    private readonly IEnumerable<TodoItem> _itemsToSelect;
    private readonly bool _confirmResult;

    public string? LastPrompt { get; private set; }
    public string? LastMessage { get; private set; }
    public string? LastError { get; private set; }
    public string? LastSuccess { get; private set; }
    public string? LastItem { get; private set; }
    public string? LastList { get; private set; }
    public string? LastItems { get; private set; }
    public IOutputFormatter OutputFormatter { get; } = new InteractiveOutputFormatter();

    public MockUserInteraction(IEnumerable<TodoItem> itemsToSelect, bool confirmResult = true)
    {
        _itemsToSelect = itemsToSelect;
        _confirmResult = confirmResult;
    }

    public IEnumerable<TodoItem> SelectItems(string message, IEnumerable<TodoItem> items)
    {
        return _itemsToSelect;
    }

    public bool Confirm(string message)
    {
        return _confirmResult;
    }

    public void ShowError(string message)
    {
        // No-op in tests
    }

    public void ShowSuccess(string message)
    {
        // No-op in tests
    }

    public void ClearScreen()
    {
        // No-op in tests
    }

    public void Prompt(string message)
    {
        LastPrompt = message;
    }

    public void Message(string message)
    {
        LastMessage = message;
    }

    public void Error(string message)
    {
        LastError = message;
    }

    public void Success(string message)
    {
        LastSuccess = message;
    }

    public void Item(TodoItem item, bool noStatus = false)
    {
        LastItem = OutputFormatter.FormatItem(item, noStatus);
    }

    public void List(TodoList list, bool noStatus = false)
    {
        LastList = OutputFormatter.FormatList(list, noStatus);
    }

    public void Items(IEnumerable<TodoItem> items, bool noStatus = false)
    {
        LastItems = OutputFormatter.FormatItems(items, noStatus);
    }
} 