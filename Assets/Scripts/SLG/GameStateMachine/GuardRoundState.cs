using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardRoundState : GameStates
{
    private GuardRoundState instance;
    public GuardRoundState Instance {
        get {
            return instance;
        }
    }

    private HexGridManager grid;
    private GameStateContext context;
    private List<PoliceDemo> guards;

    private bool updateSignal = false;

    public GuardRoundState(HexGridManager h, GameStateContext c) {
        if (instance != null)
            return;
        grid = h;
        instance = this;
        context = c;
        guards = grid.cSpawner.GetPolices();
    }

    public override void onEntered()
    {
        grid.TransferToPoliceRound();
        guards = grid.cSpawner.GetPolices();

        Debug.Log("Police Round now!");
        updateSignal = true;
    }

    public override void onExit()
    {
        updateSignal = false;
    }

    public override void changeState()
    {
        context.changeState();
    }

    // 进行鼠标点击的射线检测
    private void Update()
    {
        if (!updateSignal)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // TODO: 这里要重写点击函数了
        }
    }

    protected enum GuardStates {
        THINKING, MOVING
    }
}
