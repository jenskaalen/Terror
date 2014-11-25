using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(EndGameHandling))]
    public class GameManager : MonoBehaviour
    {
        private GamePlayer _gamePlayer;
        private GameInfo _gameInfo;

        private void Awake()
        {
            _gamePlayer = FindObjectOfType<GamePlayer>();
            _gameInfo = FindObjectOfType<GameInfo>();
            DontDestroyOnLoad(_gamePlayer);
            DontDestroyOnLoad(_gameInfo);
            DontDestroyOnLoad(this);

            string sceneName = Application.loadedLevelName;
            Debug.Log(sceneName);

            switch (sceneName)
            {
                case "mainmenu":
                    break;
                case "lobby":
                    LoadLobby();
                    break;
                case "demoLevel":
                    LoadDemoLevel();
                    break;
                default:
                    throw new Exception("Scene load not implemented: " + sceneName);
            }

        }

        private void LoadLobby()
        {
            if (_gamePlayer.State == PlayerState.ConnectingAsClient)
            {
                Network.Connect(_gamePlayer.ServerIp, _gamePlayer.ServerPort);
                Debug.Log("Connected to lobby");
                _gamePlayer.State = PlayerState.Connected;

            }
            else if (_gamePlayer.State == PlayerState.HostingLobby)
            {
                _gamePlayer.IsHost = true;
                Network.InitializeServer(8, _gamePlayer.ServerPort, false);
                Debug.Log("Lobby hosted");
            }
            else
            {
                throw new NotImplementedException("Player state not supported");
            }
        }

        private void LoadDemoLevel()
        {
            Debug.Log("Current connections " + Network.connections.Count());

            if (_gamePlayer.State == PlayerState.ConnectingAsClient)
            {
                Network.Connect(_gamePlayer.ServerIp, _gamePlayer.ServerPort);
                _gamePlayer.State = PlayerState.Connected;

            }
            else if (_gamePlayer.State == PlayerState.HostingLobby)
            {
                Debug.Log("Lobby hosted");
            }
            else
            {
                //TODO: urgh, fix this, just doing it for the quicjfixies
                if (Network.isClient)
                {
                    Debug.Log("Letting server know we've loaded");
                    Network.Connect(_gamePlayer.ServerIp, _gamePlayer.ServerPort);
                    _gamePlayer.State = PlayerState.Connected;

                    var sceneplay = FindObjectOfType<LevelManager>();
                    sceneplay.networkView.RPC("PlayerLoadedLevel", RPCMode.Server);
                }
            }
        }

        public void StartServer()
        {
            Network.InitializeServer(8, 2000, false);
        }
    }
}
