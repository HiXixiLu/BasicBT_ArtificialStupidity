using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* HexCell：一个纯粹的信息单元，用于保存六边形棋格在立方体坐标系下的坐标信息，保存颜色信息，邻居信息等其他信息
 */
public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public Color color; //仅仅以 HexCell 为单位记录颜色信息
    [SerializeField]
    HexCell[] neighbors;    //可动态地运算好并保存

    [SerializeField] public RectTransform uiRect;

    public int Elevation {
        get {
            return elevation;
        }
        set {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = elevation * HexMetrics.elevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
        }
    }
    int elevation;

    public HexCell GetNeighbor(HexDirections direction) {
        return neighbors[(int)direction];
    }
    public void SetNeighbors(HexDirections direction, HexCell cell) {
        neighbors[(int)direction] = cell;
        // 注意： 相邻关系是相互的
        cell.neighbors[(int)direction.Opposite()] = this;
    }
}

[System.Serializable]
public struct HexCoordinates
{
    [SerializeField]
    private int x, z;

    // cube coordinate ： x+y+z = 0
    public int X
    {
        get
        {
            return x;
        }
    }

    public int Z
    {
        get
        {
            return z;
        }
    }

    public int Y
    {
        get
        {
            return -X - Z;
        }
    }

    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);    //将整个坐标归入平面 x + y + z = 0
    }
    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }
    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }

    // 将射线检测点转换为单独的Hex中心点
    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);   // 确定一个点击位置属于哪个坐标
        float y = -x;   // y坐标是x坐标的镜像

        // 为了解决 z 不为 0 时候的点击位置：
        float offset = position.z / (HexMetrics.outerRadius * 3f);  //z方向上每两行, offset的整数部分才会加1
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);   //将 f 四舍五入到最近的整数
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);
        if (iX + iY + iZ != 0)
        {
            Debug.LogWarning("rounding error!");
            // 坐标修正
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            // 为什么不需要修正 iY？
            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinates(iX, iZ);
    }
}

// 顶点朝上式摆放
public enum HexDirections {
    NE, E, SE, SW, W, NW
}
/* 对 HexDirection 的 Extension Method
* An extension method is a static method inside a static class that behaves like an instance method of some type. 
* That type could be anything, a class, an interface, a struct, a primitive value, or an enum
*/
public static class HexDirectionEctendions {

    public static HexDirections Opposite(this HexDirections direction) {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);  //enum可以隐式转为int进行运算，但反过来不可以
    }
    public static HexDirections Previous(this HexDirections direction) {
        return direction == HexDirections.NE ? HexDirections.NW : (direction - 1);
    }
    public static HexDirections Next(this HexDirections direction) {
        return direction == HexDirections.NW ? HexDirections.NE : (direction + 1);
    }
}