using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStates: MonoBehaviour
{
    abstract public void onEntered();   // 进入状态
    abstract public void onExit();     // 退出状态
    abstract public void changeState(); // 切换状态
}
