using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incendiary : Grenade
{
    public Incendiary() {
        RangeSetter = ValueBoundary.IncendiryRange;
        AttackDistanceSetter = ValueBoundary.IncendiryDistance;
        DamageSetter = ValueBoundary.IncendiaryDamage;

        CapacitySetter = ValueBoundary.IncendiaryCapacity;
    }

    public override void Attack(HexCellMesh from, HexCellMesh to)
    {
        // TODO: 抛物线动画
        if (Capacity > 0)
            CapacitySetter = Capacity - 1;
        else
            CapacitySetter = 0;
    }
}
