using System;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using System.Collections;
using Random = System.Random;

public class Connecter : MonoBehaviour
{
    public Transform PlayerPrefab;

    private string _serverIp = "127.0.0.1";
    //TODO: make dynamic
    private int _port = 2000;
    private GamePlayer _gamePlayer;

    private void Awake()
    {
        _gamePlayer = GameObject.FindObjectOfType<GamePlayer>();
        DontDestroyOnLoad(_gamePlayer);
    }

	// Update is called once per frame
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Server ip:");
        _serverIp = GUI.TextField(new Rect(10, 35, 200, 20), _serverIp, 25);

	    if (GUI.Button(new Rect(50, 250, 200, 20), "Connect"))
	    {
	        _gamePlayer.ServerIp = _serverIp;
	        _gamePlayer.ServerPort = _port;
            _gamePlayer.State = PlayerState.ConnectingAsClient;
	        _gamePlayer.PlayerName = GenerateRandomName();
            Application.LoadLevel("lobby");
        }

        if (GUI.Button(new Rect(50, 300, 200, 20), "Host a lobby"))
        {
            //BecomeAServer();
            _gamePlayer.ServerPort = _port;
            _gamePlayer.State = PlayerState.HostingLobby;
            _gamePlayer.PlayerName = GenerateRandomName();
            Application.LoadLevel("lobby");
        }
	}

    private string GenerateRandomName()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var result = new string(
            Enumerable.Repeat(chars, 5)
                      .Select(s => s[random.Next(s.Length)])
                      .ToArray());
        return result;
    }
}