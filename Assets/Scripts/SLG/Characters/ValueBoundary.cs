using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* 用于各项基础棋子属性的数值
 */
public static class ValueBoundary
{
    // 生理数值
    public const int HealthLimit = 100;
    public const int MovementScaleLimit = 14;
    public const int MeleeMovementScale = 10;
    public const int ArcherMovementScale = 14;

    // 道具数值
    public const int MeleeRange = 1;
    public const int HealthRecovered = 40;

    // 武器数值
    public const int PistalGunshot = 10;
    public const int RiffleGunshot = 18;

    public const int PistolDamage = 40;
    public const int RiffleDamage = 70;
    public const int MeleeDamage = 50;
    public const int IncendiaryDamage = 30;
    public const int TearGasDamage = 20;

    public const int PistolAmmos = 14;
    public const int RiffleAmmos = 15;

    public const int IncendiryDistance = 7;
    public const int TearGasDistance = 8;

    public const int IncendiryRange = 2;    //以到中心棋格距离为准
    public const int TearGasRange = 3;   

    // 杂项数值
    public const float movingTimeStep = 0.2f;  // in seconds
    public const int ActionLimit = 5;   // actions can be executed in a single round

    // 杂项信息
    public static string policeName = "police";
    public static string destroyerName = "destroyer";
    public static string citizenName = "citizen";

}
