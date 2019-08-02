using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* When you add a script which uses RequireComponent to a GameObject, 
 * the required component will automatically be added to the GameObject. 
 */
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    MeshCollider meshCollider;
    List<Color> colors;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        meshCollider = gameObject.AddComponent<MeshCollider>(); //有Collider组件才能进行射线检测
        colors = new List<Color>();
    }

    // tell the mesh to triangulate its cells
    public void Triangulate(HexCell[] cells) {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for (int i = 0; i < cells.Length; i++) {
            Triangulate(cells[i]);
        }

        // toArray() 配合 shader 是渲染的关键步骤
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();

        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }
    // 在确定的世界坐标下另行渲染三角形
    //void Triangulate(HexCell cell) {
    //    Vector3 center = cell.transform.localPosition;
    //    for (int i = 0; i < 6; i++)
    //    {
    //        AddTriangle(center, center + HexMetrics.corners[i], center + HexMetrics.corners[(i+1)%6]);
    //        AddTriangleColor(cell.color);
    //    }
    //}
    void Triangulate(HexCell cell) {
        for (HexDirections d = HexDirections.NE; d <= HexDirections.NW; d++) {
            Triangulate(d, cell);
        }
    }
    void Triangulate(HexDirections direction, HexCell cell) {
        Vector3 center = cell.transform.localPosition;
        // 划定 solid color范围
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
        // 三角的正面（即面对屏幕面）的判定以具顶点的索引值 —— 索引排序使顶点为顺时针的即三角形正面
        AddTriangle(center, v1, v2);
        AddTriangleColor(cell.color);

        // 仅渲染三个方向的bridge就可以覆盖所有的方向（因为六边形整体自西向东渲染）
        if (direction <= HexDirections.SE) {
            TriangulateConnection(direction, cell, v1, v2);
        }
    }
    void TriangulateConnection(HexDirections direction, HexCell cell, Vector3 v1, Vector3 v2) {
        // 避免渲染凸出的 bridge
        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null)
            return;

        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;

        // 添加 blend region 的梯形
        AddQuad(v1, v2, v3, v4);

        // 混色 —— 考虑到一个顶点最多由三个Hexagon共享
        //neighbor = cell.GetNeighbor(direction) ?? cell; // 双目运算符 ?? 防止某些没有direction方向邻居的情况
        //HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
        //HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;

        //Color bridgeColor = (cell.color + neighbor.color) * 0.5f;
        AddQuadColor(cell.color, neighbor.color);

        // 填充bridge以外的三角缝隙 —— 这是大三角优化版本,由于三个六边形共享一个空隙三角，因此只需填充两个方向的三角形便可
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (nextNeighbor != null && direction <= HexDirections.E) {
            AddTriangle(v2, v4, v2 + HexMetrics.GetBridge(direction.Next()));
            AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
        }
    }

    //By default, if they are arranged in a clockwise direction the triangle is considered to be forward-facing and visible
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
    // 进行 mesh 的顶点着色 —— 代码中使用顶点着色，但还需要 shader 配合
    void AddTriangleColor(Color color) {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
    // Used to blend colors
    void AddTriangleColor(Color c1, Color c2, Color c3) {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    // blend region 的梯形
    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        int vertexIndex = vertices.Count;
        // 最后才添加 
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        // (v1，v3，v2)
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        // (v2，v3，v4)
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    //void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    //{
    //    // 梯形 blend region时的颜色规则
    //    colors.Add(c1); // v1
    //    colors.Add(c2); // v2
    //    colors.Add(c3); // v3
    //    colors.Add(c4); // v4
    //}
    void AddQuadColor(Color c1, Color c2) {
        colors.Add(c1); // v1
        colors.Add(c1); // v2
        colors.Add(c2); // bridge v3
        colors.Add(c2); // bridge v4
    }
}

