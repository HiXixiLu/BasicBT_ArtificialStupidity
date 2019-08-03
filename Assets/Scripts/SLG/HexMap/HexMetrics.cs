using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public static float outerRadius = 10f;  // set outerRadius as 10f cause the default size of a plane component is 10.
    public static float innerRadius = outerRadius * Mathf.Sqrt(3) / 2;

    public const float solidFactor = 0.75f;
    public const float blendFactor = 1f - solidFactor;

    public const float elevationStep = 5f;  // y轴上升单位

    // 顶点朝上的排列方式
    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
    };

    // 由于使用了 enum 的枚举方向，因此增加一个依靠 HexDirections 判断索引的方法
    public static Vector3 GetFirstCorner(HexDirections direction) {
        return corners[(int)direction];
    }
    public static Vector3 GetSecondCorner(HexDirections direction) {
        return corners[((int)direction + 1) % 6];
    }

    // 从原顶点后缩到 solidFactor 所拾取比例的新顶点 
    public static Vector3 GetFirstSolidCorner(HexDirections direction)
    {
        return corners[(int)direction] * solidFactor;
    }
    public static Vector3 GetSecondSolidCorner(HexDirections direction)
    {
        return corners[((int)direction + 1) % 6] * solidFactor;
    }

    // 为防止邻居之间的射线污染
    public static Vector3 GetBridge(HexDirections direction) {
        //return (GetFirstCorner(direction) + GetSecondCorner(direction)) * 0.5f * blendFactor;

        // 将两个六边形之间的 bridge 合并
        return (GetFirstCorner(direction) + GetSecondCorner(direction)) * blendFactor;
    }
}
