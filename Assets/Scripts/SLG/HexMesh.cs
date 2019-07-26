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
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();

        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }
    // 在确定的世界坐标下另行渲染三角形
    void Triangulate(HexCell cell) {
        Vector3 center = cell.transform.localPosition;
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(center, center + HexMetrics.corners[i], center + HexMetrics.corners[(i+1)%6]);
            AddTriangleColor(cell.color);
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
}

