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

    private int actionLimits = CharacterLimits.ActionLimit;

    public DestoyerRoundState(HexGridManager h, GameStateContext c) {
        if (instance != null)
            return;

        grid = h;
        instance = this;
        context = c;
        destroyers = grid.cSpawner.GetEnemies();

        foreach (EnemyDemo e in destroyers) {
            e.SetEventCallback(OnEventMovementCompletion);
        }
    }

    public override void onEntered()
    {
        grid.TransferToDestroyerRound();
        actionLimits = CharacterLimits.ActionLimit;
        grid.ChangeActionsNum(actionLimits);

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
        if (actionLimits > 0)
        {
            actionLimits--;
            grid.ChangeActionsNum(actionLimits);
            if (actionLimits == 0)
                changeState();
        }
    }

    protected enum DestroyerStates {
        THINKING, MOVING
    }
}
