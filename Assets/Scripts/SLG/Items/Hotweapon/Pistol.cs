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

    // 在鼠标移动的过程中，
    public override void Attack(Vector3 direction) {
        if (Ammos > 0)
        {
            // TODO: 动画
            AmmosSetter = Ammos - 1;
        }
        else {
            AmmosSetter = 0;
        }
    }
}
