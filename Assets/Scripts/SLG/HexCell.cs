using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public Color color;
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

            // 不需要修正 iY？
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