using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour
{
    public int targetSceneIndex; // The scene index that triggers the music    
    public static AudioManager instance;

    public AudioMixerGroup mixerGroup;
    public Sound[] sounds;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds)
        {   
            //kiedy dodam spatialBlend/3d DŸwiêki to te audio source'y powinny siê pojawiaæ w obiekatach z których dŸwiêk siê wydaje
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixerGroup;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartMusic();
    }

    public void StartMusic()
    {
        targetSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Active Scene Index: " + targetSceneIndex);

        Stop("levelMusic"); //stop the music if it's still playing after reloading the scene

        
        if (targetSceneIndex == 0) // Assuming 0 is the Level
        {
            Debug.Log("Playing LevelMusic");
            Play("levelMusic");
        }
    }

    public void Play(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        int clipCount = s.clips.Length;
        s.source.clip = s.clips[clipCount > 1 ? Random.Range(0, clipCount) : 0];


        s.source.volume = s.volume * (1f + Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        s.source.time = s.startTime; // Set playback position
        s.source.Play();

        if (s.duration > 0f)
        {
            DOVirtual.DelayedCall(s.duration, () => s.source.Stop());
        }
    }

    public void Stop(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        if (s.source != null)
        {
            s.source.Stop();
            s.source.time = 0f; // Resetowanie pozycji odtwarzania
        }
    }

}
