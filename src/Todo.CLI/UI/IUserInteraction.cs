using System.Collections.Generic;
using Todo.Core.Model;

namespace Todo.CLI.UI;

public interface IUserInteraction
{
    IEnumerable<TodoItem> SelectItems(string message, IEnumerable<TodoItem> items);
    bool Confirm(string message);
    void ShowError(string message);
    void ShowSuccess(string message);
    void ClearScreen();
    IOutputFormatter OutputFormatter { get; }
} 