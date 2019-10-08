using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truncheon : Coldpweapon
{
    public Truncheon() {
        damage = ValueBoundary.MeleeDamage;
    }

    public override void attack() {
        // TODO: 动画
    }
}
