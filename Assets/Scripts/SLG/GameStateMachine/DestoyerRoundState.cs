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

    private List<EnemyDemo> destroyers;
    private GameStateContext context;

    public DestoyerRoundState(GameStateContext c) {
        if (instance != null)
            return;

        instance = this;
        context = c;
        destroyers = context.grid.cSpawner.GetEnemies();

        foreach (EnemyDemo e in destroyers) {
            e.SetEventCallback(OnEventMovementCompletion);
        }
    }

    public override void onEntered()
    {
        context.grid.TransferToDestroyerRound();
        context.actionLimit = ValueBoundary.ActionLimit;
        context.grid.ChangeActionsNum(context.actionLimit);

        Debug.Log("Destroyer Round Now !");
        foreach (EnemyDemo e in destroyers)
        {
            e.transform.GetComponent<Collider>().enabled = true;
        }
        
    }

    public override void onExit()
    {
        //StopAllCoroutines();
        foreach (EnemyDemo e in destroyers)
        {
            e.transform.GetComponent<Collider>().enabled = false;
        }
    }

    public override void changeState()
    {
        context.changeState();
    }

    public void OnEventMovementCompletion()
    {
        if (context.actionLimit > 0)
        {
            context.actionLimit--;
            context.grid.ChangeActionsNum(context.actionLimit);
            if (context.actionLimit == 0)
                changeState();
        }
    }

    protected enum DestroyerStates {
        THINKING, MOVING
    }
}
