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

    private HexCellStatus status = HexCellStatus.UNAVAILABLE;
    private HexCellStatus lastStatus;

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
    // 启发式函数所用的估计值
    public int SearchHeuristic { get; set; }
    public int SearchPriority {
        get {
            return distance + SearchHeuristic;
        }
    }
    public HexCellMesh NextWithSamePriority { get; set; }

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
    public bool canbeDestination() {
        return status != HexCellStatus.UNAVAILABLE && occupant == null;
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
        else
            status = HexCellStatus.AVAILABLE;

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
        if (status == HexCellStatus.UNAVAILABLE)
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
            return false;
        }

        for (int i = 0; i < 6; i++) {
            Vector3 corner = center + HexCellMetric.corners[i];
            count += cornerIsAvailable(corner);
        }

        if (count > 0)
        {
            return false;
        }
        else {
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
                return 1;
            }
        }
        return 0;
    }

    public void ResetOccupant() {
        TransferToStatus(HexCellStatus.AVAILABLE);
        occupant = null;
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
        if (cell.status == HexCellStatus.AVAILABLE) {
            neighbors[(int)direction] = cell;
            // 注意： 相邻关系是相互的
            cell.neighbors[(int)direction.Opposite()] = this;
        }
    }

    /// <summary>
    /// 处理棋格状态变化 —— 一个用枚举实现的有限状态机
    /// 这里是状态转移函数
    /// </summary>
    /// <param name="e"> 枚举事件 </param>
    public void TransferToStatus(HexCellStatus e) {
        switch (e) {
            case HexCellStatus.AVAILABLE:
                enterAvailableStatus();
                break;

            case HexCellStatus.BE_ATTACKED:
                enterAttackedStatus();
                break;

            case HexCellStatus.BE_RESCUED:
                enterRescuedStatus();
                break;

            case HexCellStatus.IN_GUNSHOT:
                enterGunshotStatus();
                break;

            case HexCellStatus.IN_MOVEMENT_RANGE:
                enterMovementRangeStatus();
                break;

            case HexCellStatus.OCCUPIED_AND_SELECTED:
                enterOSStatus();
                break;

            case HexCellStatus.OCCUPIED_AND_UNSELECTED:
                enterOUSStatus();
                break;

            case HexCellStatus.UNOCCUPIED_AND_SELETED:
                enterUOSStatus();
                break;

            case HexCellStatus.PATH_HIGHLIGHT:
                enterPathHighlightStatus();
                break;

            case HexCellStatus.UNAVAILABLE:
                enterUnavailableStatus();
                break;

            case HexCellStatus.DESTINATION_HOVER:
                enterDestinationHoverStatus();
                break;

            default:
                enterAvailableStatus();
                break;
        }
    }
    
    // 一系列状态事件函数 —— 每个状态下的属性改变也应该是无交集的
    void enterAvailableStatus() {
        TriangulateWithColor(defaultColor);
        selectedHighlight.GetComponent<Image>().color = defaultColor;
        selectedHighlight.SetActive(false);

        lastStatus = status = HexCellStatus.AVAILABLE;
    }
    void enterAttackedStatus() { }
    void enterRescuedStatus() { }
    void enterGunshotStatus() {
        if (status == HexCellStatus.UNAVAILABLE)
            return;

        TriangulateWithColor(gunshotColor);
        selectedHighlight.SetActive(false);
        selectedHighlight.GetComponent<Image>().color = defaultColor;

        if (status == HexCellStatus.IN_MOVEMENT_RANGE 
            || status == HexCellStatus.OCCUPIED_AND_UNSELECTED 
            || status == HexCellStatus.OCCUPIED_AND_SELECTED 
            || status == HexCellStatus.AVAILABLE) {

            lastStatus = status;
        }
        status = HexCellStatus.IN_GUNSHOT;

    }
    void enterMovementRangeStatus() {
        TriangulateWithColor(movementColor);
        selectedHighlight.SetActive(false);
        selectedHighlight.GetComponent<Image>().color = defaultColor;

        lastStatus = status = HexCellStatus.IN_MOVEMENT_RANGE;

    }
    void enterOSStatus() {
        TriangulateWithColor(defaultColor);
        selectedHighlight.SetActive(true);
        selectedHighlight.GetComponent<Image>().color = seletedColor;

        lastStatus = status = HexCellStatus.OCCUPIED_AND_SELECTED;
    }
    void enterOUSStatus() {
        TriangulateWithColor(defaultColor);
        selectedHighlight.SetActive(false);
        selectedHighlight.GetComponent<Image>().color = defaultColor;

        lastStatus = status = HexCellStatus.OCCUPIED_AND_UNSELECTED;
    }
    void enterUOSStatus() {
        TriangulateWithColor(defaultColor);
        selectedHighlight.SetActive(true);
        selectedHighlight.GetComponent<Image>().color = seletedColor;

        lastStatus = status = HexCellStatus.UNOCCUPIED_AND_SELETED;
    }
    void enterPathHighlightStatus()
    {
        TriangulateWithColor(defaultColor);
        selectedHighlight.SetActive(true);
        selectedHighlight.GetComponent<Image>().color = defaultColor;

        lastStatus = status = HexCellStatus.PATH_HIGHLIGHT;
    }
    void enterUnavailableStatus() {
        lastStatus = status = HexCellStatus.UNAVAILABLE;

        selectedHighlight.SetActive(false);
        selectedHighlight.GetComponent<Image>().color = defaultColor;
    }
    void enterDestinationHoverStatus() {
        TriangulateWithColor(defaultColor);
        selectedHighlight.SetActive(true);
        selectedHighlight.GetComponent<Image>().color = defaultColor;

        if (status == HexCellStatus.IN_MOVEMENT_RANGE
            || status == HexCellStatus.OCCUPIED_AND_UNSELECTED
            || status == HexCellStatus.OCCUPIED_AND_SELECTED
            || status == HexCellStatus.AVAILABLE)
        {

            lastStatus = status;
        }

        status = HexCellStatus.DESTINATION_HOVER;   // 8/18：不改变lastStatus，因为悬停棋格周围一定是处于 IN_GUNSHOT状态的棋格
    }

    public void GoBackLastStatus() {
        if(lastStatus != HexCellStatus.UNAVAILABLE)
            TransferToStatus(lastStatus);
    }

    public void UpdateDistanceLabel() {
        if(status != HexCellStatus.UNAVAILABLE)
            uiText.text = distance.ToString();
    }
}

// 枚举实现的有限状态机
public enum HexCellStatus {
    UNAVAILABLE,    // 被障碍或死去棋子占据的不可用状态
    AVAILABLE,      // 单纯的空闲态
    MOUSE_LEAVED,    // 从鼠标悬停离开
    OCCUPIED_AND_SELECTED,      // 棋格上有棋子且被选中
    OCCUPIED_AND_UNSELECTED,
    UNOCCUPIED_AND_SELETED,
    PATH_HIGHLIGHT,     // 标识途径路径的高亮态
    IN_MOVEMENT_RANGE,  // 处于被选中棋子的移动范围内
    IN_GUNSHOT,         // 处于被选中棋子的攻击范围内
    BE_ATTACKED,        // 棋格上的棋子被攻击
    BE_RESCUED,          // 棋格上的棋子被救援
    DESTINATION_HOVER    // 有棋子被选中时的悬空目标点
}
