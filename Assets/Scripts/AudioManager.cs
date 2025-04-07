using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource audioSource;

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
        musicSource.PlayOneShot(clip);
    }

    public void volumeZeroPointZeroOne()
    {
        musicSource.volume = 0.01f;
    }

    public void volumeZeroPointTwo()
    {
        musicSource.volume = 0.2f;
    }
}