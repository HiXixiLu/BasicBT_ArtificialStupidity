using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incendiary : Grenade
{
    public Incendiary() {
        RangeSetter = ValueBoundary.IncendiryRange;
        AttackDistanceSetter = ValueBoundary.IncendiryDistance;
        DamageSetter = ValueBoundary.IncendiaryDamage;
    }

    public override void Attack(Vector3 from, Vector3 to)
    {
        throw new System.NotImplementedException();
    }
}
