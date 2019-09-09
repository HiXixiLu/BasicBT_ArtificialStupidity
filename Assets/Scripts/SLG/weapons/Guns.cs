using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public abstract void Attack(Vector3 direction) ;    // 枪械所瞄准的向量
}
