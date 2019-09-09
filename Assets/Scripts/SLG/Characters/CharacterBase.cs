using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected Vector3 yOffset = new Vector3(0, 4, 0);    // 保持胶囊在地面上的偏移值
    protected Vector3 yDownOffset = new Vector3(0, 2, 0);   // 保持胶囊躺倒的偏移值

    public delegate void EventCallback();   // 角色完成某个事件的回调
    protected EventCallback movementCompleteCallback;
    public void SetEventCallback(EventCallback e) {
        movementCompleteCallback = e;
    }

    public HexCellMesh Occupation;
    
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
            if (health + value >= ValueBoundary.HealthLimit)
                health = ValueBoundary.HealthLimit;
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
        List<HexCellMesh> path = new List<HexCellMesh>(pathHexes);  //防止引用被清空
        pathHexes[0].Occupant = this;
        this.Occupation = pathHexes[0];

        for (int i = path.Count - 1; i >= 0; i--)
        {
            Vector3 from = transform.position;
            Vector3 to = path[i].center + yOffset;
            float time = 0f;
            Vector3 mSpeed = (to - from) / ValueBoundary.movingTimeStep;

            //rotateToDirection(path[i]);   // bug： 有闪出运动路径的问题出现

            while (time < ValueBoundary.movingTimeStep)
            {
                yield return null;
                transform.Translate(mSpeed * Time.deltaTime);
                time += Time.deltaTime;
            }

            transform.position = path[i].center + yOffset;
        }

        if (movementCompleteCallback != null) {
            movementCompleteCallback();     // 委托 —— 可实现闭包
        }

    }

    /// <summary>
    /// 将棋子以一定角速度旋转朝向某个方向
    /// </summary>
    /// <param name="direction"></param>
    void rotateToDirection(HexCellMesh desCell) {
        Vector3 direction = desCell.transform.position + yOffset - this.transform.position;
        float angle = Vector3.Angle(this.transform.forward, direction);

        transform.localRotation = Quaternion.Euler(0f, angle, 0f);
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

        Health = (Health + value) > ValueBoundary.HealthLimit ? ValueBoundary.HealthLimit : Health + value;
    }
}

public enum characterInteraction {
    BE_ATTACKED,
    BE_RESCUED
}
