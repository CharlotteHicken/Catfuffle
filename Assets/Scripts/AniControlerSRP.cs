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
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

}
