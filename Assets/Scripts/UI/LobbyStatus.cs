using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyStatus : MonoBehaviour
{
    private GameData gameData;
    private Networking networking;

    private Color redColor = new Color(0.9098f, 0.2549f, 0.0941f);
    private Color greenColor = new Color(0.2666f, 0.7411f, 0.1960f);

    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        networking = FindObjectOfType<Networking>();
    }

    public void changeMyStatus()
    {
        if (gameData.myStatus == 1)
        {
            gameData.myStatus = 0;
            GetComponent<TMPro.TextMeshProUGUI>().color = redColor;
            GetComponent<TMPro.TextMeshProUGUI>().text = "Not Ready";
        }
        else
        {
            gameData.myStatus = 1;
            GetComponent<TMPro.TextMeshProUGUI>().color = greenColor;
            GetComponent<TMPro.TextMeshProUGUI>().text = "Ready";
        }
        networking.changeStatus();
    }
}
