using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource musicSource2;

    [Header("Audio Clip")]
    public AudioClip Grab;
    public AudioClip Hit;
    public AudioClip Jumping;
    public AudioClip Swinging;
    public AudioClip Death;
    public AudioClip Pop;
    public AudioClip Bounce;
    public AudioClip Menue;
    public AudioClip Battle;
    public AudioClip Victory;

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
    public void PlayMusic(AudioClip clip)
    {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
        Debug.Log($"Now playing: {clip.name}");
    }
    public void PlayMusic2(AudioClip clip)
    {
        musicSource.Stop();
        musicSource2.Stop();
        musicSource2.clip = clip;
        musicSource2.Play();
        Debug.Log($"Now playing: {clip.name}");
    }

    public void volumeZeroPointZeroOne()
    {
        musicSource.volume = 0.01f;
    }

    public void volumeZeroPointTwo()
    {
        musicSource.volume = 0.2f;
    }
    
    public void volumeZeroPointTwo2()
    {
        musicSource.volume = 0.2f;
    }

    public void volumeOne()
    {
        musicSource.volume = 1;
    }

}