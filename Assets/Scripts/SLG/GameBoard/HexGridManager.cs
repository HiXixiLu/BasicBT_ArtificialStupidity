using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    [SerializeField] public int width = 10;
    [SerializeField] public int height = 10;

    public HexCellMesh cellMeshPrefab;
    public HexCellMesh[] cells;

    HexCellMesh lastSeleted, lastHover;

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

        // TODO： 文字UI
    }

    /// <summary>
    /// 更新被点击棋格颜色
    /// </summary>
    /// <param name="cell"> 被点击的对象 </param>
    private void refreshHexBoard(HexCellMesh cell) {
        if (lastSeleted != null) {
            lastSeleted.HandleEvent(HexCellStatusEvent.RESET);
        }

        cell.HandleEvent(HexCellStatusEvent.BE_SELECTED);
        lastSeleted = cell;
    }
    /// <summary>
    /// 更新鼠标悬停棋格的颜色
    /// </summary>
    /// <param name="cell"></param>
    private void moniterMouseOverHexBoard(HexCellMesh cell) {
        if (lastHover != null) {
            if(lastSeleted == lastHover)
                lastHover.HandleEvent(HexCellStatusEvent.BE_SELECTED);
            else
                lastHover.HandleEvent(HexCellStatusEvent.RESET);
        }            

        cell.HandleEvent(HexCellStatusEvent.MOUSE_HOVER);
        lastHover = cell;
    }

    private void HandleClick() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            if (hit.transform.name.Contains("HexCellMesh")) {         
                refreshHexBoard(hit.transform.GetComponent<HexCellMesh>());
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
                moniterMouseOverHexBoard(hit.transform.GetComponent<HexCellMesh>());
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            HandleClick();
        }

        HandleMouseHover();
    }
}
