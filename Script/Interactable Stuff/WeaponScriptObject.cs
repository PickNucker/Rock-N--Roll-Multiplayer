using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Weapon", menuName = "new Weapon")]
public class WeaponScriptObject : ScriptableObject
{
    [SerializeField] int id;
    [SerializeField] string weaponName;
    [SerializeField] float weaponDamage = 20f;
    [SerializeField] float shootSpeed = 2f;
    [SerializeField] int currentMaxAmmo = 30;
    [SerializeField] int startClips = 30;
    [SerializeField] int maxAmmoVorrat = 260;
    [SerializeField] float reloadSpeed = 2f;
    [SerializeField] bool dauerFeuer = true;
    [SerializeField] bool canChangeFireRate = true;
    [SerializeField] AnimatorOverrideController controller = default;
    [SerializeField] GameObject muzzlePrefab = default;

    [Space][Header("Audio")]
    [SerializeField] AudioTriggerFX shootSfx;
    [SerializeField] AudioTriggerFX reloadSfx;
    [SerializeField] AudioTriggerFX noAmmoSfx;

    Transform bulletSpawnPos = default;

    public void AttachWeapon(Animator anim, Transform handTransform)
    {
        if(controller != null)
            anim.runtimeAnimatorController = controller;

        foreach (Transform item in handTransform)
        {
            // Get Weapon in hand transform
            if(item.name == weaponName)
            {
                item.gameObject.SetActive(true);
                foreach (Transform child in item)
                {
                    // Get SpawnPos of weapon prefab
                    if(child.name == "SpawnProjectiles")
                    {
                        bulletSpawnPos = child;
                    }
                }
            }else item.gameObject.SetActive(false);
        }
    }

    public void PlayShootAudio(Vector3 pos)
    {
        shootSfx.Play(pos);
    }

    public void PlayReloadAudio(Vector3 pos, GameObject parent)
    {
        reloadSfx.Play(pos, parent);
    }

    public void PlayNoAmmoAudio(Vector3 pos)
    {
        noAmmoSfx.Play(pos);
    }

    #region GetMethoden

    public GameObject GetMuzzlePrefab()
    {
        return muzzlePrefab;
    }

    public string GetWeaponName()
    {
        return weaponName;
    }

    public float GetWeaponDamage()
    {
        return weaponDamage;
    }
    public float GetReloadSpeed()
    {
        return reloadSpeed;
    }

    public float GetTimerBtwBullets()
    {
        return shootSpeed;
    }

    public Transform GetBulletSpawnPos()
    {
        return bulletSpawnPos;
    }

    public int GetCurrentStartAmmo()
    {
        return currentMaxAmmo;
    }

    public int GetStartClip()
    {
        return startClips;
    }

    public int GetMaxAmmoVorrat()
    {
        return maxAmmoVorrat;
    }

    public bool GetBurstMode()
    {
        return dauerFeuer;
    }

    public bool GetChangeBurstMode()
    {
        return canChangeFireRate;
    }

    public void SetBurstMode()
    {
        dauerFeuer = !dauerFeuer;
    }
    #endregion
}
