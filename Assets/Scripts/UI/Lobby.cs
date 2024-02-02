using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    private GameData gameData;
    private Networking networking;

    private List<List<GameObject>> cards;

    private Color redColor = new Color(0.9098f, 0.2549f, 0.0941f);
    private Color greenColor = new Color(0.2666f, 0.7411f, 0.1960f);

    class isBeginRequest
    {
        public string type;
    }

    class isLobbyRequest
    {
        public string type;
    }

    class lobbyResponse
    {

        public string type;
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
        public string player12;

    }

    class lobbyPlayer
    {
        public int team;
        public string username;
        public string status;
    }

    string beginRequest;
    string lobbyRequest;

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        networking = FindObjectOfType<Networking>();
        cards = new List<List<GameObject>>();
        for (int i = 0; i < 4; i++)
        {
            cards.Add(new List<GameObject>());
            for (int j = 0; j < 3; j++)
            {
                cards[i].Add(this.transform.GetChild(i).GetChild(1).GetChild(j).gameObject);
            }
        }
        isBeginRequest request = new isBeginRequest();
        request.type = "isBegin";
        beginRequest = JsonUtility.ToJson(request);
        isLobbyRequest request2 = new isLobbyRequest();
        request2.type = "Lobby";
        lobbyRequest = JsonUtility.ToJson(request2);

        //create a timer to check lobby status
        InvokeRepeating("askLobbyStatus", 0.0f, 0.64f);
        //CancelInvoke("askLobbyStatus");
    }

    // Update is called once per frame

    void askLobbyStatus()
    {
        if (gameData.creator && networking.canGameBegin(beginRequest))
        {
            System.Threading.Thread.Sleep(1000);
            gameData.gameState = 2;
            networking.startGame();
            SceneManager.LoadScene("Game");
            return;
        }

        if (gameData.gameState == 1)
        {
            lobbyResponse response = JsonUtility.FromJson<lobbyResponse>(networking.askLobbyStatus(lobbyRequest));
            lobbyPlayer[] lobbyPlayers = new lobbyPlayer[12];
            lobbyPlayers[0] = JsonUtility.FromJson<lobbyPlayer>(response.player0);
            lobbyPlayers[1] = JsonUtility.FromJson<lobbyPlayer>(response.player1);
            lobbyPlayers[2] = JsonUtility.FromJson<lobbyPlayer>(response.player2);
            lobbyPlayers[3] = JsonUtility.FromJson<lobbyPlayer>(response.player3);
            lobbyPlayers[4] = JsonUtility.FromJson<lobbyPlayer>(response.player4);
            lobbyPlayers[5] = JsonUtility.FromJson<lobbyPlayer>(response.player5);
            lobbyPlayers[6] = JsonUtility.FromJson<lobbyPlayer>(response.player6);
            lobbyPlayers[7] = JsonUtility.FromJson<lobbyPlayer>(response.player7);
            lobbyPlayers[8] = JsonUtility.FromJson<lobbyPlayer>(response.player8);
            lobbyPlayers[9] = JsonUtility.FromJson<lobbyPlayer>(response.player9);
            lobbyPlayers[10] = JsonUtility.FromJson<lobbyPlayer>(response.player10);
            lobbyPlayers[11] = JsonUtility.FromJson<lobbyPlayer>(response.player11);
            if (response.type == "Begin")
            {
                gameData.gameState = 2;
                SceneManager.LoadScene("Game");
                return;
            }
            int[] teamMemberCount = new int[4];
            for (int i = 0; i < 4; i++)
            {
                teamMemberCount[i] = 0;
            }
            foreach (lobbyPlayer player in lobbyPlayers)
            {
                if (player.username != "Empty")
                {
                    GameObject card = cards[player.team - 1][teamMemberCount[player.team - 1]];
                    teamMemberCount[player.team - 1]++;
                    if (player.status == "ready")
                    {
                        gameData.myStatus = 1;
                        card.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = greenColor;
                        card.transform.GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Ready";
                    }
                    else
                    {
                        gameData.myStatus = 0;
                        card.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = redColor;
                        card.transform.GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Not Ready";
                    }
                    if (card.transform.GetChild(0).gameObject.activeSelf == false)
                    {
                        card.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = player.username;
                        card.transform.GetChild(0).gameObject.SetActive(true);
                        card.transform.GetChild(1).gameObject.SetActive(true);
                        card.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }

            }
        }
    }
}