using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearGas : Grenade
{
    public TearGas() {
        RangeSetter = ValueBoundary.TearGasRange;
        AttackDistanceSetter = ValueBoundary.TearGasDistance;
        DamageSetter = ValueBoundary.TearGasDamage;

        CapacitySetter = ValueBoundary.TearGasCapacity;
    }

    public override void Attack(HexCellMesh from, HexCellMesh to)
    {
        throw new System.NotImplementedException();
    }
}
