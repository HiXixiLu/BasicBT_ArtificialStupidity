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
    [SerializeField]
    CharactorPanel m_charactorPanel;

    private CharacterBase CSelected;
    private void updateCSelected(CharacterBase c = null) {
        CSelected = null;
        CSelected = c;
    }

    // TODO: 日后做成网络模块后，一旦选定某一方就不能操作另一方
    //private void onUserClick() {
    //    Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition); 
    //    RaycastHit hit;
    //    Debug.Log("onUserClick..");
    //    if (Physics.Raycast(inputRay, out hit)) {

    //        // 单点警察
    //        if (hit.transform.name.Contains("Police")) {

    //            if (m_observerGrid.GetCurrentGameState() == StateId.GUARD)
    //            {
    //                CharacterBase avatar = hit.transform.GetComponent<PoliceDemo>();
    //                updateCSelected(avatar);
    //                m_observerGrid.onSingleClickToCharactor(avatar);

    //                m_charactorPanel.ShowPanel(avatar);
    //            }
    //            else {
    //                updateCSelected();
    //            }
    //            return;
    //        }

    //        // 点击恐怖分子
    //        if (hit.transform.name.Contains("Enemy")) {
    //            if (m_observerGrid.GetCurrentGameState() == StateId.DESTOYER)
    //            {
    //                CharacterBase avatar = hit.transform.GetComponent<EnemyDemo>();
    //                updateCSelected(avatar);
    //                m_observerGrid.onSingleClickToCharactor(avatar);

    //                m_charactorPanel.ShowPanel(avatar);
    //            }
    //            else {
    //                updateCSelected();
    //            }
    //            return;
    //        }

    //        // 点击格子
    //        if (hit.transform.name.Contains("HexCellMesh")) {
    //            if (CSelected != null)
    //            {
    //                HexCellMesh from = CSelected.Occupation;
    //                HexCellMesh to = hit.transform.GetComponent<HexCellMesh>();

    //                if (to != from)
    //                {
    //                    m_observerGrid.onClickToSearch(from, to);
    //                    updateCSelected();
    //                }
    //                //else {
    //                //    CharacterBase avatar = null;
    //                //    if (m_observerGrid.GetCurrentGameState() == StateId.GUARD)
    //                //        avatar = hit.transform.GetComponent<PoliceDemo>();
    //                //    else if (m_observerGrid.GetCurrentGameState() == StateId.DESTOYER)
    //                //        avatar = hit.transform.GetComponent<EnemyDemo>();
    //                //    else if (m_observerGrid.GetCurrentGameState() == StateId.CITIZENS)
    //                //        return;

    //                //    updateCSelected(avatar);
    //                //    m_observerGrid.onSingleClickToCharactor(avatar);
    //                //}
                   
    //            }
    //            else {
    //                updateCSelected();
    //                m_observerGrid.onSingleClickToAvailableHex( hit.transform.GetComponent<HexCellMesh>() );
                    
    //            }
    //            return;
    //        }
    //    }
    //}
    private void OnUserClick() {
        StateId cur = m_observerGrid.GetCurrentGameState();
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.Log("onUserClick..");
        if (Physics.Raycast(inputRay, out hit)) {
            if (cur == StateId.CITIZENS)
            {
                return;
            }
            else if (cur == StateId.DESTOYER)
            {
                if (hit.transform.name.Contains("Enemy"))
                {
                    CharacterBase avatar = hit.transform.GetComponent<EnemyDemo>();
                    updateCSelected(avatar);
                    m_observerGrid.onSingleClickToCharactor(avatar);

                    m_charactorPanel.ShowPanel(avatar);
                }
                else if (hit.transform.name.Contains("HexCellMesh")) {
                    HexCellMeshClickHandler(ref hit);
                }
                else
                {
                    CharacterInteractionHandler(ref hit);
                    updateCSelected();
                }
                return;
            }
            else if (cur == StateId.GUARD)
            {
                if (hit.transform.name.Contains("Police"))
                {
                    CharacterBase avatar = hit.transform.GetComponent<PoliceDemo>();
                    updateCSelected(avatar);
                    m_observerGrid.onSingleClickToCharactor(avatar);

                    m_charactorPanel.ShowPanel(avatar);
                }
                else if (hit.transform.name.Contains("HexCellMesh"))
                {
                    HexCellMeshClickHandler(ref hit);
                }
                else {
                    CharacterInteractionHandler(ref hit);
                    updateCSelected();
                }
                return;
            }
        }

        

    }
    private void HexCellMeshClickHandler(ref RaycastHit hit) {
        if (CSelected != null) {
            HexCellMesh from = CSelected.Occupation;
            HexCellMesh to = hit.transform.GetComponent<HexCellMesh>();

            if (to != from)
            {
                m_observerGrid.onClickToSearch(from, to);
                updateCSelected();
            }
        }
    }
    /// <summary>
    /// 处理角色之间的互动行为 —— 目前只有进攻行为
    /// </summary>
    /// <param name="hit"></param>
    private void CharacterInteractionHandler(ref RaycastHit hit) {
        CharacterBase chaClicked = hit.transform.GetComponent<CharacterBase>();
        if (chaClicked != CSelected && CSelected != null)
        {
            // 1. 算 distance 2.确定行为
            // 暂时采用进攻方和被进攻方分别发送事件进行更新的方案
            // TODO: 这里又出现了大串 if-else 对， 最好从 UI事件 中解耦出去，但一下子没有想好
            int targetDistance = m_observerGrid.FindDijkstraDistance(CSelected.Occupation, chaClicked.Occupation);
            Debug.Log("所计算的Dijkstra距离 = " + targetDistance);
            if (m_observerGrid.GetCurrentGameState() == StateId.GUARD)
            {
                if (chaClicked.Name == ValueBoundary.destroyerName)
                {
                    //CSelected.attack(chaClicked, targetDistance);
                    CSelected.attack(chaClicked, targetDistance, (bool can, int damage) =>
                    {
                        Debug.Log("进入 AttackConditionCallback 回调");
                        if (can)
                        {
                            chaClicked.beAttacked(damage);
                            m_observerGrid.NortifyContextChangement();  // 通知
                            m_charactorPanel.UpdateCharactorUI(CSelected);  //更新角色面板
                        }
                        else {
                            return;
                        }
                    });
                }
            }
            else if (m_observerGrid.GetCurrentGameState() == StateId.DESTOYER)
            {
                if (chaClicked.Name == ValueBoundary.policeName || chaClicked.Name == ValueBoundary.citizenName)
                {
                    //CSelected.attack(chaClicked, targetDistance);
                    CSelected.attack(chaClicked, targetDistance, (bool can, int damage) =>
                    {
                        Debug.Log("进入 AttackConditionCallback 回调");
                        if (can)
                        {
                            chaClicked.beAttacked(damage);  // TODO: 计算范围伤害
                            m_observerGrid.NortifyContextChangement();  // 通知
                            m_charactorPanel.UpdateCharactorUI(CSelected);  //更新角色面板
                        }
                        else {
                            return;
                        }
                    });
                }
            }
            else
            {
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
            //onUserClick();
            OnUserClick();
        }
        onUserHover();
    }
}
