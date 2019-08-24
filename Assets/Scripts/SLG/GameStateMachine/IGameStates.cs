using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStates
{
    void onEntered();   // 进入状态
    void onExit();      // 退出状态
}
