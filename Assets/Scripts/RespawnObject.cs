using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObject : MonoBehaviour
{
    Vector3 startPosition;
    bool isDying;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying)
        {
            Dying();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("in trigger enter");
        if (other.CompareTag("objectKillVolume"))
        {
            isDying = true;
        }
    }

    void Dying()
    {
        transform.position = startPosition;
        rb.velocity = Vector3.zero;
        isDying = false;
    }
}
