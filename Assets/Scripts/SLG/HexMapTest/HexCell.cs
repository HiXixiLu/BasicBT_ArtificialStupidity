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