using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SessionDisplay : MonoBehaviour
{
    public void OnConnectionSuccess(int sessionID)
    {
        TryGetComponent(out TextMeshProUGUI display);
        display.text = $"Connected to server - session ID: {sessionID}";
    }

    public void OnConnectionFailed(string error)
    {
        TryGetComponent(out TextMeshProUGUI display);
        display.text = $"Error connecting to serevr: {error}";
    }
}
