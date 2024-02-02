using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;
    public bool isSound = true;
    public bool isMusic = true;
    public bool isConnected = false;
    public string userName = "";
    public string password = "";
    public string gameId = "";
    public string myTeam = "";
    public bool creator = false;
    public int gameState = -1; //1 -> Lobby , 2-> In play
    public int myStatus = -1; //0 -> Not ready, 1 -> Ready

    public int team1Score = 0;
    public int team2Score = 0;
    public int team3Score = 0;
    public int team4Score = 0;



    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {

    }
}
