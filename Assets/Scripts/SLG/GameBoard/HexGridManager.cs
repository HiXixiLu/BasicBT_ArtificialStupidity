using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Xixi: 
 * 2019/8 月初建立
 * 2019/8/14  好像这个类在慢慢变成一个全局的 Controller 类啊哈哈
 */
public class HexGridManager : MonoBehaviour
{
    [SerializeField] public int width = 10;
    [SerializeField] public int height = 10;

    public HexCellMesh cellMeshPrefab;
    public HexCellMesh[] cells; // TODO: 采用什么数据结构才能合理地管理各个棋格呢？

    HexCellMesh lastSelected, lastHover;
    List<HexCellMesh> selectedNeighbors = new List<HexCellMesh>();
    List<HexCellMesh> HoveredNeighbors = new List<HexCellMesh>();

    public CharacterSpawner cSpawner;

    private void Awake()
    {
        cells = new HexCellMesh[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }

    }
    private void Start()
    {
        cSpawner.InstantiateCharacters();
    }


    private void CreateCell(int x, int z, int i) {
        Vector3 position;

        position.x = (x + z * 0.5f - z / 2) * (HexCellMetric.innerRadius * 2f);  // -z/2 是为了调整奇数行的偏移
        position.y = 0.1f;  // 稍往上提升
        position.z = z * (HexCellMetric.outerRadius * 1.5f);

        cells[i] = Instantiate<HexCellMesh>(cellMeshPrefab);
        cells[i].transform.SetParent(this.transform, false);
        cells[i].transform.localPosition = position;
        cells[i].TriangulateCellMesh(position);
        cells[i].coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        //cells[i].coordinates = new Hex2DCoordinates(x, z);

        // 显示 坐标
        cells[i].uiText.text = cells[i].coordinates.ToStringAxisCoordinate();
        //cells[i].uiText.text = cells[i].coordinates.ToString();
    }

    private void HandleClick() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            if (hit.transform.name.Contains("HexCellMesh")) {
                recolorClickedChunk(hit.transform.GetComponent<HexCellMesh>());

                // 计算各格子与选中格的距离
                FindDistancesTo(hit.transform.GetComponent<HexCellMesh>());
            }
        }
    }
    private void HandleMouseHover() {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit))
        {
            if (hit.transform.name.Contains("HexCellMesh"))
            {
                recolorHoverChunk(hit.transform.GetComponent<HexCellMesh>());
            }
        }
    }

    /// <summary>
    /// 更新被点击棋格颜色
    /// </summary>
    /// <param name="cell"> 被点击的对象 </param>
    private void recolorClickedHex(HexCellMesh cell)
    {
        if (lastSelected != null)
        {
            lastSelected.HandleEvent(HexCellStatusEvent.RESET);
        }

        cell.HandleEvent(HexCellStatusEvent.BE_SELECTED);
        lastSelected = cell;
    }
    /// <summary>
    /// 更新鼠标悬停棋格的颜色
    /// </summary>
    /// <param name="cell"></param>
    private void moniterMouseHover(HexCellMesh cell)
    {
        if (lastHover != null)
        {
            if (lastSelected == lastHover)
                lastHover.HandleEvent(HexCellStatusEvent.BE_SELECTED);
            else
                lastHover.HandleEvent(HexCellStatusEvent.RESET_HOVER);
        }

        cell.HandleEvent(HexCellStatusEvent.MOUSE_HOVER);
        lastHover = cell;
    }

    /// <summary>
    /// 给一定范围内的 Hex 上色
    /// </summary>
    /// <param name="cell">被选中的棋格</param>
    /// <param name="range"> 距离范围 </param>
    private void recolorClickedChunk(HexCellMesh cell)
    {
        foreach (HexCellMesh c in selectedNeighbors)
        {
            c.HandleEvent(HexCellStatusEvent.RESET_COLOR);
        }

        recolorClickedHex(cell);

        if (cell.Occupant == null)
        {
            return;
        }
        else {
            selectedNeighbors = findNeighbors(cell.coordinates, cell.Occupant.movementScale);
            foreach (HexCellMesh c in selectedNeighbors)
            {
                c.HandleEvent(HexCellStatusEvent.SHOW_MOVEMENT_SCALE);
            }
        }

    }
    private void recolorHoverChunk(HexCellMesh cell, int range = 0) {

        foreach (HexCellMesh c in HoveredNeighbors) {
            c.HandleEvent(HexCellStatusEvent.RESET_COLOR);
        }

        moniterMouseHover(cell);
 
        HoveredNeighbors = findNeighbors(cell.coordinates, range);
        foreach (HexCellMesh c in HoveredNeighbors)
        {
            c.HandleEvent(HexCellStatusEvent.SHOW_GUNSHOT_RANGE);
        }
    }

    /// <summary>
    /// range = 3, 则寻找距离3格以内的邻居
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    private List<HexCellMesh> findNeighbors(Hex2DCoordinates pos, int range) {
        List<HexCellMesh> neighbors = new List<HexCellMesh>();
        // 与 pos 同一行
        for (int i = 1; i <= range; i++) {
            int idx = 0;
            if (pos.X + i < width) {
                idx = pos.X + i + pos.Z * width;
                neighbors.Add(cells[idx]);
            }
            if (pos.X - i >= 0) {
                idx = pos.X - i + pos.Z * width;
                neighbors.Add(cells[idx]);
            }            
        }

        // range 内的 pos 上下行
        for (int i = 1; i <= range; i++) {
            int HexNum = 2 * range + 1 - i;
            int xLeft, xRight, curZ;

            if (pos.Z % 2 == 0)
            {
                xLeft = (pos.X - HexNum / 2);
                xRight = (xLeft + HexNum - 1);         
            }
            else
            {
                xRight = (pos.X + HexNum / 2);
                xLeft = xRight - HexNum + 1;
            }

            xLeft = xLeft >= 0 ? xLeft : 0;
            xRight = xRight < width ? xRight : width;

            // 上半
            curZ = (pos.Z + i);
            if (curZ < height)
            {
                for (int j = xLeft; j <= xRight; j++)
                {
                    int idx = j + curZ * width;
                    neighbors.Add(cells[idx]);
                }
            }
            // 下半
            curZ = pos.Z - i;
            if (curZ >= 0)
            {
                for (int j = xLeft; j <= xRight; j++)
                {
                    int idx = j + curZ * width;
                    neighbors.Add(cells[idx]);
                }
            }


        }
        return neighbors;
    }
    private List<HexCellMesh> findNeighbors(HexCoordinates pos, int range)
    {
        // 需十分注意 —— HexCoordinates中的 x 坐标是经由真正的行偏移值 (x - z/2)运算 处理过的
        List<HexCellMesh> neighbors = new List<HexCellMesh>();

        // 与 pos 同一行
        for (int i = 1; i <= range; i++)
        {
            int idx = 0;
            if (pos.X + pos.Z / 2 + i < width)
            {
                idx = pos.X  + pos.Z/2 + i + pos.Z * width;
                neighbors.Add(cells[idx]);
            }
            if (pos.X + pos.Z / 2 - i >= 0)
            {
                idx = pos.X + pos.Z / 2 - i + pos.Z * width;
                neighbors.Add(cells[idx]);
            }
        }

        // range 内的 pos 上下行
        for (int i = 1; i <= range; i++)
        {
            int HexNum = 2 * range + 1 - i;
            int xLeft, xRight, curZ;

            if (pos.Z % 2 == 0)
            {
                xLeft = (pos.X + pos.Z / 2 - HexNum / 2);   // xLeft 变成了数组中实际的行偏移值
                xRight = (xLeft + HexNum - 1);
            }
            else
            {
                xRight = (pos.X + pos.Z / 2 + HexNum / 2);
                xLeft = xRight - HexNum + 1;
            }

            xLeft = xLeft >= 0 ? xLeft : 0;
            xRight = xRight < width ? xRight : width-1;

            // 上半
            curZ = (pos.Z + i);
            if (curZ < height)
            {
                for (int j = xLeft; j <= xRight; j++)
                {
                    int idx = j + curZ * width;
                    neighbors.Add(cells[idx]);
                }
            }
            // 下半
            curZ = pos.Z - i;
            if (curZ >= 0)
            {
                for (int j = xLeft; j <= xRight; j++)
                {
                    int idx = j + curZ * width;
                    neighbors.Add(cells[idx]);
                }
            }


        }
        return neighbors;
    }

    private void FindDistancesTo(HexCellMesh cell)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = cell.coordinates.DistanceTo(cells[i].coordinates);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        HandleMouseHover();
    }
}
