using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOff : MonoBehaviour
{
    public GameObject go;

    private void Start()
    {
        go.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("SlapR1") != 0 )
        {
            go.SetActive(false);
        }
    }
}
