using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniControlerSRP : MonoBehaviour
{
    public Animator ani;
    public bool isJumping;
    public bool isWalking;

    public void Update()
    {

        if (Input.GetAxis("Jump") != 0)
        {
            ani.SetBool("isJumping", true);
            isJumping = true;
        }
        else
        {
            ani.SetBool("isJumping", false);
            isJumping = false;
        }

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            ani.SetBool("isWalking", true);
            isWalking = true;
        }
        else
        {
            ani.SetBool("isWalking", false);
            isWalking = false;
        }

    }

}
