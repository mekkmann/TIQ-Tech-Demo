using System.Collections.Generic;

public enum Status { SUCCESS, RUNNING, FAILURE };
public class Node
{
    public Status Status;
    public List<Node> Children = new();
    public int CurrentChildIndex = 0;
    public string Name;

    // CONSTRUCTORS
    public Node() { }
    public Node(string name)
    {
        Name = name;
    }

    // METHODS
    public virtual Status Process()
    {
        return Children[CurrentChildIndex].Process(); ;
    }

    public void AddChild(Node child)
    {
        // null check with compund assignment
        Children ??= new();

        Children.Add(child);
    }
}
