using System.Collections.Generic;
using Todo.Core.Model;
using Todo.CLI.UI;

namespace Todo.CLI.Tests.Handlers;

public class MockUserInteraction : IUserInteraction
{
    private readonly IEnumerable<TodoItem> _itemsToSelect;
    private readonly bool _confirmResult;

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
} 