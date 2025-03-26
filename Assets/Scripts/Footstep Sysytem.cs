using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSysytem : MonoBehaviour
{
    public AudioSource FootSteps;

    public void Update()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            FootSteps.enabled = true;
        }
        else
        {
            FootSteps.enabled = false;
        }
    }

}
