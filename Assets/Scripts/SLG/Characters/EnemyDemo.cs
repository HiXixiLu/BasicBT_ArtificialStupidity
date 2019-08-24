using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDemo : CharacterBase
{
    // 基础属性
    protected override void Awake()
    {
        Health = CharacterLimits.HealthLimit;
        MovementScaleSetter = CharacterLimits.MeleeMovementScale;
        GunshotSetter = CharacterLimits.PistalGunshot;
        DamageSetter = CharacterLimits.PistolDamage;

        base.Awake();
    }

    public override void handleInteraction(characterInteraction action, int value)
    {
        switch (action)
        {
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

    public void attack()
    {
        // TODO: 怎样发动攻击
    }

    private void beAttacked(int value)
    {

        if (Health > 0)
        {
            Health -= value;
        }

        if (Health <= 0)
        {
            down();
        }
    }

    private void beRescued(int value)
    {
        if (IsDown)
            return;

        Health = (Health + value) > CharacterLimits.HealthLimit ? CharacterLimits.HealthLimit : Health + value;
    }
}
