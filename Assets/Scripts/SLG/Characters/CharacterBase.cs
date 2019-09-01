using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected Vector3 yOffset = new Vector3(0, 4, 0);    // 保持胶囊在地面上的偏移值
    protected Vector3 yDownOffset = new Vector3(0, 2, 0);   // 保持胶囊躺倒的偏移值

    public delegate void EventCallback();   // 角色完成某个事件的回调
    protected EventCallback movementCompleteCallback;
    public void setEventCallback(EventCallback e) {
        movementCompleteCallback = e;
    }
    
    int health;          // 生命
    string name;
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

    public string NameSetter {
        set {
            name = value;
        }
    }
    public string Name {
        get {
            return name;
        }
    }

    protected virtual void Awake() { }

    public virtual void handleInteraction(characterInteraction action, int value) { }

    // 移动函数
    public virtual void moveByPath(ref List<HexCellMesh> pathHexes) {
        if (pathHexes.Count == 0)
            return;

        if (pathHexes[0].Distance > this.movementScale)
            return;

        if (isDown == true)
            return;

        StartCoroutine(moving(pathHexes));
    }

    /// <summary>
    /// 按一定时间步长逐格子移动棋子
    /// </summary>
    /// <param name="pathHexes"> 寻路过程中保存的棋格 </param>
    /// <returns></returns>
    IEnumerator moving(List<HexCellMesh> pathHexes)
    {

        for (int i = pathHexes.Count - 1; i >= 0; i--)
        {
            Vector3 from = transform.position;
            Vector3 to = pathHexes[i].center + yOffset;
            float time = 0f;
            Vector3 mSpeed = (to - from) / CharacterLimits.movingTimeStep;

            //rotateToSpeedDirection(from, to);

            while (time < CharacterLimits.movingTimeStep)
            {
                yield return null;
                transform.Translate(mSpeed * Time.deltaTime);
                time += Time.deltaTime;
            }

            transform.position = pathHexes[i].center + yOffset;
        }
        pathHexes[0].Occupant = this;

        if (movementCompleteCallback != null) {
            movementCompleteCallback(); //可实现闭包吗
        }
        // TODO: 必须确保移动期间不发生其他移动、互动等操作
    }
    /// <summary>
    /// 由当前 z 轴朝向与运动方向的 夹角，确定旋转
    /// </summary>
    /// <param name="from"> 出发方格 </param>
    /// <param name="to"> 目的方格 </param>
    /// <returns></returns>
    private void rotateToSpeedDirection(Vector3 from, Vector3 to) {
        Vector3 vectorSpeed = (to - from).normalized;
        transform.forward = vectorSpeed;    // TODO: 旋转有 bug
    }

    public virtual void down() {
        this.health = -1;
        this.isDown = true;

        this.transform.Rotate(new Vector3(90,0,0));
        this.transform.position -= yDownOffset;
    }

    public virtual void beAttacked(int value)
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

    public virtual void beRescued(int value)
    {
        if (IsDown)
            return;

        Health = (Health + value) > CharacterLimits.HealthLimit ? CharacterLimits.HealthLimit : Health + value;
    }
}

public enum characterInteraction {
    BE_ATTACKED,
    BE_RESCUED
}
