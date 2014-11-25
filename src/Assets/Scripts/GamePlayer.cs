using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Local class which represents the playing player and will store data, like state and connection info, needed for the player to persist between scenes
    /// </summary>
    public class GamePlayer : MonoBehaviour
    {
        public PlayerState State;
        public string ServerIp;
        public int ServerPort;
        public string PlayerName;
        public string CharacterPicked;
        public bool IsHost;
    }

    public enum PlayerState
    {
        Disconnected,
        ConnectingAsClient,
        HostingLobby,
        /// <summary>
        /// Connected as a client
        /// </summary>
        Connected,
        HostingGame
    }
}
