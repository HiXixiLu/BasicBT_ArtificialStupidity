using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 投掷的武器根据抛物线计算运动 —— 产生的是落地点一定范围内的伤害 
 */
public abstract class Grenade
{
    int range;
    int damage;
    int attackDistance;
    int capacity;

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
    public int Capacity {
        get { return capacity; }
    }
    public int CapacitySetter {
        set { capacity = value; }
    }

    public abstract void Attack(HexCellMesh from, HexCellMesh to);  // 抛物线轨迹 —— 以计算落地点的伤害范围
}
