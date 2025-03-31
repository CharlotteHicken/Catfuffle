using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    // Start is called before the first frame update
    void Start()
    {
        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Jump1") != 0)
        {
            player1.SetActive(true);
        }
        if (Input.GetAxis("Jump2") != 0)
        {
            player2.SetActive(true);
        }
        if (Input.GetAxis("Jump3") != 0)
        {
            player3.SetActive(true);
        }
        if (Input.GetAxis("Jump4") != 0)
        {
            player4.SetActive(true);
        }
    }
}
