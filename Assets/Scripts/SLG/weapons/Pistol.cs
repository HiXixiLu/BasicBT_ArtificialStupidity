using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Guns
{
    public Pistol() {
        AmmosSetter = ValueBoundary.PistolAmmos;
        DamageSetter = ValueBoundary.PistolDamage;
        GunshotSetter = ValueBoundary.PistalGunshot;
    }

    public override void Attack(Vector3 direction) {
        // TODO: 按某个方向攻击
    }
}
