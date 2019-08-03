using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; //用于管理UI交互事件

public class HexMapEditor : MonoBehaviour
{
    [SerializeField]
    public Color[] colors;
    public HexGrid hexGrid;     //Connect the hex grid
    private Color activeColor;
    private int activeElevation = 0;
    public Slider elevationSlider;

    private void Awake()
    {
        SelectColor(0);
        elevationSlider.onValueChanged.AddListener(delegate (float elevation) {
            activeElevation = (int)elevation;
        });
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            if (hit.transform.name.Contains("HexMesh"))
            {
                EditCell(hexGrid.GetCell(hit.point));
            }

            //else if (hit.transform.name.Contains("Police")) {
            //    // TODO: 真正的选中函数，以控制角色行为
            //    Debug.Log("Police is selected.");
            //}
        }
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }
    //public void SetElevation(float elevation) {
    //    activeElevation = (int)elevation;
    //}

    void EditCell(HexCell cell) {
        cell.color = activeColor;
        cell.Elevation = activeElevation;
        hexGrid.Refresh();
    }
}
