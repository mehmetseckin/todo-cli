using System.Collections.Generic;
using Todo.Core.Model;

namespace Todo.CLI.UI;

public interface IOutputFormatter
{
    string FormatSuccess(string message);
    string FormatError(string message);
    string FormatList(TodoList list, bool noStatus);
    string FormatItem(TodoItem item, bool noStatus);
    string FormatItems(IEnumerable<TodoItem> items, bool noStatus);
} 