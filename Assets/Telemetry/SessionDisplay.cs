using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SessionDisplay : MonoBehaviour
{
    public void OnConnectionSuccess(int SessionID)
    {
        TryGetComponent(out TextMeshProUGUI display);
        display.text = $"Conncected to server - session ID: {SessionID}";

    }
    public void OnConnectionFall(string error)
    {
        TryGetComponent(out TextMeshProUGUI display);
        display.text = $"Error connecting to server: {error}";
    }
}