using UnityEngine;
using Photon.Pun;
using System.IO;

[System.Serializable]
public class AudioTriggerFX
{
    [SerializeField] string[] audioName;
    [SerializeField] bool playOnAwake = true;
    [SerializeField] bool loop = false;
    [SerializeField] [Range(0, 1)] float volume = .2f;
    [SerializeField] float minPitch = 0.95f;
    [SerializeField] float maxPitch = 1.05f;
    [SerializeField] [Range(0, 1)] float threeDBlend = 1f;
    [SerializeField] float maxDistance = 100f;

    public void Play(Vector3 pos)
    {
        if (audioName.Length == 0) return;

        GameObject go = new GameObject();
        AudioSource source = go.AddComponent<AudioSource>();
        go.transform.position = pos;

        source.playOnAwake = playOnAwake;
        source.loop = loop;
        source.volume = volume;
        source.spatialBlend = threeDBlend;
        source.pitch = minPitch + Random.value * (minPitch - maxPitch);
        source.maxDistance = maxDistance;
        source.clip = AudioManager.GetSound(audioName[Random.Range(0, audioName.Length)]);

        source.rolloffMode = AudioRolloffMode.Linear;
        source.dopplerLevel = 0;

        source.Play();
        MonoBehaviour.Destroy(source.gameObject, source.clip.length + .5f);
        //Transform.FindObjectOfType<M_SpawnPlayer>().DestroyGameObject(source.gameObject);
    }
    public void Play(Vector3 pos, GameObject newParent)
    {
        if (audioName.Length == 0) return;

        GameObject go = new GameObject();
        AudioSource source = go.AddComponent<AudioSource>();
        go.transform.position = pos;
        newParent.transform.parent = go.transform;

        source.playOnAwake = playOnAwake;
        source.loop = loop;
        source.volume = volume;
        source.spatialBlend = threeDBlend;
        source.pitch = minPitch + Random.value * (minPitch - maxPitch);
        source.maxDistance = maxDistance;
        source.clip = AudioManager.GetSound(audioName[Random.Range(0, audioName.Length)]);

        source.rolloffMode = AudioRolloffMode.Linear;
        source.dopplerLevel = 0;

        source.Play();
        MonoBehaviour.Destroy(source.gameObject, source.clip.length + .5f);
        //Transform.FindObjectOfType<M_SpawnPlayer>().DestroyGameObject(source.gameObject);
    }

    public void ChangeMaxDistance(float distance)
    {
        maxDistance = distance;
    }

    public void ChangeVolume(float newVolume)
    {
        volume = newVolume;
    }

}
