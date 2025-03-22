using UnityEngine;

public class Selector : Node
{
    // CONSTRUCTORS
    public Selector(string name) : base(name) { }

    // METHODS
    public override Status Process()
    {
        Debug.Log("Running: " + Children[CurrentChildIndex].Name);
        Status childStatus = Children[CurrentChildIndex].Process();
        Debug.Log($"Status of {Children[CurrentChildIndex].Name}: " + childStatus.ToString());

        if (childStatus == Status.RUNNING) return Status.RUNNING;

        if (childStatus == Status.SUCCESS)
        {
            CurrentChildIndex = 0;
            return Status.SUCCESS;
        }

        CurrentChildIndex++;
        if (CurrentChildIndex >= Children.Count)
        {
            CurrentChildIndex = 0;
            return Status.FAILURE;
        }

        return Status.RUNNING;
    }
}
