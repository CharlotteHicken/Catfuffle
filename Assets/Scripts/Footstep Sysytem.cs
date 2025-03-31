using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSoundManager : MonoBehaviour
{
    public AudioClip footstepSoundR;
    public AudioClip footstepSoundL;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = footstepSoundL;
        audioSource.playOnAwake = false;
    }

    public void PlayFootstepSoundL()
    {
        //Debug.Log("hi");
        audioSource.PlayOneShot(footstepSoundL);
    }

    public void PlayFootstepSoundR()
    {
        audioSource.PlayOneShot(footstepSoundR);
    }
}
