using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* OhiraPlus模型仅仅是 Citizen在用，最好不要有无关耦合 */
public class OhiraPlusModel
{
    List<HexCellMesh> cWaited = new List<HexCellMesh>();
    int msgCount = 0;   //移动是否完成的信号

    private OhiraPlusModel() {}
    private static OhiraPlusModel _instance;
    public static OhiraPlusModel Instance() {
        if(_instance != null)
            return _instance;
        else{
            _instance = new OhiraPlusModel();
            return _instance;
        }
    }

    public void resetMsgCount() {
        msgCount = 0;
    }
    

    // 得到待移动列表
    public void GetCitizens(ref List<HexCellMesh> Refugees) {
        cWaited = Refugees;
    }

    // 选择移动终点 —— 暂时没能做到动画所需的计算
    public void FindMovementDestination() {

        foreach (HexCellMesh c in cWaited) {
            // TODO: 逐个寻找的移动
        }


    }

    // 用于传递的委托函数
    public void MovementCompletionCallback() {
        msgCount += 1;
    }

    // 阻塞式判断
    bool IsRunningCompleted()
    {
        bool completed = false;
        while (msgCount < cWaited.Count)
        {
            if (msgCount == cWaited.Count)
                completed = true;
        }
        return completed;
    }

}
