using System.Collections.ObjectModel;

namespace mp4explorer.Models;

public class Node
{
    public ObservableCollection<Node> Children { get; } = new();
    public string Title { get; }
  
    public Node(string title)
    {
        Title = title;
    }

    public Node(string title, ObservableCollection<Node> children)
    {
        Title = title;
        Children = children;
    }
}