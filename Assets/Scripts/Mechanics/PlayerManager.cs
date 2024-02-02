using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class PlayerManager : MonoBehaviour
{
    private GameData gameData;
    private Networking networking;
    private Dictionary<int, GameObject> players;
    public GameObject camera;
    public GameObject score1;
    public GameObject score2;
    public GameObject score3;
    public GameObject score4;
    public GameObject winner;


    private int myId;
    private Dictionary<int, Vector3> playerPositions;
    private Dictionary<int, Vector3> playerRotations;
    private Dictionary<int, int> playerTeams;
    private bool canMove = true;

    private string request;
    private string[] positions;

    class getPlayerRequest
    {
        public string type;
    }

    class myPosRequest
    {
        public string type;
        public float x;
        public float y;
        public float z;
        public float rx;
        public float ry;
        public float rz;

    }

    class getPlayerResponse
    {
        public int myid;
        public string player0;
        public string player1;
        public string player2;
        public string player3;
        public string player4;
        public string player5;
        public string player6;
        public string player7;
        public string player8;
        public string player9;
        public string player10;
        public string player11;
    }

    class myPosResponse
    {
        public int t1;
        public int t2;
        public int t3;
        public int t4;
        public string player0;
        public string player1;
        public string player2;
        public string player3;
        public string player4;
        public string player5;
        public string player6;
        public string player7;
        public string player8;
        public string player9;
        public string player10;
        public string player11;
    }

    class serverPlayer
    {
        public int id;
        public string username;
        public float x;
        public float y;
        public float z;
        public float rx;
        public float ry;
        public float rz;
        public int team;
    }

    class myPosPlayer
    {
        public int id;
        public float x;
        public float y;
        public float z;
        public float rx;
        public float ry;
        public float rz;
        public bool f;
    }

    class freezeRequest
    {
        public string type;
        public int id;
        public int team;
    }

    myPosRequest mypos;
    TMPro.TextMeshProUGUI scoreText1;
    TMPro.TextMeshProUGUI scoreText2;
    TMPro.TextMeshProUGUI scoreText3;
    TMPro.TextMeshProUGUI scoreText4;
    TMPro.TextMeshProUGUI winnerText;
    string[] scoreTexts = new string[4];

    private Dictionary<int, bool> frozenPlayers;
    // Start is called before the first frame update
    void Start()
    {
        //play music

        gameData = FindObjectOfType<GameData>();
        if (gameData.isMusic)
        {
            GetComponent<AudioSource>().Play();
        }
        networking = FindObjectOfType<Networking>();
        players = new Dictionary<int, GameObject>();
        playerPositions = new Dictionary<int, Vector3>();
        playerRotations = new Dictionary<int, Vector3>();
        frozenPlayers = new Dictionary<int, bool>();
        getPlayers();
        mypos = new myPosRequest();
        mypos.type = "mypos";
        Invoke("finishGame", 302);
        Thread t = new Thread(new ThreadStart(updatePlayerPos));
        t.Start();

    }

    void getPlayers()
    {
        getPlayerRequest request = new getPlayerRequest();
        request.type = "getPlayers";
        getPlayerResponse response = JsonUtility.FromJson<getPlayerResponse>(networking.getPlayerData(JsonUtility.ToJson(request)));
        serverPlayer[] serverPlayers = new serverPlayer[12];
        serverPlayers[0] = JsonUtility.FromJson<serverPlayer>(response.player0);
        serverPlayers[1] = JsonUtility.FromJson<serverPlayer>(response.player1);
        serverPlayers[2] = JsonUtility.FromJson<serverPlayer>(response.player2);
        serverPlayers[3] = JsonUtility.FromJson<serverPlayer>(response.player3);
        serverPlayers[4] = JsonUtility.FromJson<serverPlayer>(response.player4);
        serverPlayers[5] = JsonUtility.FromJson<serverPlayer>(response.player5);
        serverPlayers[6] = JsonUtility.FromJson<serverPlayer>(response.player6);
        serverPlayers[7] = JsonUtility.FromJson<serverPlayer>(response.player7);
        serverPlayers[8] = JsonUtility.FromJson<serverPlayer>(response.player8);
        serverPlayers[9] = JsonUtility.FromJson<serverPlayer>(response.player9);
        serverPlayers[10] = JsonUtility.FromJson<serverPlayer>(response.player10);
        serverPlayers[11] = JsonUtility.FromJson<serverPlayer>(response.player11);
        scoreText1 = score1.GetComponent<TMPro.TextMeshProUGUI>();
        scoreText2 = score2.GetComponent<TMPro.TextMeshProUGUI>();
        scoreText3 = score3.GetComponent<TMPro.TextMeshProUGUI>();
        scoreText4 = score4.GetComponent<TMPro.TextMeshProUGUI>();
        winnerText = winner.GetComponent<TMPro.TextMeshProUGUI>();
        int[] teamMemberCount = new int[4];
        for (int i = 0; i < 4; i++)
        {
            teamMemberCount[i] = 0;
        }
        foreach (serverPlayer player in serverPlayers)
        {
            if (player.username != "Empty")
            {
                players.Add(player.id, this.transform.GetChild(3 * (player.team - 1) + teamMemberCount[player.team - 1]).gameObject);
                frozenPlayers.Add(player.id, false);
                teamMemberCount[player.team - 1]++;
                players[player.id].transform.GetChild(8).GetComponent<TMPro.TextMeshPro>().text = player.username;
                players[player.id].transform.position = new Vector3(player.x, player.y, player.z);
                players[player.id].transform.eulerAngles = new Vector3(player.rx, player.ry, player.rz);
                playerPositions.Add(player.id, new Vector3(player.x, player.y, player.z));
                playerRotations.Add(player.id, new Vector3(player.rx, player.ry, player.rz));
                players[player.id].GetComponent<PlayerValues>().team = player.team;
                players[player.id].GetComponent<PlayerValues>().myId = player.id;
                players[player.id].SetActive(true);
            }
        }
        myId = response.myid;
        camera.transform.position = new Vector3(playerPositions[myId].x, playerPositions[myId].y + 18, playerPositions[myId].z - 20);
    }

    void FixedUpdate()
    {
        //if w of is pressed move forward or backward if no collision
        if (canMove && Input.GetKey(KeyCode.W))
        {
            if (!Physics.Raycast(players[myId].transform.position, players[myId].transform.forward, 1))
            {
                players[myId].transform.position += players[myId].transform.forward * 0.5f;
                //move camera forward
                camera.transform.position += players[myId].transform.forward * 0.5f;
            }
        }
        if (canMove && Input.GetKey(KeyCode.S))
        {
            if (!Physics.Raycast(players[myId].transform.position, -players[myId].transform.forward, 1))
            {
                players[myId].transform.position -= players[myId].transform.forward * 0.5f;
                //move camera backward
                camera.transform.position -= players[myId].transform.forward * 0.5f;
            }
        }

        //if a or d is pressed rotate left or right
        if (canMove && Input.GetKey(KeyCode.A))
        {
            players[myId].transform.eulerAngles += new Vector3(0, -1, 0);
            //rotate camera around player
            camera.transform.RotateAround(playerPositions[myId], Vector3.up, -1);
        }
        if (canMove && Input.GetKey(KeyCode.D))
        {
            players[myId].transform.eulerAngles += new Vector3(0, 1, 0);
            //rotate camera around player
            camera.transform.RotateAround(playerPositions[myId], Vector3.up, 1);
        }

        if (canMove && Input.GetKeyDown(KeyCode.Space))
        {
            List<GameObject> nearPlayers = new List<GameObject>();
            foreach (KeyValuePair<int, GameObject> player in players)
            {
                if (player.Key != myId)
                {
                    if (Vector3.Distance(playerPositions[myId], playerPositions[player.Key]) < 30)
                    {
                        nearPlayers.Add(player.Value);
                        Debug.Log(player.Value.transform.GetChild(8).GetComponent<TMPro.TextMeshPro>().text);
                    }
                }
            }
            if (nearPlayers.Count >= 2)
            {
                int[] teamMemberCount = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    teamMemberCount[i] = 0;
                }
                teamMemberCount[players[myId].GetComponent<PlayerValues>().team - 1]++;
                foreach (GameObject player in nearPlayers)
                {
                    teamMemberCount[player.GetComponent<PlayerValues>().team - 1]++;
                }
                if (teamMemberCount[players[myId].GetComponent<PlayerValues>().team - 1] >= 2)
                {
                    foreach (GameObject player in nearPlayers)
                    {
                        if (teamMemberCount[player.GetComponent<PlayerValues>().team - 1] < 2)
                        {
                            if (!frozenPlayers[player.GetComponent<PlayerValues>().myId])
                            {
                                freezeRequest request = new freezeRequest();
                            request.type = "freeze";
                            request.team = players[myId].GetComponent<PlayerValues>().team;
                            request.id = player.GetComponent<PlayerValues>().myId;
                            networking.freeze(JsonUtility.ToJson(request));
                            }
                        }
                    }
                }
            }
        }

        scoreText1.text = scoreTexts[0];
        scoreText2.text = scoreTexts[1];
        scoreText3.text = scoreTexts[2];
        scoreText4.text = scoreTexts[3];
        playerPositions[myId] = players[myId].transform.position;
        playerRotations[myId] = players[myId].transform.eulerAngles;

        foreach (KeyValuePair<int, bool> player in frozenPlayers)
        {
            if (player.Value)
            {
                players[player.Key].transform.GetChild(9).gameObject.SetActive(true);

            }
        }

        foreach (KeyValuePair<int, GameObject> player in players)
        {
            if (player.Key != myId)
            {
                player.Value.transform.position = playerPositions[player.Key];
                player.Value.transform.eulerAngles = playerRotations[player.Key];
            }
        }
    }

    void updatePlayerPos()
    {
        while (true)
        {
            mypos.x = playerPositions[myId].x;
            mypos.y = playerPositions[myId].y;
            mypos.z = playerPositions[myId].z;
            mypos.rx = playerRotations[myId].x;
            mypos.ry = playerRotations[myId].y;
            mypos.rz = playerRotations[myId].z;
            myPosResponse response;
            try
            {
                response = JsonUtility.FromJson<myPosResponse>(networking.updateMyPos(JsonUtility.ToJson(mypos)));
            }
            catch (Exception e)
            {
                Debug.Log(e);
                continue;
            }

            scoreTexts[0] = response.t1.ToString();
            scoreTexts[1] = response.t2.ToString();
            scoreTexts[2] = response.t3.ToString();
            scoreTexts[3] = response.t4.ToString();

            myPosPlayer[] myPosPlayers = new myPosPlayer[12];
            myPosPlayers[0] = JsonUtility.FromJson<myPosPlayer>(response.player0);
            myPosPlayers[1] = JsonUtility.FromJson<myPosPlayer>(response.player1);
            myPosPlayers[2] = JsonUtility.FromJson<myPosPlayer>(response.player2);
            myPosPlayers[3] = JsonUtility.FromJson<myPosPlayer>(response.player3);
            myPosPlayers[4] = JsonUtility.FromJson<myPosPlayer>(response.player4);
            myPosPlayers[5] = JsonUtility.FromJson<myPosPlayer>(response.player5);
            myPosPlayers[6] = JsonUtility.FromJson<myPosPlayer>(response.player6);
            myPosPlayers[7] = JsonUtility.FromJson<myPosPlayer>(response.player7);
            myPosPlayers[8] = JsonUtility.FromJson<myPosPlayer>(response.player8);
            myPosPlayers[9] = JsonUtility.FromJson<myPosPlayer>(response.player9);
            myPosPlayers[10] = JsonUtility.FromJson<myPosPlayer>(response.player10);
            myPosPlayers[11] = JsonUtility.FromJson<myPosPlayer>(response.player11);
            foreach (myPosPlayer player in myPosPlayers)
            {
                if (player.id != -1 && player.id != myId)
                {
                    playerPositions[player.id] = new Vector3(player.x, player.y, player.z);
                    playerRotations[player.id] = new Vector3(player.rx, player.ry, player.rz);
                }
                if (player.id != -1 && player.f)
                {
                    if (player.f)
                    {
                        frozenPlayers[player.id] = true;
                        if (player.id == myId)
                        {
                            canMove = false;
                        }
                    }
                }
            }
        }
    }

    void finishGame()
    {
        int[] scores = new int[4];
        scores[0] = int.Parse(scoreTexts[0]);
        scores[1] = int.Parse(scoreTexts[1]);
        scores[2] = int.Parse(scoreTexts[2]);
        scores[3] = int.Parse(scoreTexts[3]);
        if (scores[0] == scores[1] && scores[1] == scores[2] && scores[2] == scores[3])
        {
            winnerText.text = "Draw";
        }
        else
        {
            int max = 0;
            for (int i = 0; i < 4; i++)
            {
                if (scores[i] > max)
                {
                    max = scores[i];
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (scores[i] == max)
                {
                    winnerText.text = "Team " + (i + 1).ToString() + " has Won!";
                }
            }
        }
        winnerText.gameObject.SetActive(true);


    }
}



