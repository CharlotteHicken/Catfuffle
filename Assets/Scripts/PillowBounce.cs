using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowBounce : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rb;
    public float verticalBoost;
    void Start()
    {
      
    }

    // Update is called once per frame
 
    private void OnTriggerStay(Collider other)
    {
        rb = other.GetComponent<Rigidbody>();
        if(rb != null)
        {

            
            Vector3 finalForce = (Vector3.up * verticalBoost *2);
            rb.AddForce(finalForce, ForceMode.Impulse);
        }
    }
}
