using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    // 棋盘的长宽
    [SerializeField] public int width = 6;
    [SerializeField] public int height = 6;

    public HexCell cellPrefab;
    public HexCell[] cells;
    [SerializeField] public Text HexCellText;
    Canvas gridCanvas;
    HexMesh hexMesh;

    public Color defaultColor = Color.white;
    //public Color touchedColor = Color.magenta;

    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[height * width];
        for (int z = 0, i = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                CreateCell(x, z, i++);
            }
        }
    }
    private void Start()
    {
        hexMesh.Triangulate(cells);
    }

    void CreateCell(int x, int z, int i) {
        Vector3 position;
        position.x = (x + z*0.5f - z/2) * (HexMetrics.innerRadius * 2f);    //仅奇数行偏移（注意 z/2 为整数除法）
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);   // z offset

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(this.transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;

        // 自东向西添加东西方向的邻居
        if (x > 0) {
            cell.SetNeighbors(HexDirections.W, cells[i - 1]);
        }

        // 自北向南添加邻居 —— 由于Z方向的之字布局，偶数行与奇数行要区别对待
        if (z > 0) {
            if ((z & 1) == 0)
            { 
                // even rows
                cell.SetNeighbors(HexDirections.SE, cells[i - width]);
                // SE neighbors of even rows
                if (x > 0) {
                    cell.SetNeighbors(HexDirections.SW, cells[i - width - 1]);
                }
            }
            else {
                // odd rows
                cell.SetNeighbors(HexDirections.SW, cells[i - width]);
                // SE neighbors of odd row
                if (x < width - 1) {
                    cell.SetNeighbors(HexDirections.SE, cells[i - width + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(HexCellText);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0)) {  //每帧调用的 Input类 API注意甄别检测区别
    //        HandleInput();
    //    }
    //}
    //void HandleInput() {
    //    Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);   // Ray 是射线，两种属性可诠释（origin, direction）
    //    RaycastHit hit;
    //    if (Physics.Raycast(inputRay, out hit)) {
    //        TouchCell(hit.point);
    //    }
    //}
    //void TouchCell(Vector3 position)
    //{
    //    position = transform.InverseTransformPoint(position);   // transform position from world space to local space.
    //    HexCoordinates coordinates = HexCoordinates.FromPosition(position);
    //    Debug.Log("touched at " + coordinates.ToString());
    //    int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;  // 往z轴正向的格子，x坐标有偏移需要修正
    //    HexCell cell = cells[index];
    //    cell.color = touchedColor;
    //    hexMesh.Triangulate(cells);
    //}

    // 变色改从外部传入
    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);   // transform position from world space to local space.
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at " + coordinates.ToString());
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;  // 往z轴正向的格子，x坐标有偏移需要修正
        HexCell cell = cells[index];
        cell.color = color;
        hexMesh.Triangulate(cells);
    }
}
