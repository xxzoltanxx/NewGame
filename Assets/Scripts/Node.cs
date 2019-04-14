using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public enum NodeStates
{
    SUCCESS,
    FAILURE,
    RUNNING
};
public abstract class Node
{
    /* Delegate that returns the state of the node.*/
    /* The current state of the node */
    protected NodeStates m_nodeState;
    public NodeStates nodeState
    {
        get { return m_nodeState; }
    }
    /* The constructor for the node */
    public Node() { }
    /* Implementing classes use this method to evaluate the desired
    set of conditions */
    public abstract NodeStates Evaluate(float dt);
}

public class Selector : Node
{
    /** The child nodes for this selector */
    protected List<Node> m_nodes = new List<Node>();
    /** The constructor requires a list of child nodes to be
    * passed in*/
    public Selector(List<Node> nodes)
    {
        m_nodes = nodes;
    }
    /* If any of the children reports a success, the selector will
    * immediately report a success upwards. If all children fail,
    * it will report a failure instead.*/
    public override NodeStates Evaluate(float dt)
    {
        foreach (Node node in m_nodes)
        {
            switch (node.Evaluate(dt))
            {
                case NodeStates.FAILURE:
                    continue;
                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
                default:
                    continue;
            }
        }
        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}
public class Sequence : Node
{
    /** Chiildren nodes that belong to this sequence */
    private List<Node> m_nodes = new List<Node>();
    /** Must provide an initial set of children nodes to work */
    public Sequence(List<Node> nodes)
    {
        m_nodes = nodes;
    }
    /* If any child node returns a failure, the entire node fails.
    Whence all
    * nodes return a success, the node reports a success. */
    public override NodeStates Evaluate(float dt)
    {
        bool anyChildRunning = false;
        foreach (Node node in m_nodes)
        {
            switch (node.Evaluate(dt))
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
                case NodeStates.SUCCESS:
                    continue;
                case NodeStates.RUNNING:
                    anyChildRunning = true;
                    continue;
                default:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
            }
        }
        m_nodeState = anyChildRunning ? NodeStates.RUNNING :
        NodeStates.SUCCESS;
        return m_nodeState;
    }
}
public class Inverter : Node
{
    /* Child node to evaluate */
    private Node m_node;
    public Node node
    {
        get { return m_node; }
    }
    /* The constructor requires the child node that this inverter
    decorator
    * wraps*/
    public Inverter(Node node)
    {
        m_node = node;
    }
    /* Reports a success if the child fails and
    * a failure if the child succeeds. Running will report
    * as running */
    public override NodeStates Evaluate(float dt)
    {
        switch (m_node.Evaluate(dt))
        {
            case NodeStates.FAILURE:
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;
            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;
        }
        m_nodeState = NodeStates.SUCCESS;
        return m_nodeState;
    }
}

public class ActionNode : Node
{
    /* Method signature for the action. */
    public delegate NodeStates ActionNodeDelegate(float dt);
    /* The delegate that is called to evaluate this node */
    private ActionNodeDelegate m_action;
    /* Because this node contains no logic itself,
    * the logic must be passed in in the form of
    * a delegate. As the signature states, the action
    * needs to return a NodeStates enum */
    public ActionNode(ActionNodeDelegate action)
    {
        m_action = action;
    }
    /* Evaluates the node using the passed in delegate and
    * reports the resulting state as appropriate */
    public override NodeStates Evaluate(float dt)
    {
        switch (m_action(dt))
        {
            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;
            case NodeStates.FAILURE:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;
            default:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
        }
    }
}