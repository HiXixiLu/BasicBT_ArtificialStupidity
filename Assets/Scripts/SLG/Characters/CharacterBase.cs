using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    int health;          // 生命
    int movementScale;   // 移动范围
    bool isDown = false; // 是否倒下
    int gunshot;
    int damage;
    public int Gunshot
    {
        get
        {
            return gunshot;
        }
    }
    protected int GunshotSetter {
        set {
            gunshot = value;
        }
    }
    public int Damage
    {
        get
        {
            return damage;
        }
    }
    protected int DamageSetter {
        set {
            damage = value;
        }
    }
    public int MovementScale {
        get {
            return movementScale;
        }
    }
    protected int MovementScaleSetter {
        set {
            movementScale = value;
        }
    }
    public bool IsDown {
        get {
            return isDown;
        }
        set {
            isDown = value;
        }
    }
    public int Health {
        get {
            return health;
        }
        set {
            if (health + value >= CharacterLimits.HealthLimit)
                health = CharacterLimits.HealthLimit;
            else
                health = value;
        }
    }

    protected virtual void Awake() { }

    public virtual void handleInteraction(characterInteraction action, int value) { }
    public virtual void movetoPosition() { }
    public virtual void down() {
        this.health = -1;
        this.isDown = true;
    }
}

public enum characterInteraction {
    BE_ATTACKED,
    BE_RESCUED
}
