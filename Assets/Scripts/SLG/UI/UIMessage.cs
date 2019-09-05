using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UIMessage<T>
{
    T[] paramList;
    UIMessageCategory caterroty;
}

public enum UIMessageCategory {
    Movement, Interaction
}