using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
public class Scoring : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI scoreCount;
    public float currentScore;

    private void Update()
    {
        
    }
    public void AddScore()
    {
            currentScore += 1; // Add 1 point per elimination
            scoreCount.text = ""+ currentScore; // Update the UI
        
    }
}


