using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType {
    Sequence = 0,
    Fallback = 1,
    Parallel = 2,
    Decorator = 3,
    Condition = 4,
    Action = 5
}

public enum ResultType {
    Success = 0, 
    Failure = 1,
    Running = 2,
}

/// <summary>
/// 这里的 Node 设计出来就是用于在 Update() 里更新的
/// 抛弃 int 索引的形式, 试着用 C++ 的指针思想去记录节点在树中的位置
/// 就要特别清楚，C++ 中的指针，在C# 里应该是什么 —— 应该是一个引用传递
/// 基类 —— 勘误：行为树里根本不存在专门的根节点，而是
/// </summary>
public abstract class BaseNode 
{
    public NodeType nodeType;
    public List<BaseNode> children;
    public ResultType nodeState;
    public abstract ResultType Execute();
}

public abstract class ControlFlowNode : BaseNode {
    public BaseNode lastRunningNode;
}

public abstract class ExecutionNode : BaseNode {
    public BaseNode lastRunningNode;  
}

/// <summary>
/// 
/// </summary>
public class Sequence : ControlFlowNode
{
    public Sequence() {
        children = new List<BaseNode>();
        nodeType = NodeType.Sequence;
    }

    public override ResultType Execute() {
        // 当我使用 foreach 的时候，变量是被引用了还是被拷贝了？
        foreach (BaseNode child in children)
        {
            switch (child.Execute())
            {
                case ResultType.Success:
                    continue;
                case ResultType.Failure:
                    nodeState = ResultType.Failure;
                    return nodeState;
                case ResultType.Running:
                    lastRunningNode = child;
                    nodeState = ResultType.Running;
                    return nodeState;
            }
        }
        nodeState = ResultType.Success; // 如果顺序执行到底，中途没有打断，则默认成功
        return nodeState;
    }
}

/// <summary>
/// 
/// </summary>
public class Fallback : ControlFlowNode {
    public Fallback()
    {
        children = new List<BaseNode>();
        nodeType = NodeType.Fallback;
    }

    public override ResultType Execute()
    {
        foreach (BaseNode child in children)
        {
            switch (child.Execute())
            {
                case ResultType.Success:
                    nodeState = ResultType.Success;
                    return nodeState;
                case ResultType.Failure:
                    continue;
                case ResultType.Running:
                    lastRunningNode = child;
                    nodeState = ResultType.Running;
                    return nodeState;
            }
        }
        nodeState = ResultType.Failure; // 如果顺序执行到底，中途没有打断，则默认失败
        return nodeState;
    }
}

/// <summary>
/// 
/// </summary>
public class Decorator : ControlFlowNode {

    public Decorator(BaseNode onlyChild) {
        nodeType = NodeType.Decorator;
        children = new List<BaseNode>(1);
        children.Add(onlyChild);
    }

    public Decorator() {
        nodeType = NodeType.Decorator;
        children = new List<BaseNode>(1);
    }

    public override ResultType Execute()
    {
        //TODO: How to decorate?
        return ResultType.Failure;
    }
}

/// <summary>
/// 
/// </summary>
public class Condtion : ExecutionNode {
    public delegate bool ConditionDelegation();
    public ConditionDelegation detailCondition;

    public Condtion() {
        nodeType = NodeType.Condition;
        children = new List<BaseNode>();
    }

    public Condtion(ConditionDelegation delegation)
    {
        nodeType = NodeType.Condition;
        children = new List<BaseNode>();
        detailCondition = delegation;
    }

    public override ResultType Execute()
    {
        if (detailCondition != null) {
            if (detailCondition())
            {
                nodeState = ResultType.Success;
            }
            else {
                nodeState = ResultType.Failure;
            }
            return nodeState;
        }
        Debug.LogError("ConditionDelegation is null!");
        return ResultType.Failure;
        
    }
}

/// <summary>
/// 
/// </summary>
public class Action : ExecutionNode {
    public delegate void ActionDelegation();    // 先声明一个返回void 、无参的委托类型 ActionDelegation
    public ActionDelegation detailAction;

    public Action(ActionDelegation delegation)
    {
        nodeType = NodeType.Action;
        detailAction = delegation;
        children = new List<BaseNode>();
    }
    public Action()
    {
        nodeType = NodeType.Action;
        children = new List<BaseNode>();
    }

    public override ResultType Execute()
    {
        if (detailAction == null)
            return ResultType.Failure;
        detailAction();
        nodeState = ResultType.Success;
        return nodeState;
    }
}