using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleWeapon : MonoBehaviour
{
    [SerializeField] Text currentAmmoText;
    [SerializeField] Text currentAmmoVorratText;
    [SerializeField] Text weaponNameText;
    [SerializeField] Text burstModeText;
    [Space]

    [SerializeField] Image dauerModusImage;
    [SerializeField] Image singleModusImage;

    [Space]
    [SerializeField] AudioTriggerFX changeFireModeSfx;

    PlayerMovement player;

    PhotonView m_View;

    int currentAmmo;
    int currentAmmoVorrat;
    int newAmount;
    bool isReloading;
    bool burstMode;
    string weaponName;

    float timerToChangeFireMode;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        m_View = GetComponent<PhotonView>();
    }

    void Start()
    {
        UpdateWeaponUI();
        if (!m_View.IsMine)
        {
            Destroy(currentAmmoText.gameObject);
            Destroy(currentAmmoVorratText.gameObject);
            Destroy(weaponNameText.gameObject);
            Destroy(burstModeText.gameObject);
            Destroy(dauerModusImage.gameObject);
            Destroy(singleModusImage.gameObject);           
        }
    }

    void Update()
    {
        if (!m_View.IsMine) return;
        timerToChangeFireMode += Time.deltaTime;

        burstMode = player.GetCurrentWeapon().GetBurstMode();

        if (burstMode)
        {
            if(m_View.IsMine)
            {
                singleModusImage.enabled = false;
                dauerModusImage.enabled = true;
            }
        }
        else
        {
            if (m_View.IsMine)
            {
                singleModusImage.enabled = true;
                dauerModusImage.enabled = false;
            }
                
        }

        if(player.GetCurrentWeapon().GetChangeBurstMode() && Input.GetKeyDown(KeyCode.B) && timerToChangeFireMode >= .8f)
        {
            //changeFireModeSfx.Play();
            player.GetCurrentWeapon().SetBurstMode();
            timerToChangeFireMode = 0;
        }

        weaponNameText.text = weaponName;
        burstModeText.text = burstMode ? "Auto : Dauer" : "Auto : Einzel";
        currentAmmoText.text = currentAmmo.ToString();
        currentAmmoVorratText.text = currentAmmoVorrat.ToString();

    }

    public void Reload()
    {

        if (currentAmmoVorrat <= 0) return;
        isReloading = true;

        StartCoroutine(FinishReload());
    }

    IEnumerator FinishReload()
    {
        yield return new WaitForSeconds(player.GetCurrentWeapon().GetReloadSpeed());

        newAmount = 0;

        int ammoUsed = Mathf.Abs(player.GetCurrentWeapon().GetCurrentStartAmmo() - currentAmmo);

        for (int i = 0; i < ammoUsed; i++)
        {
            if (currentAmmoVorrat <= 0) break;
            currentAmmo += 1;
            currentAmmoVorrat -= 1;
        }

        isReloading = false;
    }

    public void GetMoreAmmoVorrat(int newAmmo)
    {
        currentAmmoVorrat = Mathf.Max(currentAmmoVorrat + newAmmo, player.GetCurrentWeapon().GetMaxAmmoVorrat());
    }

    public void Shoot()
    {
        currentAmmo = Mathf.Max(currentAmmo - 1, 0);
    }
    public void UpdateWeaponUI()
    {
        currentAmmo = player.GetCurrentWeapon().GetCurrentStartAmmo();
        currentAmmoVorrat = player.GetCurrentWeapon().GetStartClip();
        weaponName = player.GetCurrentWeapon().GetWeaponName();
        burstMode = player.GetCurrentWeapon().GetBurstMode();
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public bool GetIfReloading()
    {
        return isReloading;
    }
}
