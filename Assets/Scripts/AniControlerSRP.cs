using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniControlerSRP : MonoBehaviour
{
    public Animator ani;

    public void Update()
    {
        ani.SetFloat("jump", Input.GetAxis("Jump"));
        ani.SetFloat("horizontal", Input.GetAxis("Horizontal"));
    }

}
