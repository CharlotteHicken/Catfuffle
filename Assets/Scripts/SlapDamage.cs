using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapDamage : MonoBehaviour
{
    // Start is called before the first frame update

    float damage = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            float hits = other.GetComponent<PlayerController>().hitCount;
            hits =- damage;
            Debug.Log(hits);
        }
       
    }
}
