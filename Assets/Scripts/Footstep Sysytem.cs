using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSysytem : MonoBehaviour
{
    public AudioSource FootSteps;

    public void Update()
    {
        if(Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.D)))))
        {
            FootSteps.enabled = true;
        }
        else
        {
            FootSteps.enabled = false; 
        }
    }
}
