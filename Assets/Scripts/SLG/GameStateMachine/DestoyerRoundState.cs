using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyerRoundState : GameStates
{
    private DestoyerRoundState instance;
    public DestoyerRoundState Instance {
        get {
            return instance;
        }
    }

    private HexGridManager grid;
    private List<EnemyDemo> destroyers;
    private GameStateContext context;

    public DestoyerRoundState(HexGridManager h, GameStateContext c) {
        if (instance != null)
            return;

        grid = h;
        instance = this;
        context = c;
        destroyers = grid.cSpawner.GetEnemies();
    }

    public override void onEntered()
    {
        grid.TransferToDestroyerRound();

        Debug.Log("Destroyer Round Now !");
        chaseCitizens();
    }

    public override void onExit()
    {
        //StopAllCoroutines();
    }

    void chaseCitizens()
    {
        int movingUnits = 0;

        foreach (EnemyDemo e in destroyers)
        {
            List<HexCellMesh> path = grid.FindNearTarget();
            e.moveByPath(ref path);
        }

        // 阻塞 —— 因为模型没有实现，暂时阻塞
        //while (movingUnits < destroyers.Count)
        //{
        //    movingUnits = 0;
        //    foreach (EnemyDemo e in destroyers)
        //    {
        //        if (e.MovementDone == true)
        //            movingUnits += 1;
        //    }
        //}
        changeState();
    }

    public override void changeState()
    {
        context.changeState();
    }

    protected enum DestroyerStates {
        THINKING, MOVING
    }
}
