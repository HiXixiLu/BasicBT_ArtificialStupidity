using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceDemo : CharacterBase
{
    protected override void Awake()
    {
        Health = CharacterLimits.HealthLimit;
        MovementScaleSetter = CharacterLimits.MeleeMovementScale;
        GunshotSetter = CharacterLimits.PistalGunshot;
        DamageSetter = CharacterLimits.PistolDamage;
        NameSetter = CharacterLimits.policeName;

        base.Awake();
    }

    public override void handleInteraction(characterInteraction action, int value) {
        switch (action) {
            case characterInteraction.BE_ATTACKED:
                beAttacked(value);
                break;
            case characterInteraction.BE_RESCUED:
                beRescued(value);
                break;
            default:
                break;
        }
    }

    public void attack(CharacterBase opponent) {
        opponent.handleInteraction(characterInteraction.BE_ATTACKED, this.Damage);
        // TODO: 增加动画
    }
}
