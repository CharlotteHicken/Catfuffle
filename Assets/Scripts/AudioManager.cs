using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip Grab;
    public AudioClip Hitting;
    public AudioClip Jumping;
    public AudioClip Swinging;
    public AudioClip Walking;

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}