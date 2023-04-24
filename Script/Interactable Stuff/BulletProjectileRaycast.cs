using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectileRaycast : MonoBehaviourPun
{
    [SerializeField] float moveSpeed = 200f;

    public Vector3 target;

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }

    public void ChangeSpeed(float speed)
    {
        moveSpeed = speed;
    }

    void Update()
    {
        float beforDistance = Vector3.Distance(transform.position, target);

        Vector3 moveDir = (target - transform.position).normalized;

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfter = Vector3.Distance(transform.position, target);

        if(beforDistance < distanceAfter)
        {
            //Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        Destroy(gameObject);
    }
}
