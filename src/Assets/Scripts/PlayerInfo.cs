using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Used by the server to keep track of connected (and disconnected) players
    /// </summary>
    public class PlayerInfo
    {
        public string PlayerName { get; set; }
        public PlayerControl PlayerControl { get; set; }
        public NetworkPlayer Player { get; set; }
        public bool IsConnected { get; set; }
        public string CharacterPicked { get; set; }
    }
}
