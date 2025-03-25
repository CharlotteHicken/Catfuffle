using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSoundManager : MonoBehaviour
{
    public AudioClip footstepSound; 
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = footstepSound;
        audioSource.playOnAwake = false; 
    }

    public void PlayFootstepSound()
    {
        Debug.Log("hi");
        audioSource.PlayOneShot(footstepSound);
    }
}
