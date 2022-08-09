using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public SoundOrginizer[] soundHolders;
    [HideInInspector]
    public List<Sound> sounds;
    
    [HideInInspector]
    public static float masterVolume = .5f;

    public static AudioManager audioManager;

    void Awake()
    {
        if(ImportantObjs.audioManager == null)
        {
            ImportantObjs.audioManager = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        for(int i = 0; i < soundHolders.Length; i ++)
        {
            foreach(Sound s in soundHolders[i].sounds)
            {
                sounds.Add(s);
            }
        }
        

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    public Sound Play(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning(name + "doesn't exist you moron");
            return null;
        }
        
        s.source.Play();
        return s;
    }
    public void Stop(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning(name + "doesn't exist you moron");
            return;
        }
        s.source.Stop();
    }

    public void CallDefeatedBoss(string bossTheme)
    {
        StartCoroutine(DefeatedBoss(bossTheme));
    }
    public IEnumerator DefeatedBoss(string bossTheme)
    {
        Stop(bossTheme);
        yield return new WaitForSeconds(Play("Win").source.clip.length);
        Play("Ambiant Nature");
        //GameManager.gameStart = false;
    }

    public void StopAllSongs()
    {
        foreach(Sound sound in sounds)
        {
            Stop(sound.name);
        }
    }

    public static void ChangeMasterVolume(float newVolume)
    {
        AudioListener.volume = newVolume * 2;
    }
}

[System.Serializable]
public class SoundOrginizer
{
    public string name;
    public Sound[] sounds;
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [HideInInspector]
    public float adjustedVolume;

    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
