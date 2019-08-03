using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* HexGrid 是一个信息管理单元
 * 管理渲染单元：绑定HexMesh这一GameObject，用于管理渲染单元
 * 管理信息：保存着整个棋盘的长宽信息、保存信息单元数组 HexCell[]
 * 管理UI单元：关联着 Canvas 对象
 * 管理各种预制件的初始化： HexCell， _Prefab等
 */
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

    public PoliceDemo policePrefab;
    public CitizenDemo citizenPrefab;
    public EnemyDemo enemyPrefab;

    public Color defaultColor = Color.white;

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
        //CreatePolice(); //test only
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
        cell.uiRect = label.rectTransform;
    }

    // 由于选中格子的时候操作变多 —— 除了涂色，恐怕还有地形，今后还有移动 —— 则射线检测后改为返回被点击的格子
    public HexCell GetCell(Vector3 position) {
        position = transform.InverseTransformDirection(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at " + coordinates.ToString());
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        return cells[index];
    }
    // 产生地形后一定要重绘
    public void Refresh() {
        hexMesh.Triangulate(cells);
    }

    // 创建角色
    void CreatePolice() {
        PoliceDemo p = Instantiate<PoliceDemo>(policePrefab);
        Transform chtr = transform.Find("Characters");
        p.transform.SetParent(chtr);
    }
}
