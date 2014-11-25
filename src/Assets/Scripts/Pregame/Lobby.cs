using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Pregame
{
    public class Lobby : MonoBehaviour
    {
        private GamePlayer _gamePlayer;
        private GameInfo _gameInfo;

        private void Awake()
        {
            _gamePlayer = FindObjectOfType<GamePlayer>();
            _gameInfo = FindObjectOfType<GameInfo>();
            _gamePlayer = FindObjectOfType<GamePlayer>();
            Debug.Log("is host? " + _gamePlayer.IsHost);

            if (_gamePlayer.IsHost)
                ServerInitializeSelfForPlay();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(250));
            GUILayout.Label("Connected players:");

            foreach (var player in _gameInfo.PlayerInfos)
            {
                var style = new GUIStyle();
                style.normal.textColor = Color.red;

                GUILayout.BeginVertical();
                GUILayout.Label(player.PlayerName, style);
                GUILayout.Label(String.Format("Picked: {0}", player.CharacterPicked));
                GUILayout.Label("--------------------------");
                GUILayout.EndVertical();
            }


            GUILayout.EndVertical();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Human"))
            {
                _gamePlayer.CharacterPicked = "Human";
                networkView.RPC("UpdatePlayerSelection", RPCMode.AllBuffered, _gamePlayer.CharacterPicked, _gamePlayer.PlayerName);
            }
            
            if (GUILayout.Button("Mary"))
            {
                _gamePlayer.CharacterPicked = "Mary";
                networkView.RPC("UpdatePlayerSelection", RPCMode.AllBuffered, _gamePlayer.CharacterPicked, _gamePlayer.PlayerName);
            }
            GUILayout.EndHorizontal();

            if (GUI.Button(new Rect(10, 400, 150, 100), "Start game"))
            {
                networkView.RPC("StartGame", RPCMode.AllBuffered);
            }
        }

        [RPC]
        private void StartGame()
        {
            Application.LoadLevel("demoLevel");       
        }

        [RPC]
        public void JoinLobby(NetworkPlayer player, string name)
        {
            var playerinfo = new PlayerInfo();
            playerinfo.PlayerName = name;
            playerinfo.Player = player;

            _gameInfo.PlayerInfos.Add(playerinfo);

            Debug.Log(name + " joined lobby. PlayerInfo is now " + _gameInfo.PlayerInfos.Count);
        }

        private void OnConnectedToServer()
        {
            var gameplayer = GameObject.FindObjectOfType<GamePlayer>();
            networkView.RPC("JoinLobby", RPCMode.AllBuffered, Network.player, gameplayer.PlayerName);

            Debug.Log("Connected to server!");
            PlayerRegisterSelfForPlay();
        }

        private void OnPlayerConnected(NetworkPlayer player)
        {
            Debug.Log("Player connected from ip " + player.ipAddress);
        }

        private void OnServerInitialized()
        {
            Debug.Log("Server has been initlaized");
            var gameplayer = GameObject.FindObjectOfType<GamePlayer>();
            //JoinLobby(Network.player, gameplayer.name);
            networkView.RPC("JoinLobby", RPCMode.AllBuffered, Network.player, gameplayer.PlayerName);
        }

        private void OnPlayerDisconnected(NetworkPlayer player)
        {
            Debug.Log("a player has been disconnected");
        }

        [RPC]
        private void UpdatePlayerSelection(string pickedCharacter, string playerName)
        {
            var playerinfo = _gameInfo.PlayerInfos.FirstOrDefault(player => player.PlayerName == playerName);

            if (playerinfo == null)
                throw new Exception("Lobbyplayer not found " + playerName);

            playerinfo.CharacterPicked = pickedCharacter;
        }

        private void PlayerRegisterSelfForPlay()
        {
            _gameInfo.networkView.RPC("RegisterPlayer", RPCMode.AllBuffered, _gamePlayer.PlayerName, Network.player);
        }

        private void ServerInitializeSelfForPlay()
        {
            Debug.Log("Player connected to server. Creating player object");
            _gameInfo.networkView.RPC("RegisterPlayer", RPCMode.AllBuffered, _gamePlayer.PlayerName, Network.player);
        }
    }
}  