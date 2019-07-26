using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public static float outerRadius = 10f;  // set outerRadius as 10f cause the default size of a plane component is 10.
    public static float innerRadius = outerRadius * Mathf.Sqrt(3) / 2;

    // 顶点朝上的排列方式
    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
    };
}
