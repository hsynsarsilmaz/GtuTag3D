using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.IO;

public class Networking : MonoBehaviour
{
    public string serverIP;
    public int serverPort;
    public static Networking instance;
    IPAddress ipAddress;
    TcpClient client;
    NetworkStream stream;
    string message;
    byte[] data = new byte[3500];

    class helloRequest
    {
        public string type;
        public string response;
    }

    class statusRequest
    {
        public string type;
        public string response;
    }

    class startRequest
    {
        public string type;
        public string response;
    }

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

        // Read server IP from ipconfig.conf
        try
        {
            string ipConfigPath = Path.Combine(Application.dataPath, "ipconfig.conf");
            serverIP = File.ReadAllText(ipConfigPath).Trim();
        }
        catch (IOException e)
        {
            Debug.LogError("Could not read IP address from file: " + e);
            return;
        }

        serverPort = 3389;

        // Connect to server
        ipAddress = IPAddress.Parse(serverIP);
        client = new TcpClient();

        try
        {
            client.Connect(ipAddress, serverPort);
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
        }

        Debug.Log("Connection established");
        stream = client.GetStream();
        helloRequest request = new helloRequest();
        request.type = "Hello";
        byte[] responseData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));
        stream.Write(responseData, 0, responseData.Length);
        byte[] data = new byte[256];
        int bytes = stream.Read(data, 0, data.Length);
        helloRequest response = JsonUtility.FromJson<helloRequest>(Encoding.UTF8.GetString(data, 0, bytes));
        Debug.Log(response.response);
    }

    public bool signUp(string request)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(request);
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
        if (message == "Taken")
        {
            return false;
        }
        else if (message == "Done")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool login(string request)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(request);
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
        if (message == "Done")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string createGame(string request)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(request);
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
        return message;
    }

    public string joinGame(string request)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(request);
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
        return message;
    }

    public string askLobbyStatus(string request)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(request);
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
        return message;
    }

    public void changeStatus()
    {
        statusRequest request = new statusRequest();
        request.type = "Status";
        byte[] requestData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
    }

    public bool canGameBegin(string request)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(request);
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
        return message == "Yes";
    }

    public void startGame()
    {
        startRequest request = new startRequest();
        request.type = "Start";
        byte[] requestData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
    }

    public string getPlayerData(string request)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(request);
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
        return message;
    }

    public string updateMyPos(string request)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(request);
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
        return message;
    }

    public string freeze(string request)
    {
        byte[] requestData = Encoding.UTF8.GetBytes(request);
        stream.Write(requestData, 0, requestData.Length);
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.UTF8.GetString(data, 0, bytes);
        return message;
    }


    void Update()
    {


    }
}
