using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Xixi: 
 * 2019/8 月初建立
 * 2019/8/28  状态类可以与 Manager 解耦吗
 * 2019/9/4  其实这个类更多的应该是管理和改变棋格类
 */
public class HexGridManager : MonoBehaviour
{
    [SerializeField] public int width = 10;
    [SerializeField] public int height = 10;

    public HexCellMesh cellMeshPrefab;
    [SerializeField]private HexCellMesh[] cells;
    public HexCellMesh[] Cells{
        get {
            return cells;
        }
    }

    HexCellMesh searchFromHex, selectedHex, lastHover;
    List<HexCellMesh> movementRange = new List<HexCellMesh>();
    List<HexCellMesh> HoveredNeighbors = new List<HexCellMesh>();
    List<HexCellMesh> pathHexes = new List<HexCellMesh>();


    public CharacterSpawner cSpawner;
    public StateMachineUI stateMachineUI;
    private GameStateContext gameStateContext;

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
        gameStateContext = new GameStateContext(this);  // 状态模式环境类初始化

    }

    public StateId GetCurrentGameState() {
        return gameStateContext.State;
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

        // 显示索引 坐标
        cells[i].uiText.text = cells[i].coordinates.ToString2DIndex();

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

    public void HandleClick() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {

            if (hit.transform.name.Contains("HexCellMesh")) {
                HexCellMesh cell = hit.transform.GetComponent<HexCellMesh>();

                // 选中警察棋子
                if (cell.Occupant != null && cell.Occupant.Name == CharacterLimits.policeName)
                {
                    ClearPathHexes();

                    if (searchFromHex != null) {
                        searchFromHex.TransferToStatus(HexCellStatus.OCCUPIED_AND_UNSELECTED);
                    }

                    searchFromHex = cell;

                    recolorClickedChunk(cell);
                }
                // 选中棋子后点击空闲格子
                if(searchFromHex != null && cell != searchFromHex)
                {
                    recolorClickedChunk(cell);

                    searchFromHex.TransferToStatus(HexCellStatus.OCCUPIED_AND_SELECTED);
                    MarkPath(searchFromHex, selectedHex);

                    ClearHoverNeighbors();
                    searchFromHex.TransferToStatus(HexCellStatus.OCCUPIED_AND_UNSELECTED);
                    searchFromHex = null;
                }
                // 未选中棋子时点击空闲格子
                else if (searchFromHex == null && cell.Occupant == null) {
                    onSingleClickToAvailableHex(cell);
                }
                // 选中棋子时点击敌方棋子
            }

        }
    }

    public void onSingleClickToAvailableHex(HexCellMesh cell) {
        ClearPathHexes();
        ClearMovementRange();
        ClearHoverNeighbors();

        if (cell.Occupant != null)
        {
            onSingleClickToCharactor(cell.Occupant);
        }
        else {
            recolorClickedHex(cell);
        }

    }
    public void onClickToSearch(HexCellMesh from, HexCellMesh to) {
        recolorClickedChunk(to);

        from.TransferToStatus(HexCellStatus.OCCUPIED_AND_SELECTED);
        MarkPath(from, to);

        ClearHoverNeighbors();
        from.TransferToStatus(HexCellStatus.OCCUPIED_AND_UNSELECTED);

    }
    public void onSingleClickToCharactor(CharacterBase avatar) {
        ClearPathHexes();
        if (GetCurrentGameState() == StateId.CITIZENS)
            return;
        else if ((GetCurrentGameState() == StateId.DESTOYER && avatar.Name == CharacterLimits.destroyerName)
            || (GetCurrentGameState() == StateId.GUARD && avatar.Name == CharacterLimits.policeName)) {

            recolorClickedChunk(avatar.Occupation);
            Debug.Log("Update hovered range from center: " + avatar.Occupation.coordinates.ToString2DIndex());
            recolorCharacterGunshot(avatar);
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
            selectedHex.TransferToStatus(HexCellStatus.AVAILABLE);
        }

        if (cell.Occupant != null)
            cell.TransferToStatus(HexCellStatus.UNOCCUPIED_AND_SELETED);
        else
            cell.TransferToStatus(HexCellStatus.OCCUPIED_AND_SELECTED);

        selectedHex = cell;
    }
    /// <summary>
    /// 更新鼠标悬停棋格的颜色
    /// </summary>
    /// <param name="cell"></param>
    //private void moniterMouseHover(HexCellMesh cell)
    //{
    //    if (lastHover != null && lastHover != cell) {
    //        lastHover.GoBackLastStatus();
    //    }
    //    if (searchFromHex != null && searchFromHex != cell) {
    //        cell.TransferToStatus(HexCellStatus.DESTINATION_HOVER);
    //    }
    //    lastHover = cell;

    //}

    /// <summary>
    /// 给一定范围内的 Hex 上色
    /// </summary>
    /// <param name="cell">被选中的棋格</param>
    /// <param name="range"> 距离范围 </param>
    private void recolorClickedChunk(HexCellMesh cell)
    {
        ClearHoverNeighbors();
        ClearMovementRange();

        recolorClickedHex(cell);
        movementRange = findMovementRange(cell);

        foreach (HexCellMesh c in movementRange)
        {
            c.TransferToStatus(HexCellStatus.IN_MOVEMENT_RANGE);
        }

    }
    private void recolorCharacterGunshot(CharacterBase avatar) {
        int range = avatar.Gunshot;
        ClearHoverNeighbors();

        HoveredNeighbors = findNeighbors(avatar.Occupation.coordinates, range);
        foreach (HexCellMesh c in HoveredNeighbors)
        {
            c.TransferToStatus(HexCellStatus.IN_GUNSHOT);
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
    /// <summary>
    /// 寻找某个格子上棋子的可移动范围 —— 与寻路算法实际上进行了重复的计算
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    List<HexCellMesh> findMovementRange(HexCellMesh cell) { 
        List<HexCellMesh> hexes = new List<HexCellMesh>();

        if (cell.Occupant == null)
            return hexes;

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
        }

        List<HexCellMesh> frontiers = new List<HexCellMesh>();
        cell.Distance = 0;

        frontiers.Add(cell);

        while (frontiers.Count > 0)
        {

            HexCellMesh current = frontiers[0];
            frontiers.RemoveAt(0);  // 出列

            for (HexDirections d = HexDirections.NE; d <= HexDirections.NW; d++)
            {
                HexCellMesh neighbor = current.GetNeighbor(d);
                if (neighbor == null)
                    continue;
                if (!neighbor.canbeDestination())
                    continue;

                // 若未被处理过
                if (neighbor.Distance == int.MaxValue)
                {
                    neighbor.Distance = current.Distance + 1;
                    neighbor.PathFrom = current;
                    frontiers.Add(neighbor);
                }
                // 若已处理过
                else if (current.Distance + 1 < neighbor.Distance)
                {
                    neighbor.Distance = current.Distance + 1;
                    neighbor.PathFrom = current;
                }

                neighbor.UpdateDistanceLabel();

                if (current.Distance <= cell.Occupant.MovementScale)
                {
                    hexes.Add(current);
                }
            }
        }

        return hexes;
    }

    int FindDistance(HexCellMesh from, HexCellMesh to) {
        return from.coordinates.DistanceTo(to.coordinates); //不考虑障碍的直线距离
    }

    /// <summary>
    /// 寻路算法
    /// </summary>
    /// <param name="fromCell"></param>
    /// <param name="toCell"></param>
    void MarkPath(HexCellMesh fromCell, HexCellMesh toCell) {
        StopAllCoroutines();
        StartCoroutine(SearchPath(fromCell, toCell));
    }
    // Dijkstra Algorithm
    IEnumerator SearchPath(HexCellMesh fromCell, HexCellMesh toCell) {
        ClearPathHexes();

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
        }
        yield return null;
        List<HexCellMesh> frontiers = new List<HexCellMesh>();
        fromCell.Distance = 0;

        toCell.TransferToStatus(HexCellStatus.PATH_HIGHLIGHT);
        pathHexes.Add(toCell);

        frontiers.Add(fromCell);

        while (frontiers.Count > 0) {

            //yield return null;  //方便观察

            HexCellMesh current = frontiers[0];
            frontiers.RemoveAt(0);  // 出列

            if (current == toCell)
            {
                current = current.PathFrom;
                // 显示路径
                while (current != fromCell)
                {
                    pathHexes.Add(current);
                    current.TransferToStatus(HexCellStatus.PATH_HIGHLIGHT);
                    current = current.PathFrom; //回溯
                }
                break;
            }

            for (HexDirections d = HexDirections.NE; d <= HexDirections.NW; d++) {
                HexCellMesh neighbor = current.GetNeighbor(d);
                if (neighbor == null)
                    continue;
                if (!neighbor.canbeDestination())
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

        // 标记结束后发送可以开始移动的通知：
        sendMovementOrder(fromCell, toCell);

    }
    void ClearPathHexes() {
        foreach (HexCellMesh c in pathHexes) {
            c.TransferToStatus(HexCellStatus.AVAILABLE);
        }
        pathHexes.Clear();
    }
    void ClearHoverNeighbors()
    {
        foreach (HexCellMesh c in HoveredNeighbors)
        {
            c.GoBackLastStatus();
        }
        HoveredNeighbors.Clear();
    }
    void ClearMovementRange() {
        foreach (HexCellMesh c in movementRange) {
            c.TransferToStatus(HexCellStatus.AVAILABLE);
        }
        movementRange.Clear();
    }

    void sendMovementOrder(HexCellMesh fromCell, HexCellMesh toCell) {
        if (fromCell.Occupant == null)
            return;
        
        if (toCell.Distance > fromCell.Occupant.MovementScale)
            return;

        fromCell.Occupant.moveByPath(ref pathHexes);
        fromCell.ResetOccupant();
    }

    /// <summary>
    /// 扫描场面 Destoyer 方位和 避难所方位，综合得出逃窜方向
    /// </summary>
    /// <returns></returns>
    HexDirections findRunningDirection() {
        Debug.Log("Scanning");
        // TODO: 这里需要综合考虑 destoyer 位置与 避难所位置得出最终的逃窜方向
        return HexDirections.SE;    //test only
    }
    /// <summary>
    /// 寻找特定方向的优先逃窜路径
    /// </summary>
    /// <returns></returns>
    public List<HexCellMesh> FindEscapePath() {
        HexDirections direction = findRunningDirection();
        // TODO: 寻找往某个方向逃窜的路径

        return new List<HexCellMesh>(); // test only
    }
    /// <summary>
    /// 寻找通往最近目标的路径
    /// </summary>
    /// <returns></returns>
    public List<HexCellMesh> FindNearTarget() {
        // TODO: 寻找通往目标的路径

        return new List<HexCellMesh>(); // test only
    }

    /// <summary>
    /// 状态机的 UI 可视化
    /// </summary>
    public void TransferToPoliceRound() {
        stateMachineUI.onPoliceRound();
    }
    public void TransferToDestroyerRound() {
        stateMachineUI.onDestroyerRound();
    }
    public void TransferToCitizenRound() {
        stateMachineUI.onCitizenRound();
    }
    public void ChangeActionsNum(int num) {
        stateMachineUI.SetActionsNum(num);
    }

}
