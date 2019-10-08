using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 枪械：
 * 对某个方向上第一个击中的单位产生伤害 （暂不考虑区分友方、敌方的问题）
 */
public abstract class Guns
{
    int damage;
    int gunshot;
    int ammos;

    protected int DamageSetter {
        set { damage = value; }
    }
    public int Damage {
        get { return damage; }
    }
    protected int GunshotSetter {
        set { gunshot = value; }
    }
    public int Gunshot {
        get { return gunshot; }
    }
    protected int AmmosSetter {
        set { ammos = value; }
    }
    public int Ammos {
        get { return ammos; }
    }

    // 一个动画函数
    public abstract void Attack(Vector3 direction) ;    // 枪械所瞄准的向量
}
