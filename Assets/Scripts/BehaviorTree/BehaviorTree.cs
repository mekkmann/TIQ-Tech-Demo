using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : Node
{
    // CONSTRUCTORS
    public BehaviorTree()
    {
        Name = "Tree";
    }
    public BehaviorTree(string name) : base(name) { }

    // METHODS
    public override Status Process()
    {
        return base.Process();
    }

    // methods and members relating to printing the tree to the console
    private struct NodeLevel
    {
        public int Level;
        public Node Node;
    }

    public void PrintTree()
    {
        string treePrintOut = "";

        Stack<NodeLevel> nodeStack = new();
        Node currentNode = this;
        nodeStack.Push(
            new NodeLevel { Level = 0, Node = currentNode }
            );

        while (nodeStack.Count != 0)
        {
            NodeLevel nextNode = nodeStack.Pop();
            treePrintOut += new string('-', nextNode.Level) + $"{nextNode.Node.Name} \n";
            for (int i = nextNode.Node.Children.Count - 1; i >= 0; i--)
            {
                nodeStack.Push(
                    new NodeLevel { Level = nextNode.Level + 1, Node = nextNode.Node.Children[i] }
                    );
            }
        }

        Debug.Log(treePrintOut);
    }
}
