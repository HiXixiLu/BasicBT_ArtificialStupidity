using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDemo : CharacterBase
{
    Coldpweapon chopper = new Chopper();
    Grenade grenades = new Incendiary();
    public Coldpweapon Coldpweapon {
        get { return chopper; }
    }
    public Guns Gun {
        get { return null; }
    }
    public Grenade Grenade {
        get { return grenades; }
    }

    // 基础属性
    protected override void Awake()
    {
        Health = ValueBoundary.HealthLimit;
        MovementScaleSetter = ValueBoundary.MeleeMovementScale;
        //GunshotSetter = ValueBoundary.PistalGunshot;  //射程和伤害应该由回合中所使用的武器决定，而不是硬编码决定
        //DamageSetter = ValueBoundary.PistolDamage;
        NameSetter = ValueBoundary.destroyerName;

        base.Awake();
    }

    public override void handleInteraction(characterInteraction action, int value)
    {
        switch (action)
        {
            case characterInteraction.BE_ATTACKED:
                beAttacked(value);
                break;
            //case characterInteraction.BE_RESCUED:
            //    beRescued(value);
            //    break;
            default:
                break;
        }
    }

    // obselete
    public override void attack(CharacterBase target, int distance)
    {
        if (distance == ValueBoundary.MeleeRange)
        {
            chopper.attack();
        }
        else if (distance <= ValueBoundary.IncendiryDistance)
        {
            Vector3 direction = target.transform.position - this.transform.position;
            if (grenades.Capacity > 0)
            {
                // grenade系列函数的特殊处理 —— 范围攻击 —— 放到 HexManager


                grenades.CapacitySetter = grenades.Capacity - 1;
            }
        }
    }
    /// <summary>
    /// 使用回调来通知事件来源是否可以进行合法进攻
    /// </summary>
    /// <param name="target">被进攻的中心点人物 </param>
    /// <param name="distance"> 两个单位之间的距离 </param>
    /// <param name="conditionCallback"> 通知事件来源是否可以更新被进攻者状态的回调 </param>
    public override void attack(CharacterBase target, int distance, AttackConditionCallback conditionCallback)
    {
        if (distance == ValueBoundary.MeleeRange)
        {
            chopper.attack();
            conditionCallback(true, chopper.Damage);
        }
        else if (distance <= ValueBoundary.IncendiryDistance)
        {
            // TODO: 计算抛物线动画所需要的参数
            if (grenades.Capacity > 0)
            {
                // grenade系列函数的特殊处理 —— 范围攻击 —— 放到 回调中去

                Debug.Log("燃烧弹攻击许可");
                grenades.CapacitySetter = grenades.Capacity - 1;
                conditionCallback(true, grenades.Damage);
                
            }
            else
            {
                Debug.Log("Incendiary's out.");
                conditionCallback(false);
            }
        }
        else {
            Debug.Log("Distance out of range");
            conditionCallback(false);
        }
    }
}
