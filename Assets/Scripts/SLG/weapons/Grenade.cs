using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grenade
{
    int range;
    int damage;
    int attackDistance;

    protected int RangeSetter {
        set { range = value; }
    }
    public int Range {
        get { return range; }
    }
    protected int DamageSetter {
        set { damage = value; }
    }
    public int Damage {
        get { return damage; }
    }
    protected int AttackDistanceSetter {
        set { attackDistance = value; }
    }
    public int AttackDistance {
        get { return attackDistance; }
    }

    public abstract void Attack(Vector3 from, Vector3 to);  // 抛物线轨迹
}
