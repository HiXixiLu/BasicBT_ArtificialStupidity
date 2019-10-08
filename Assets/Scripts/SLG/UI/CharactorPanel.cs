using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactorPanel : MonoBehaviour
{
    public Text CharacterName;
    public Image Avatar;
    public Image Health;    // 一个填充方式为 Fill 的图片

    public Text MovementRange;

    public Image Gun;
    public Text GunDamage;
    public Text Ammo;

    public Image Grenade;
    public Text GrenadeDamage;
    public Text GrenadeCount;

    public Text ColdweaponDamage;
    public Image ColdweaponImage;

    //public Text KitCount; // 原本用于医疗包的计数

    /// <summary>
    /// 屏幕右边的角色UI面板的更新操作
    /// </summary>
    /// <param name="role"></param>
    public void UpdateCharactorUI(CharacterBase role)
    {
        // TODO: 角色面板更新
        Debug.Log("角色面板更新");
        //SetAvatar()
        SetCharatorName(role.Name);
        SetHealth(role.Health);
        SetMovementRange(role.MovementScale);

        if (role.Name == ValueBoundary.destroyerName)
        {
            EnemyDemo p = (EnemyDemo)role;
            if (p.Gun != null)
            {
                SetAmmo(p.Gun.Ammos);
                SetGunDamage(p.Gun.Damage);
                //SetGunSprite()
            }
            else {
                SetAmmo();
                SetGunDamage();
            }
            if (p.Coldpweapon != null)
            {
                //SetColdweaponImage()
                SetColdweaponDamage(p.Coldpweapon.Damage);
            }
            if (p.Grenade != null)
            {
                //SetGrenadeImage()
                SetGrenadeCount(p.Grenade.Capacity);
                SetGrenadeDamage(p.Grenade.Damage);
            }
            else {
                SetGrenadeCount();
                SetGrenadeDamage();
            }
        }
        else if (role.Name == ValueBoundary.policeName) {
            PoliceDemo p = (PoliceDemo)role;
            if (p.Gun != null) {
                SetAmmo(p.Gun.Ammos);
                SetGunDamage(p.Gun.Damage);
                //SetGunSprite()
            }
            if (p.Coldpweapon != null) {
                //SetColdweaponImage()
                SetColdweaponDamage(p.Coldpweapon.Damage);
            }
            if (p.Grenade != null)
            {
                //SetGrenadeImage()
                SetGrenadeCount(p.Grenade.Capacity);
                SetGrenadeDamage(p.Grenade.Damage);
            }
            else {
                SetGrenadeCount();
                SetGrenadeDamage();
            }
        }
    }
    public void HidePanel() {
        this.gameObject.SetActive(false);
    }
    public void ShowPanel(CharacterBase role) {
        this.gameObject.SetActive(true);
        UpdateCharactorUI(role);
    }


    void SetCharatorName(string name) {
        CharacterName.text = name;
    }
    void SetAvatar(Sprite a) {
        Avatar.sprite = a;
    }
    void SetHealth(int health) {
        if (health > ValueBoundary.HealthLimit) {
            Health.fillAmount = 1;
            return;
        }
        float val = (float)health / ValueBoundary.HealthLimit;
        Health.fillAmount = val;
    }
    void SetMovementRange(int range) {
        MovementRange.text = range.ToString();
    }
    void SetGunSprite(Sprite gun) {
        Gun.sprite = gun;
    }
    void SetGunDamage(int dam = 0) {
        if (dam > 0)
            GunDamage.text = dam.ToString();
        else
            GunDamage.text = "0";
    }
    void SetAmmo(int ammo = 0) {
        if (ammo > 0)
            Ammo.text = ammo.ToString();
        else
            Ammo.text = "None";
    }
    void SetGrenadeImage(Sprite image) {
        Grenade.sprite = image;
    }
    void SetGrenadeDamage(int dam = 0) {
        if (dam > 0)
            GrenadeDamage.text = dam.ToString();
        else
            GrenadeDamage.text = "0";
    }
    void SetGrenadeCount(int count = 0) {
        if (count > 0)
            GrenadeCount.text = count.ToString();
        else
            GrenadeCount.text = "None";
    }
    void SetColdweaponImage(Sprite img) {
        ColdweaponImage.sprite = img;
    }
    void SetColdweaponDamage(int damage = 0) {
        if(damage > 0)
            ColdweaponDamage.text = damage.ToString();
    }
    //void SetKitCount(int count) {
    //    KitCount.text = count.ToString();
    //}
}
