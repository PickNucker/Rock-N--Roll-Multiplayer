using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public static CamShake instance;

    CinemachineVirtualCamera cam;

    float timer = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCam(float strenght, float timer)
    {
        CinemachineBasicMultiChannelPerlin camPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        camPerlin.m_AmplitudeGain = strenght;
        this.timer = timer;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            CinemachineBasicMultiChannelPerlin camPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            camPerlin.m_AmplitudeGain = 0;
        }
    }
}
