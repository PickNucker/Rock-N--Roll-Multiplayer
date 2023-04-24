using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class SoundManager
{
    public enum Sound
    {
        Ak47_Shoot
    }

    public static void PlaySound(Sound sound)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource source = soundGameObject.AddComponent<AudioSource>();
        GameManagerHandler.instance.m_GameManager_View.RPC("M_PlaySound", RpcTarget.All, sound, source, GameManagerHandler.instance.m_GameManager_View);

    }

    static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameManagerHandler.SoundAudioClip soundAudioClip in GameManagerHandler.instance.soundArray)
        {
            if(soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + "not Found!");
        return null;
    }
}
