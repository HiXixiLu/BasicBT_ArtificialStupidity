using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* 用于各项基础棋子属性的数值
 */
public static class CharacterLimits
{
    // 生理数值
    public const int HealthLimit = 100;
    public const int MovementScaleLimit = 10;
    public const int MeleeMovementScale = 4;
    public const int ArcherMovementScale = 6;

    // 道具数值
    public const int PistalGunshot = 3;
    public const int RiffleGunshot = 6;
    public const int MeleeRange = 1;
    public const int HealthRecovered = 40;

    // 武器数值
    public const int PistolDamage = 20;
    public const int RiffleDamage = 35;
    public const int MeleeDamage = 50;

    // 杂项数值
    public const float movingTimeStep = 0.2f;  // in seconds

}
