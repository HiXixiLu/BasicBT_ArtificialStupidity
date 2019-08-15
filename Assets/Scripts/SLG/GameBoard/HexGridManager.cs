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

    HexCellMesh searchFromHex, selectedHex, lastHover;
    List<HexCellMesh> movementRange = new List<HexCellMesh>();
    List<HexCellMesh> HoveredNeighbors = new List<HexCellMesh>();
    List<HexCellMesh> pathHexes = new List<HexCellMesh>();

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

        // 自东向西添加东西方向的邻居
        if (x > 0)
        {
            cells[i].SetNeighbors(HexDirections.W, cells[i - 1]);
        }

        // 自北向南添加邻居 —— 由于Z方向的之字布局，偶数行与奇数行要区别对待
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                // even rows
                cells[i].SetNeighbors(HexDirections.SE, cells[i - width]);
                // SE neighbors of even rows
                if (x > 0)
                {
                    cells[i].SetNeighbors(HexDirections.SW, cells[i - width - 1]);
                }
            }
            else
            {
                // odd rows
                cells[i].SetNeighbors(HexDirections.SW, cells[i - width]);
                // SE neighbors of odd row
                if (x < width - 1)
                {
                    cells[i].SetNeighbors(HexDirections.SE, cells[i - width + 1]);
                }
            }
        }
    }

    private void HandleClick() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {

            if (hit.transform.name.Contains("HexCellMesh")) {
                HexCellMesh cell = hit.transform.GetComponent<HexCellMesh>();
                recolorClickedChunk(cell);

                // 计算各格子与选中格的距离
                if (cell.Occupant != null)
                {
                    if (searchFromHex != null) {
                        searchFromHex.HandleEvent(HexCellStatusEvent.RESET_PATH_HEX);
                        clearPathHexes();
                    }

                    searchFromHex = cell;
                }
                else if(cell != searchFromHex && searchFromHex != null){
                    searchFromHex.HandleEvent(HexCellStatusEvent.BE_SELECTED);
                    FindPath(searchFromHex, selectedHex);
                }
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
        if (selectedHex != null)
        {
            selectedHex.HandleEvent(HexCellStatusEvent.RESET);
        }

        cell.HandleEvent(HexCellStatusEvent.BE_SELECTED);
        selectedHex = cell;
    }
    /// <summary>
    /// 更新鼠标悬停棋格的颜色
    /// </summary>
    /// <param name="cell"></param>
    private void moniterMouseHover(HexCellMesh cell)
    {
        if (lastHover != null)
        {
            if (selectedHex == lastHover)
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
        foreach (HexCellMesh c in movementRange)
        {
            c.HandleEvent(HexCellStatusEvent.RESET_COLOR);
        }

        recolorClickedHex(cell);

        movementRange = findMovementRange(cell);
        foreach (HexCellMesh c in movementRange)
        {
            c.HandleEvent(HexCellStatusEvent.SHOW_MOVEMENT_SCALE);
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
    List<HexCellMesh> findMovementRange(HexCellMesh cell) {
        List<HexCellMesh> hexes = new List<HexCellMesh>();
        if (cell.Occupant != null)
        {
            return hexes;
        }
        else {
            // TODO: 寻找一定距离内的可移动格子
            return hexes;
        }

    }

    // test only
    //private void FindDistancesTo(HexCellMesh cell)
    //{
    //    for (int i = 0; i < cells.Length; i++)
    //    {
    //        cells[i].Distance = cell.coordinates.DistanceTo(cells[i].coordinates);
    //    }
    //}
    int FindDistance(HexCellMesh from, HexCellMesh to) {
        return from.coordinates.DistanceTo(to.coordinates); //不考虑障碍的直线距离
    }

    /// <summary>
    /// 寻路算法
    /// </summary>
    /// <param name="fromCell"></param>
    /// <param name="toCell"></param>
    void FindPath(HexCellMesh fromCell, HexCellMesh toCell) {
        StopAllCoroutines();
        StartCoroutine(SearchPath(fromCell, toCell));
    }
    // Dijkstra Algorithm
    IEnumerator SearchPath(HexCellMesh fromCell, HexCellMesh toCell) {
        clearPathHexes();

        for (int i = 0; i < cells.Length; i++) {
            cells[i].Distance = int.MaxValue;
        }
        yield return null;
        List<HexCellMesh> frontiers = new List<HexCellMesh>();
        fromCell.Distance = 0;
        toCell.HandleEvent(HexCellStatusEvent.BE_SELECTED);

        frontiers.Add(fromCell);

        while (frontiers.Count > 0) {

            yield return null;  //方便观察

            HexCellMesh current = frontiers[0];
            frontiers.RemoveAt(0);  // 出列

            if (current == toCell)
            {
                current = current.PathFrom;
                // 显示路径
                while (current != fromCell)
                {
                    pathHexes.Add(current);
                    current.HandleEvent(HexCellStatusEvent.SHOW_PATH_HEX);
                    current = current.PathFrom; //回溯
                }
                break;
            }

            for (HexDirections d = HexDirections.NE; d <= HexDirections.NW; d++) {
                HexCellMesh neighbor = current.GetNeighbor(d);
                if (neighbor == null)
                    continue;
                if (!neighbor.isAvailable || neighbor.Occupant != null)
                    continue;

                // 若未被处理过
                if (neighbor.Distance == int.MaxValue)
                {
                    neighbor.Distance = current.Distance + 1;
                    neighbor.PathFrom = current;
                    frontiers.Add(neighbor);
                }
                // 若已处理过
                else if (current.Distance + 1 < neighbor.Distance) {
                    neighbor.Distance = current.Distance + 1;
                    neighbor.PathFrom = current;
                }
            }
        }
    }
    void clearPathHexes() {
        foreach (HexCellMesh c in pathHexes) {
            c.HandleEvent(HexCellStatusEvent.RESET_PATH_HEX);
        }
        pathHexes.Clear();
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
