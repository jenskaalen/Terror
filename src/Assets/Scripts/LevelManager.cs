using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    /// <summary>
    /// Added to a gameobject on all scenes
    /// </summary> 
    [RequireComponent(typeof(NetworkView))]
    public class LevelManager : MonoBehaviour
    {
        private GamePlayer _gamePlayer;
        private GameInfo _gameInfo;
        public Transform HumanPrefab;
        public Transform MaryPrefab;
        private readonly Vector3 _humanSpawnPosition = new Vector3(0, 1, 0);

        private int _connectedPlayers = 0;
        private bool _gameStarted;

        private void Awake()
        {
            _gameInfo = FindObjectOfType<GameInfo>();
            _gamePlayer = FindObjectOfType<GamePlayer>();
            Debug.Log("is host? " + _gamePlayer.IsHost);
            Debug.Log("Number of active playerinfos: " + _gameInfo.PlayerInfos.Count);


            networkView.RPC("PlayerLoadedLevel", RPCMode.AllBuffered, Network.player);
            _gameInfo.Started = true;
        }

        void OnGUI()
        {
            if (!_gameStarted)
            {
                GUI.Label(new Rect(Screen.width/2.3f, Screen.height/2f, 350, 100), "Waiting for players...");
            }
        }

        [RPC]
        public void PlayerLoadedLevel(NetworkPlayer player)
        {
            _connectedPlayers++;
            Debug.Log("Connected players are now " + _connectedPlayers);
            Debug.Log("Need count of players: " + _gameInfo.PlayerInfos.Count);

            var loadedPlayer = _gameInfo.PlayerInfos.FirstOrDefault(info => info.Player == player);

            Chat.DisplayMessageStatic(String.Format("Player {0} loaded level", loadedPlayer.PlayerName));

            if (Network.isClient)
                return;

            if (_connectedPlayers == _gameInfo.PlayerInfos.Count)
            {
                Debug.Log("Starting game");
                networkView.RPC("StartGame", RPCMode.All);
                CreatePlayers();
            }
        }

        [RPC]
        private void StartGame()
        {
            _gameStarted = true;
            GameMessages.AddMessage("Game started");
        }

        private void CreatePlayers()
        {
            foreach (var playerinfo in _gameInfo.PlayerInfos)
            {
                Debug.Log(playerinfo.CharacterPicked);
                CreatePlayer(playerinfo.CharacterPicked, playerinfo.Player);
            }

            StartCoroutine(SetFlashLights());
        }

        private IEnumerator SetFlashLights()
        {
            yield return new WaitForSeconds(2);

            foreach (var playerinfo in _gameInfo.PlayerInfos)
            {
                playerinfo.PlayerControl.networkView.RPC("SetFlashLights", RPCMode.All);
            }
        }

        [RPC]
        private void CreatePlayer(string characterPick, NetworkPlayer player)
        {
            Transform characterToCreate;
            switch (characterPick.ToLower())
            {
                case "mary":
                    characterToCreate = MaryPrefab;
                    break;
                case "human":
                    characterToCreate = HumanPrefab;
                    break;
                default:
                    characterToCreate = HumanPrefab;
                    break;
                    //throw new Exception("Character type not found");
            }

            var createdPlayer = (Transform)Network.Instantiate(characterToCreate, _humanSpawnPosition, transform.rotation, 0);
            createdPlayer.networkView.RPC("SetPlayer", RPCMode.AllBuffered, player);
        }

        //handle network
        private void OnPlayerDisconnected(NetworkPlayer networkPlayer)
        {
            var player = _gameInfo.PlayerInfos.FirstOrDefault(info => info.Player == networkPlayer);

            player.PlayerControl.networkView.RPC("Die", RPCMode.All);
        }

        private void OnDisconnectedFromServer()
        {

        }
    }
}
