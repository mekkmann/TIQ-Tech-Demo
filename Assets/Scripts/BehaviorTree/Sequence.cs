using UnityEngine;
public class Sequence : Node
{
    // CONSTRUCTORS
    public Sequence(string name) : base(name) { }

    // METHODS
    public override Status Process()
    {
        Debug.Log("Running: " + Children[CurrentChildIndex].Name);
        Status childStatus = Children[CurrentChildIndex].Process();
        Debug.Log($"Status of {Children[CurrentChildIndex].Name}: " + childStatus.ToString());

        if (childStatus == Status.RUNNING) return Status.RUNNING;

        if (childStatus == Status.FAILURE) return Status.FAILURE;

        CurrentChildIndex++;
        if (CurrentChildIndex >= Children.Count)
        {
            CurrentChildIndex = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }
}