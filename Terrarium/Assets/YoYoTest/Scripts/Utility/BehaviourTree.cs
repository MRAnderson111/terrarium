/// <summary>
/// The machinery of a behaviour tree. Three ideas, about thirty lines.
/// There is no Unity in this file. It would work in any C# program.
///
/// The pieces you write for your own game live in TreeNodes.cs.
/// </summary>

public enum Status { Success, Failure, Running }

/// Everything in a tree is a Node, and every Node can be ticked.
public abstract class Node
{
    public abstract Status Tick();
}

/// SELECTOR means "try these in order until one of them works."
/// It is a priority list. The first child that does not fail wins.
public class Selector : Node
{
    Node[] children;

    public Selector(params Node[] children)
    {
        this.children = children;
    }

    public override Status Tick()
    {
        foreach (Node child in children)
        {
            Status result = child.Tick();

            if (result != Status.Failure)
                return result;          // it worked, or it is still busy
        }

        return Status.Failure;          // nothing worked
    }
}

/// SEQUENCE means "do all of these in order, and stop if one fails."
/// Use it to glue a question onto an action.
public class Sequence : Node
{
    Node[] children;

    public Sequence(params Node[] children)
    {
        this.children = children;
    }

    public override Status Tick()
    {
        foreach (Node child in children)
        {
            Status result = child.Tick();

            if (result != Status.Success)
                return result;          // it failed, or it is still busy
        }

        return Status.Success;
    }
}
