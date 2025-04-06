using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    bool isPlayer1 = false;
    bool isPlayer2 = false;
    bool isPlayer3 = false;
    bool isPlayer4 = false;
    public GameObject player1Icon;
    public GameObject player2Icon;
    public GameObject player3Icon;
    public GameObject player4Icon;
    public GameObject menuScreen;
    public GameObject tutorialScreen;
    bool menuResetTime = true;
    bool menuOn = true;
    bool tutorialOn = false;
    public float gameTimeLength = 300.0f;
    float currentTime = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (menuResetTime)
        {
            MenuRestart();
        }

        if (menuOn)
        {
            if (Input.GetAxis("Jump1") != 0)
            {
                isPlayer1 = true;
                player1Icon.SetActive(true);
            }
            if (Input.GetAxis("Jump2") != 0)
            {
                isPlayer2 = true;
                player2Icon.SetActive(true);
            }
            if (Input.GetAxis("Jump3") != 0)
            {
                isPlayer3 = true;
                player3Icon.SetActive(true);
            }
            if (Input.GetAxis("Jump4") != 0)
            {
                isPlayer4 = true;
                player4Icon.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                tutorialOn = true;
                menuScreen.SetActive(false);
                menuOn = false;
            }
        }
        if (tutorialOn)
        {
            tutorialScreen.SetActive(true);
            if (Input.GetKey(KeyCode.Escape))
            {
                tutorialScreen.SetActive(false);
                tutorialOn = false;
            }
        }

        if (!menuOn && !tutorialOn)
        {
            if (isPlayer1)
            {
                player1.SetActive(true);
            }
            if (isPlayer2)
            {
                player2.SetActive(true);
            }
            if (isPlayer3)
            {
                player3.SetActive(true);
            }
            if (isPlayer4)
            {
                player4.SetActive(true);
            }

            currentTime += Time.deltaTime;

            if (currentTime > gameTimeLength)
            {
                //zoom into winner player by deactivating loser players
                //do another timer for a few seconds just zoomed on them
                //maybe some cool effects
                MenuRestart();
                currentTime = 0;
            }
        }
     
    }

    void MenuRestart()
    {
        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);
        player1Icon.SetActive(false);
        player2Icon.SetActive(false);
        player3Icon.SetActive(false);
        player4Icon.SetActive(false);
        menuScreen.SetActive(true);
        menuResetTime = false;
        menuOn = true;
        isPlayer1 = false;
        isPlayer2 = false;  
        isPlayer3 = false;
        isPlayer4 = false;
        //reset scores
        //reset player positions
        //any other resets needed go here!
    }
}
