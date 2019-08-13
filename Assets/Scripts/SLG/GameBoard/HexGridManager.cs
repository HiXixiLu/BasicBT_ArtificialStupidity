using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    [SerializeField] public int width = 10;
    [SerializeField] public int height = 10;

    public HexCellMesh cellMeshPrefab;
    public HexCellMesh[] cells; // TODO: 采用什么数据结构才能合理地管理各个棋格呢？

    HexCellMesh lastSeleted, lastHover;
    List<HexCellMesh> selectedNeighbors;

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

        selectedNeighbors = new List<HexCellMesh>();
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
        //cells[i].coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cells[i].coordinates = new Hex2DCoordinates(x, z);

        // 显示 坐标
        //cells[i].cubeCoord.text = cells[i].coordinates.ToString();
        cells[i].coordinatesText.text = cells[i].coordinates.ToString();
    }

    private void HandleClick() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            if (hit.transform.name.Contains("HexCellMesh")) {
                recolorClickedHex(hit.transform.GetComponent<HexCellMesh>());
                //recolorClickedChunk(hit.transform.GetComponent<HexCellMesh>());
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
                //moniterMouseHover(hit.transform.GetComponent<HexCellMesh>());
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
        if (lastSeleted != null)
        {
            lastSeleted.HandleEvent(HexCellStatusEvent.RESET);
        }

        cell.HandleEvent(HexCellStatusEvent.BE_SELECTED);
        lastSeleted = cell;
    }
    /// <summary>
    /// 更新鼠标悬停棋格的颜色
    /// </summary>
    /// <param name="cell"></param>
    private void moniterMouseHover(HexCellMesh cell)
    {
        if (lastHover != null)
        {
            if (lastSeleted == lastHover)
                lastHover.HandleEvent(HexCellStatusEvent.BE_SELECTED);
            else
                lastHover.HandleEvent(HexCellStatusEvent.RESET);
        }

        cell.HandleEvent(HexCellStatusEvent.MOUSE_HOVER);
        lastHover = cell;
    }

    /// <summary>
    /// 给一定范围内的 Hex 上色
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="range">射程范围</param>
    private void recolorClickedChunk(HexCellMesh cell, int range = 3)
    {

        foreach (HexCellMesh c in selectedNeighbors)
        {
            c.HandleEvent(HexCellStatusEvent.RESET);
        }

        recolorClickedHex(cell);
        selectedNeighbors = findNeighbors(cell.coordinates, range);
        foreach (HexCellMesh c in selectedNeighbors)
        {
            c.HandleEvent(HexCellStatusEvent.GUNSHOT_RANGE);
        }
    }
    private void recolorHoverChunk(HexCellMesh cell, int range = 3) {

        foreach (HexCellMesh c in selectedNeighbors) {
            c.HandleEvent(HexCellStatusEvent.RESET);
        }

        moniterMouseHover(cell);
        selectedNeighbors = findNeighbors(cell.coordinates, range);
        foreach (HexCellMesh c in selectedNeighbors)
        {
            c.HandleEvent(HexCellStatusEvent.GUNSHOT_RANGE);
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
        for (int i = 1; i < range; i++) {
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
        for (int i = 1; i < range; i++) {
            int HexNum = 2 * range - 1 - i;
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            HandleClick();
        }

        HandleMouseHover();
    }
}
