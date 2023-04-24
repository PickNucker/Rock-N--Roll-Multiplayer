using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    [SerializeField] float timerToDestroy = 3f;

    void Start()
    {
        Destroy(this.gameObject, timerToDestroy);
    }
}
