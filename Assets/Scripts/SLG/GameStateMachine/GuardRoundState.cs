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

    private int actionLimits = CharacterLimits.ActionLimit;

    public GuardRoundState(HexGridManager h, GameStateContext c) {
        if (instance != null)
            return;

        grid = h;
        instance = this;
        context = c;
        guards = grid.cSpawner.GetPolices();

        foreach (PoliceDemo p in guards) {
            p.SetEventCallback(OnEventMovementCompletion);  //通过委托调用实现了闭包
        }
    }

    public override void onEntered()
    {
        grid.TransferToPoliceRound();
        actionLimits = CharacterLimits.ActionLimit;
        grid.ChangeActionsNum(actionLimits);

        Debug.Log("Police Round now!");
        foreach (PoliceDemo p in guards) {
            p.transform.GetComponent<Collider>().enabled = true;
        }
    }

    public override void onExit()
    {
        foreach (PoliceDemo p in guards)
        {
            p.transform.GetComponent<Collider>().enabled = false;
        }
    }

    public override void changeState()
    {
        context.changeState();
    }

    public void OnEventMovementCompletion() {
        if (actionLimits > 0) {
            actionLimits--;
            grid.ChangeActionsNum(actionLimits);
            if (actionLimits == 0)
                changeState();
        }
    }

    protected enum GuardStates {
        THINKING, MOVING
    }
}
