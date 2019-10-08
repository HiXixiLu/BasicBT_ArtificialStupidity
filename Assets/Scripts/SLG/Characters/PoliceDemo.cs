using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* 目前角色采取的方法是在自己的成员方法内部判断各种条件，UI事件类仅仅采集点击行为，并将点击事件的上下文、所点击Gameobject通知对应的逻辑模块
 */
public class PoliceDemo : CharacterBase
{
    Guns pistol = new Pistol();
    Coldpweapon truncheon = new Truncheon();
    //List<Grenade> tearGas = new List<Grenade>();  //先不加催泪弹了
    public Guns Gun {
        get { return pistol; }
    }
    public Coldpweapon Coldpweapon {
        get { return truncheon; }
    }
    public Grenade Grenade {
        get { return null; }
    }

    protected override void Awake()
    {
        Health = ValueBoundary.HealthLimit;
        MovementScaleSetter = ValueBoundary.MeleeMovementScale;
        //GunshotSetter = ValueBoundary.PistalGunshot;  //射程和伤害应该由回合中所使用的武器决定，而不是硬编码决定
        //DamageSetter = ValueBoundary.PistolDamage;
        NameSetter = ValueBoundary.policeName;

        base.Awake();
    }

    public override void handleInteraction(characterInteraction action, int value) {
        switch (action) {
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
    public override void attack(CharacterBase target, int distance) {
        if (distance == ValueBoundary.MeleeRange)
        {
            Debug.Log("警棍攻击许可");
            truncheon.attack();
            // TODO: 减少行动点 —— 该函数在状态机里
        }
        else if (distance <= pistol.Gunshot)
        {
            Vector3 direction = target.transform.position - this.transform.position;
            if (pistol.Ammos > 0)
            {
                pistol.Attack(direction);
                // TODO: 减少行动点 —— 该函数在状态机里
            }
        }
    }

    /// <summary>
    /// 使用回调来通知事件来源是否可以进行合法进攻
    /// </summary>
    /// <param name="target"> 被本单位进攻的对方单位 </param>
    /// <param name="distance"> 两个单位之间的最短棋格距离 </param>
    /// <param name="conditionCallback"> 通知事件来源是否可以更新被进攻者状态的回调 </param>
    public override void attack(CharacterBase target, int distance, AttackConditionCallback conditionCallback)
    {
        if (distance == ValueBoundary.MeleeRange)
        {
            Debug.Log("警棍攻击许可");
            truncheon.attack();
            conditionCallback(true, truncheon.Damage);
        }
        else if (distance <= pistol.Gunshot)
        {
            Vector3 direction = target.transform.position - this.transform.position;
            if (pistol.Ammos > 0)
            {
                Debug.Log("手枪攻击许可");
                pistol.Attack(direction);
                conditionCallback(true, pistol.Damage);
            }
            else
            {
                Debug.Log("Ammos are out.");
                conditionCallback(false);
            }
        }
        else {
            Debug.Log("Distance out of range");
            conditionCallback(false);
        }
    }

}
