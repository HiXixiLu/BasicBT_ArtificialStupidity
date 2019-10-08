using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateContext
{
    public int actionLimit = ValueBoundary.ActionLimit; //每回合的行动点
    public HexGridManager grid;

    private StateId state;
    public StateId State {
        get {
            return state;
        }
    }
    Dictionary<StateId, GameStates> statesDict;  // 组合关系

    private GameStateContext instance;
    public GameStateContext Instance {
        get {
            return instance;
        }
    }

    public GameStateContext(HexGridManager h) {
        if(instance != null)
            return;

        statesDict = new Dictionary<StateId, GameStates>();
        grid = h;

        statesDict.Add(StateId.GUARD, new GuardRoundState(this));
        statesDict.Add(StateId.DESTOYER, new DestoyerRoundState(this));
        statesDict.Add(StateId.CITIZENS, new CitizenRoundState(this));

        state = StateId.CITIZENS;
        statesDict[state].onEntered();

        instance = this;
    }
    
    public void changeState() {
        statesDict[state].onExit();

        actionLimit = ValueBoundary.ActionLimit;    // 重置行动点;

        if (state == StateId.CITIZENS)
        {
            state = StateId.DESTOYER;
        }
        else if (state == StateId.DESTOYER)
        {
            state = StateId.GUARD;
        }
        else if (state == StateId.GUARD)
        {
            state = StateId.CITIZENS;
        }

        statesDict[state].onEntered();
    }

    public void OnEventAttackCompletion() {
        if (actionLimit > 0)
        {
            actionLimit--;
            grid.ChangeActionsNum(actionLimit);

            if (actionLimit == 0)
                changeState();
        }
        else {
            changeState();
        }
    }
}

public enum StateId
{
    GUARD, CITIZENS, DESTOYER
}
