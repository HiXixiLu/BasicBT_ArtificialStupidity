using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Date: 2019/9/5
 * Author: Xixi
 * Desc：用于处理用户输入的类
 */
public class UserInteractionEvent : MonoBehaviour
{
    [SerializeField]
    HexGridManager m_observerGrid;

    private CharacterBase CSelected;
    private void updateCSelected(CharacterBase c = null) {
        CSelected = null;
        CSelected = c;
    }

    // 日后做成网络模块后，一旦选定某一方就不能操作另一方
    private void onUserClick() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition); 
        RaycastHit hit;
        Debug.Log("onUserClick..");
        if (Physics.Raycast(inputRay, out hit)) {

            // 单点警察
            if (hit.transform.name.Contains("Police")) {

                if (m_observerGrid.GetCurrentGameState() == StateId.GUARD)
                {
                    CharacterBase avatar = hit.transform.GetComponent<PoliceDemo>();
                    updateCSelected(avatar);
                    m_observerGrid.onSingleClickToCharactor(avatar);
                }
                else {
                    updateCSelected();
                }
                return;
            }

            // 点击恐怖分子
            if (hit.transform.name.Contains("Enemy")) {
                if (m_observerGrid.GetCurrentGameState() == StateId.DESTOYER)
                {
                    CharacterBase avatar = hit.transform.GetComponent<EnemyDemo>();
                    updateCSelected(avatar);
                    m_observerGrid.onSingleClickToCharactor(avatar);
                }
                else {
                    updateCSelected();
                }
                return;
            }

            // 点击格子
            if (hit.transform.name.Contains("HexCellMesh")) {
                if (CSelected != null)
                {
                    HexCellMesh from = CSelected.Occupation;
                    HexCellMesh to = hit.transform.GetComponent<HexCellMesh>();

                    if (to != from)
                    {
                        m_observerGrid.onClickToSearch(from, to);
                        updateCSelected();
                    }
                    //else {
                    //    CharacterBase avatar = null;
                    //    if (m_observerGrid.GetCurrentGameState() == StateId.GUARD)
                    //        avatar = hit.transform.GetComponent<PoliceDemo>();
                    //    else if (m_observerGrid.GetCurrentGameState() == StateId.DESTOYER)
                    //        avatar = hit.transform.GetComponent<EnemyDemo>();
                    //    else if (m_observerGrid.GetCurrentGameState() == StateId.CITIZENS)
                    //        return;

                    //    updateCSelected(avatar);
                    //    m_observerGrid.onSingleClickToCharactor(avatar);
                    //}
                   
                }
                else {
                    updateCSelected();
                    m_observerGrid.onSingleClickToAvailableHex( hit.transform.GetComponent<HexCellMesh>() );
                    
                }
                return;
            }
        }
    }
    private void onUserHover() {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            onUserClick();
        }
        onUserHover();
    }
}
