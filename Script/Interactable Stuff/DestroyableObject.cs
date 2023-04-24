using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class DestroyableObject : MonoBehaviour, IDamagable
{
    [SerializeField] float health = 20f;
    [SerializeField] float explosionForce = 100f;
    [SerializeField] float explosionRadius = 2f;
    [SerializeField] Transform destroyedForm = default;
    [SerializeField] float timerToRespawn = 3f;

    PhotonView m_View;
    //GameObject b;
    Vector3 lastPoint;

    float maxHealth;
    float timer = 0;

    //bool active;
    bool spawnObject;

    // Start is called before the first frame update
    void Start()
    {
        m_View = GetComponent<PhotonView>();
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= timerToRespawn)
        {
            health = maxHealth;
            spawnObject = false;
            timer = 0;
            m_View.RPC("DisableObject", RpcTarget.All, true);

        }

        if (health <= 0)
        {
            timer += Time.deltaTime;

            if (!spawnObject)
            {
                m_View.RPC("DisableObject", RpcTarget.All, false);
                var b = PhotonNetwork.Instantiate(Path.Combine("prefabs", "Target - spliced"), transform.position, transform.rotation);

                foreach (Transform item in destroyedForm)
                {
                    if (item.transform.rotation.y == 180)
                        item.transform.rotation = Quaternion.Euler(0, 0, 0);
                }

                foreach (Transform item in destroyedForm)
                {
                    item.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigid);
                    rigid.AddExplosionForce(explosionForce, item.transform.position, explosionRadius);
                }

                //active = false;
                spawnObject = true;
            }
        }
    }

    public void Damage(float dmg)
    {
        health = Mathf.Max(health - dmg, 0);
        //lastPoint = pos;
    }

    [PunRPC]
    void DisableObject(bool activity)
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = activity;
        this.gameObject.GetComponent<MeshCollider>().enabled = activity;
    }
}
