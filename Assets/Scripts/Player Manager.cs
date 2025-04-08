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
    public TextMeshProUGUI roundTimer;
    float timeElasped;
    public float winScreenTime = 10;
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
            if (gameTimeLength > 0)
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
            }
        }

        void GameTime()
        {
            gameTimeLength -= Time.deltaTime; // Decrease time by delta time each frame


            int convertToMinutes = Mathf.FloorToInt(gameTimeLength / 60); // Convert seconds to minutes
            int convertToSeconds = Mathf.FloorToInt(gameTimeLength % 60); // Get remaining seconds after converting to minutes

            roundTimer.text = string.Format("{0:00}:{1:00}", convertToMinutes, convertToSeconds); // Format the timer display
            if (0 >= gameTimeLength)
            {
                if (!playVictory)
                {
                    audioManager.PlayMusic(audioManager.Victory);
                    audioManager.volumeZeroPointTwo();
                    playVictory = true;
                }
                DetermineWinner();
            }
        }

        void DetermineWinner()
        {
            PlayerController winner = null;
            float highestScore = -1;

            foreach (var player in players)
            {
                Debug.Log($"Player: {player.gameObject.name}, Score: {player.scoreTracker.currentScore}");
                if (player.scoreTracker.currentScore > highestScore)
                {
                    highestScore = player.scoreTracker.currentScore;
                    winner = player;
                }
            }

            if (winner != null)
            {
                Debug.Log($"Winner: {winner.gameObject.name} with Score: {highestScore}");
                ShowWinScreen(winner);
            }
            else
            {
                Debug.LogWarning("No winner found!");
            }
        }

        void ShowWinScreen(PlayerController winner)
        {
            Debug.Log("Showing win screen...");
            
            player1.SetActive(false); // Deactivate all players first
            player2.SetActive(false);
            player3.SetActive(false);
            player4.SetActive(false);
            
            winner.gameObject.SetActive(true); // Activate only the winner

            winScreen.SetActive(true);
          
            roundTimer.text = " ";
            timeElasped += Time.deltaTime;
            if (timeElasped >= (winScreenTime + gameTimeLength))
            {
                timeElasped = 0;
                playVictory = false;

                MenuRestart();
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
            winScreen.SetActive(false);
            menuResetTime = false;
            menuOn = true;
            isPlayer1 = false;
            isPlayer2 = false;
            isPlayer3 = false;
            isPlayer4 = false;

            players[0].scoreTracker.currentScore = 0f;
            players[1].scoreTracker.currentScore = 0f;
            players[2].scoreTracker.currentScore = 0f;
            players[3].scoreTracker.currentScore = 0f;
            players[0].scoreTracker.scoreCount.text =""+ players[0].scoreTracker.currentScore;
            players[1].scoreTracker.scoreCount.text = "" + players[1].scoreTracker.currentScore;
            players[2].scoreTracker.scoreCount.text = "" + players[2].scoreTracker.currentScore;
            players[3].scoreTracker.scoreCount.text = "" + players[3].scoreTracker.currentScore;

            gameTimeLength = 300;
          
            //reset scores
            //reset player positions
            //any other resets needed go here!
        }
    }
}
