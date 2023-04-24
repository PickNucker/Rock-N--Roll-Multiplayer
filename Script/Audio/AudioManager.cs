using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    Dictionary<string, AudioClip> allSounds;

    private void Awake()
    {
        instance = this;

        allSounds = new Dictionary<string, AudioClip>();

        foreach (AudioClip ac in Resources.LoadAll<AudioClip>("Audio"))
        {
            allSounds.Add(ac.name, ac);
        }
    }

    public static AudioClip GetSound(string audioClip)
    {
        return instance.allSounds[audioClip];
    }
}
