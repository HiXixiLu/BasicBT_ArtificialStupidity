using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Coldpweapon
{
    protected int damage;
    protected int range = ValueBoundary.MeleeRange;
    public int Damage{
        get { return damage; }
    }
    public int Range {
        get { return range; }
    }

    // 这里只负责单纯的动画展示
    public virtual void attack() { }
}
