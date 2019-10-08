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

    private GameStateContext context;
    private List<PoliceDemo> guards;

    public GuardRoundState( GameStateContext c) {
        if (instance != null)
            return;

        instance = this;
        context = c;
        guards = context.grid.cSpawner.GetPolices();

        foreach (PoliceDemo p in guards) {
            p.SetEventCallback(OnEventMovementCompletion);  //通过委托调用实现了闭包
        }
    }

    public override void onEntered()
    {
        context.grid.TransferToPoliceRound();
        context.actionLimit = ValueBoundary.ActionLimit;
        context.grid.ChangeActionsNum(context.actionLimit);

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
        if (context.actionLimit > 0) {
            context.actionLimit--;
            context.grid.ChangeActionsNum(context.actionLimit);
            if (context.actionLimit == 0)
                changeState();
        }
    }

    protected enum GuardStates {
        THINKING, MOVING
    }
}
