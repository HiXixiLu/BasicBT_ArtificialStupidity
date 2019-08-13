using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public int health;          // 生命
    public int movementScale;   // 移动范围
    public bool isDown = false; // 是否倒下

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
