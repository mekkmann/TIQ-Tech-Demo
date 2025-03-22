public class Leaf : Node
{
    public delegate Status Tick();

    public Tick ProcessMethod;

    // CONSTRUCTORS
    public Leaf() { }
    public Leaf(string name, Tick processMethod)
    {
        Name = name;
        ProcessMethod = processMethod;
    }

    // METHODS
    public override Status Process()
    {
        if (ProcessMethod != null)
        {
            return ProcessMethod();
        }

        return Status.FAILURE;
    }
}
