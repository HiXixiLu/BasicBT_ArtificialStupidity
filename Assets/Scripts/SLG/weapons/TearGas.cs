using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearGas : Grenade
{
    public TearGas() {
        RangeSetter = ValueBoundary.TearGasRange;
        AttackDistanceSetter = ValueBoundary.TearGasDistance;
        DamageSetter = ValueBoundary.TearGasDamage;
    }

    public override void Attack(Vector3 from, Vector3 to)
    {
        throw new System.NotImplementedException();
    }
}
