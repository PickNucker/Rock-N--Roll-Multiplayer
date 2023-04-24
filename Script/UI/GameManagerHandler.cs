using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerHandler : MonoBehaviour
{
    public static GameManagerHandler instance;

    public PhotonView m_GameManager_View;

    private void Awake()
    {
        instance = this;
        m_GameManager_View = GetComponent<PhotonView>();
    }

    public SoundAudioClip[] soundArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
