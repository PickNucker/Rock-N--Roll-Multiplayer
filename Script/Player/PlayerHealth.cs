using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    [SerializeField] Image healthBar;
    [SerializeField] Text healthProzentage;

    [SerializeField] float health = 100f;

    float maxHealth;

    PhotonView m_View;
    M_GameManager playerManager;
    Animator anim;
    PlayerMovement player;

    float timer = 0;

    private void Awake()
    {
        m_View = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        playerManager = PhotonView.Find((int)m_View.InstantiationData[0]).GetComponent<M_GameManager>();
        maxHealth = health;

        if (!m_View.IsMine)
        {
            Destroy(healthBar.gameObject);
            Destroy(healthProzentage.gameObject);
        }
    }

    void Update()
    {
        if (!m_View.IsMine) return;
        timer += Time.deltaTime;
    }

    public void Damage(float dmg)
    {
        //if (timer < 2f) return;
        m_View.RPC("RPC_TakeDamage", RpcTarget.All, dmg);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!m_View.IsMine) return;

        health = Mathf.Max(health - damage, 0);

        healthBar.fillAmount = Mathf.Max(health / maxHealth, 0);
        healthProzentage.text = ((health / maxHealth) * 100).ToString("F0") + "%";

        if (health <= 0)
        {
            playerManager.AddNewDeath();
            Die();
            anim.SetTrigger("isDead");
        }
    }

    void Die()
    {
        playerManager.Die();
    }

    public bool GetDeath()
    {
        if (health <= 0) return true;
        else return false;
    }
}
