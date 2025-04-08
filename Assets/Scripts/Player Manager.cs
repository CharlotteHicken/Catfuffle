using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NodeCanvas.Tasks.Actions;
using JetBrains.Annotations;
using System;
using System.Linq;
using Unity.VisualScripting;
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
    public GameObject winScreen;
    bool menuResetTime = true;
    bool menuOn = true;
    bool tutorialOn = false;
    public float gameTimeLength = 300;
    float currentTime = 0;
    public TextMeshProUGUI roundTimer;
    float timeElasped;
    public float winScreenTime = 10;
    int i = 0;
    public List<PlayerController> players;
    // Start is called before the first frame update

    public AudioManager audioManager;
    public bool playVictory = false;
    public bool playMenue = false;
    public bool playBattle = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        // Find all game objects with the tag "Player"
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        // Iterate over each found player object
        foreach (GameObject playerObject in playerObjects)
        {
            // Get the PlayerController component from the player object
            PlayerController playerController = playerObject.GetComponent<PlayerController>();

            // If the PlayerController exists on the player object, add it to the list
            if (playerController != null)
            {
                players.Add(playerController);  // Add the playerController to the list
                Debug.Log("Found player with PlayerController: " + playerController.gameObject.name);
            }

        }
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
            if (!playMenue)
            {
                audioManager.PlayMusic(audioManager.Menue);
                audioManager.volumeZeroPointZeroOne();
                playMenue = true;
            }

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

            if (Input.GetButtonDown("Button9") || Input.GetKeyDown(KeyCode.Space))
            {
                tutorialOn = true;
                menuScreen.SetActive(false);
                menuOn = false;
            }

        }
        
        if (tutorialOn)
        {
            tutorialScreen.SetActive(true);
            if (Input.GetButtonDown("ButtonO") || Input.GetKey(KeyCode.Escape))
            {
                tutorialScreen.SetActive(false);
                tutorialOn = false;
                playMenue = false;
            }
        }

        if (!menuOn && !tutorialOn)
        {
            GameTime();

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

           
           
            
           if (i < 4)
            {
                i = 0;
            }
            while (i < 4)
            {
                if (players[i].scoreTracker.currentScore >= 5)
                {
                    i++;
                    if (players[i].scoreTracker.currentScore <= 5)
                    {
                        players[i].gameObject.SetActive(false);
                    }
                }
                if (gameTimeLength <= 0)
                {
                    i++;
                    if (players[i].scoreTracker.currentScore < players[i + 1].scoreTracker.currentScore)
                    {
                        players[i].gameObject.SetActive(false);
                    }


                }
            }
        }

        void GameTime()
        {
            gameTimeLength -= Time.deltaTime; // Decrease time by delta time each frame


            int convertToMinutes = Mathf.FloorToInt(gameTimeLength / 60); // Convert seconds to minutes
            int convertToSeconds = Mathf.FloorToInt(gameTimeLength % 60); // Get remaining seconds after converting to minutes

            roundTimer.text = string.Format("{0:00}:{1:00}", convertToMinutes, convertToSeconds); // Format the timer display
            if (timeElasped >= gameTimeLength)
            {
                if (!playVictory)
                {
                    audioManager.PlayMusic(audioManager.Victory);
                    audioManager.volumeZeroPointTwo();
                    playVictory = true;
                }
                winMenu();
            }
            playVictory = false;
        }
  
        void winMenu()
        {
            //zoom into winner player by deactivating loser players
            //if (player 1 has highest score){
                player2.SetActive(false);
                player3.SetActive(false);
                player4.SetActive(false);
            //}
            //if (player 2 has highest score){
                player1.SetActive(false);
                player3.SetActive(false);
                player4.SetActive(false);
            //}
            //if (player 3 has highest score){
                player2.SetActive(false);
                player1.SetActive(false);
                player4.SetActive(false);
            //}
            //if (player 4 has highest score){
                player2.SetActive(false);
                player3.SetActive(false);
                player1.SetActive(false);
            //}
            winScreen.SetActive(true);
            roundTimer.text = " ";

            timeElasped += Time.deltaTime;
            if (timeElasped >= (winScreenTime + gameTimeLength))
            {
                timeElasped = 0;
                MenuRestart();
            }
            
            //maybe some cool effects


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
            winScreen.SetActive(false);
            menuResetTime = false;
            menuOn = true;
            isPlayer1 = false;
            isPlayer2 = false;
            isPlayer3 = false;
            isPlayer4 = false;
            gameTimeLength = 300;
          
            //reset scores
            //reset player positions
            //any other resets needed go here!
        }
    }
}
