using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceDemo : CharacterBase
{
    // 基础属性
    int gunshot = CharacterLimits.PistalGunshot;
    int damage = CharacterLimits.PistolDamage;

    protected override void Awake()
    {
        health = CharacterLimits.HealthLimit;
        movementScale = CharacterLimits.MeleeMovementScale;
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

    public override void movetoPosition() {
        // TODO: 先完成寻路算法
    }

    public void attack() {
        // TODO: 先确定是通过棋格选定角色，还是直接选定角色
    }

    private void beAttacked(int value) {

        if (health > 0) {
            health -= value;
        }

        if (health <= 0) {
            down();
        }
    }

    private void beRescued(int value) {
        if (isDown)
            return;

        health = (health + value) > CharacterLimits.HealthLimit ? CharacterLimits.HealthLimit : health + value;
    }
}
