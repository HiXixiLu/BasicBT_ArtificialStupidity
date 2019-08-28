using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateContext
{
    private StateId state;
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

        statesDict.Add(StateId.GUARD, new GuardRoundState(h, this));
        statesDict.Add(StateId.DESTOYER, new DestoyerRoundState(h, this));
        statesDict.Add(StateId.CITIZENS, new CitizenRoundState(h, this));

        state = StateId.CITIZENS;
        statesDict[state].onEntered();

        instance = this;
    }
    
    public void changeState() {
        statesDict[state].onExit();

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

    enum StateId
    {
        GUARD, CITIZENS, DESTOYER
    }
}
