using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Android.Gradle;

public class SessionDisplay : MonoBehaviour
{
    public void OnConnectionSuccess(int sessionID)
    {
        var displayField = GetComponent<TextMeshProUGUI>();
        if (sessionID < 0)
        {
            displayField.text = $"Logging locally (Session {sessionID})";
        }
        else
        {
            displayField.text = $"Connected to Server (Session {sessionID})";
        }
    }

    public void OnConnectionFail(string errorMessage)
    {
        var displayField = GetComponent<TextMeshProUGUI>();
        displayField.text = $"Error: {errorMessage}";
    }
}
