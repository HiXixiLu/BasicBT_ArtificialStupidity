using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chopper : Coldpweapon
{
    public Chopper()
    {
        damage = ValueBoundary.MeleeDamage;     //后期做数值平衡的时候再改
        range = ValueBoundary.MeleeRange;
    }

    public override void attack()
    {
        // TODO: 对角色造成伤害
        Debug.Log("大刀攻击");
    }
}
