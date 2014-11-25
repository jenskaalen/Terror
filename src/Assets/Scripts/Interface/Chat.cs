using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Chat : MonoBehaviour
    {
        private static MessageFlow _flow = new MessageFlow();
        public List<string> ChatHistory = new List<string>();
        private static string _currentMessage = "";
        private static float _showChatUntil = 0f;
        private const float ChatShowDuration = 6f;
        private GameInfo _gameInfo;

        private void Awake()
        {
            _gameInfo = FindObjectOfType<GameInfo>();
            //DontDestroyOnLoad(this);
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.Width(250));
            _currentMessage = GUILayout.TextField(_currentMessage);

            if (GUILayout.Button("Send"))
            {
                if (Network.isServer)
                    networkView.RPC("DisplayMessage", RPCMode.All, _currentMessage, Network.player);
                    //EnterMessage(_currentMessage);
                else
                    networkView.RPC("DisplayMessage", RPCMode.All, _currentMessage, Network.player);
                    //networkView.RPC("SendMessage", RPCMode.Server, _currentMessage);
            }

            GUILayout.EndHorizontal();

            //TODO: rememmber to palce it back in clauses
            if (Time.time < _showChatUntil)
            {
                foreach (string message in _flow.VisibleMessages)
                {
                    GUILayout.Label(message);
                }
            }
        }

        [RPC]
        private void EnterMessage(string message)
        {
            networkView.RPC("DisplayMessage", RPCMode.All, message);
        }

        [RPC]
        public void DisplayMessage(string message, NetworkPlayer player)
        {
            //ChatHistory.Add(message);
            var gameplayer = _gameInfo.PlayerInfos.FirstOrDefault(info => info.Player == player);

            string formatted = String.Format("{0}: {1}", gameplayer.PlayerName, message);
            _flow.AddMessage(formatted, Time.timeSinceLevelLoad);
            _showChatUntil = Time.time + ChatShowDuration;
        }

        [RPC]
        public static void DisplayMessageStatic(string message)
        {
            //ChatHistory.Add(message);
            _flow.AddMessage(message, Time.timeSinceLevelLoad);
            _showChatUntil = Time.time + ChatShowDuration;
        }
    }
}
