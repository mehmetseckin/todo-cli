namespace Todo.Core.Model;

public class TodoList
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public int Count => Tasks.Count;
    public bool Shared { get; set; }
    public List<TodoItem> Tasks { get; set; } = [];
}
