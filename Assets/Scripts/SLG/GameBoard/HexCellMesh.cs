using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public static class HexCellMetric {
    public static float outerRadius = 5f;  // set outerRadius as 10f cause the default size of a plane component is 10.
    public static float innerRadius = outerRadius * Mathf.Sqrt(3) / 2;

    // 顶点朝上的排列方式
    public static Vector3[] corners = {
        new Vector3(0f, 0.1f, outerRadius),
        new Vector3(innerRadius, 0.1f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0.1f, -0.5f * outerRadius),
        new Vector3(0f, 0.1f, -outerRadius),
        new Vector3(-innerRadius, 0.1f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0.1f, 0.5f * outerRadius)
    };
}

/// <summary>
/// 六边形的棋格，单独一张Mesh
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexCellMesh : MonoBehaviour
{
    public HexCoordinates coordinates;  //记录立方坐标
    //public Hex2DCoordinates coordinates;    // 仅记录索引下标

    public Text uiText;   //test only
    public GameObject selectedHighlight;    //test only

    Mesh cellMesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;

    [SerializeField] HexCellMesh[] neighbors;

    MeshCollider meshCollider;

    [SerializeField]
    public Color seletedColor, defaultColor, hoverColor, gunshotColor, movementColor;

    public Vector3 center;

    public bool isAvailable = false;
    public bool isSelected = false;


    // 寻路需要用的属性
    int distance;
    public int Distance { 
        get {
            return distance;
        }
        set {
            distance = value;
            UpdateDistanceLabel();
        }
    }
    HexCellMesh pathFrom;
    public HexCellMesh PathFrom {
        get {
            return pathFrom;
        }
        set {
            pathFrom = value;
        }
    }

    CharacterBase occupant;     // 保存占据该棋格的棋子
    public CharacterBase Occupant {
        get {
            return occupant;
        }
        set {
            occupant = value;
        }
    }

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = cellMesh = new Mesh();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        meshCollider = this.gameObject.AddComponent<MeshCollider>();
    }

    /// <summary>
    /// 渲染六边形 Mesh
    /// </summary>
    /// <param name="position"></param>
    public void TriangulateCellMesh(Vector3 position) {

        center = position;

        // 射线检测障碍物
        if (!hexCellisAvailable())
            return;

        cellMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        RegulateCellMesh(defaultColor);

        cellMesh.vertices = vertices.ToArray();
        cellMesh.triangles = triangles.ToArray();
        cellMesh.colors = colors.ToArray();

        cellMesh.RecalculateNormals();
        meshCollider.sharedMesh = cellMesh;
    }
    private void RegulateCellMesh(Color color) {
        for (int i = 0; i < 6; i++) {
            AddTriangle(
                Vector3.zero,
                HexCellMetric.corners[i],
                HexCellMetric.corners[(i + 1) % 6]
                );

            AddTriangleColor(color);
        }
    }
    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertextIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(vertextIndex);
        triangles.Add(vertextIndex + 1);
        triangles.Add(vertextIndex + 2);
    }
    private void AddTriangleColor(Color color) {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    // 改变颜色（暂时）
    private void TriangulateWithColor(Color color) {
        if (!isAvailable)
            return;

        cellMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        RegulateCellMesh(color);

        cellMesh.vertices = vertices.ToArray();
        cellMesh.triangles = triangles.ToArray();
        cellMesh.colors = colors.ToArray();

        cellMesh.RecalculateNormals();
        meshCollider.sharedMesh = cellMesh;
    }

    // 障碍射线检测
    private bool hexCellisAvailable() {
        int count = 0;
        if (cornerIsAvailable(center) == 1) {
            isAvailable = false;
            return false;
        }

        for (int i = 0; i < 6; i++) {
            Vector3 corner = center + HexCellMetric.corners[i];
            count += cornerIsAvailable(corner);
        }

        if (count > 0)
        {
            isAvailable = false;
            return false;
        }
        else {
            isAvailable = true;
            return true;
        }
    }
    private int cornerIsAvailable(Vector3 origin) {
        origin.y += 99f;
        Ray topDown = new Ray(origin, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(topDown, out hit)) {
            if (hit.transform.name.Contains("Board") || hit.transform.name.Contains("HexCellMesh"))
            {
                return 0;
            }
            else {
                return 1;   // 有占用
            }
        }
        return 0;
    }

    /// <summary>
    /// 相邻棋格设置邻接表存储
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public HexCellMesh GetNeighbor(HexDirections direction)
    {
        return neighbors[(int)direction];
    }
    public void SetNeighbors(HexDirections direction, HexCellMesh cell)
    {
        neighbors[(int)direction] = cell;
        // 注意： 相邻关系是相互的
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    /// <summary>
    /// 处理棋格状态变化
    /// </summary>
    /// <param name="e"> 枚举事件 </param>
    public void HandleEvent(HexCellStatusEvent e) {
        switch (e) {
            case HexCellStatusEvent.BE_SELECTED:
                handleBeSelected();
                break;

            case HexCellStatusEvent.MOUSE_HOVER:
                handleMouseHover();
                break;

            case HexCellStatusEvent.RESET:
                handleReset();
                break;

            case HexCellStatusEvent.RESET_COLOR:
                handleResetColor();
                break;

            case HexCellStatusEvent.RESET_HOVER:
                handleResetHover();
                break;

            case HexCellStatusEvent.SHOW_GUNSHOT_RANGE:
                handleShowGunshotRange();
                break;

            case HexCellStatusEvent.SHOW_MOVEMENT_SCALE:
                handleShowMovementScale();
                break;

            case HexCellStatusEvent.RESET_PATH_HEX:
                handleResetPath();
                break;

            case HexCellStatusEvent.SHOW_PATH_HEX:
                handleShowPathHex();
                break;

            default:
                handleReset();
                break;
        }
    }
    void handleBeSelected() {
        selectedHighlight.SetActive(true);
        selectedHighlight.GetComponent<Image>().color = seletedColor;
        isSelected = true;
    }
    void handleMouseHover() {
        //TriangulateWithColor(hoverColor);
        selectedHighlight.SetActive(true);
    }
    void handleReset() {
        TriangulateWithColor(defaultColor);
        selectedHighlight.GetComponent<Image>().color = defaultColor;
        selectedHighlight.SetActive(false);
        isSelected = false;
    }
    void handleResetColor() {
        TriangulateWithColor(defaultColor);
    }

    void UpdateDistanceLabel() {
        if(isAvailable)
            uiText.text = distance.ToString();
    }
    void handleResetHover() {
        selectedHighlight.SetActive(false);
    }
    void handleShowGunshotRange() {
        TriangulateWithColor(gunshotColor);
    }
    void handleShowMovementScale() {
        TriangulateWithColor(movementColor);
    }
    void handleResetPath() {
        selectedHighlight.GetComponent<Image>().color = defaultColor;
        selectedHighlight.SetActive(false);
    }
    void handleShowPathHex() {
        selectedHighlight.GetComponent<Image>().color = movementColor;
        selectedHighlight.SetActive(true);
    }
}


public enum HexCellStatusEvent {
    BE_SELECTED,
    RESET,
    RESET_COLOR,
    RESET_HOVER,
    MOUSE_HOVER,
    SHOW_GUNSHOT_RANGE,
    SHOW_MOVEMENT_SCALE,
    RESET_PATH_HEX,
    SHOW_PATH_HEX
}
