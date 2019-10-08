using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenRoundState : GameStates
{
    private CitizenRoundState instance;
    public CitizenRoundState Instance {
        get {
            return instance;
        }
    }

    private List<CitizenDemo> citizens;
    private GameStateContext context;

    public CitizenRoundState(GameStateContext c) {
        if (instance != null)
            return;

        instance = this;
        context = c;
        citizens = context.grid.cSpawner.GetCitizens();
       
    }

    public override void onEntered()
    {
        context.grid.TransferToCitizenRound();  //权宜处理
        Debug.Log("Citizen Round Now !");

        int movingUnits = 0;

        // 接收网络的请求以更新 citizen 的位置
        // OHIRA模型 逃窜
        //runaway();

        // 阻塞 —— 因为暂时没实现 OHIRA 模型而注掉
        //while (movingUnits < citizens.Count) {
        //    movingUnits = 0;
        //    foreach (CitizenDemo c in citizens)
        //    {
        //        if (c.MovementDone == true)
        //            movingUnits += 1;
        //    }
        //}

        changeState();
    }

    /// <summary>
    /// OHIRA逃窜模型 —— 远离敌人、靠近避难所的逃窜方向
    /// </summary>
    void runaway() {
        Debug.Log("Running...");
        
    }

    public override void onExit()
    {
        //StopAllCoroutines();
    }

    public override void changeState()
    {
        context.changeState();
    }
}
