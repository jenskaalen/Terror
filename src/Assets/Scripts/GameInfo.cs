using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Used to store information about the game state both on server and clients. Will be kept (i.e. not destroyed) after creation at game lobby
    /// </summary>
    [RequireComponent(typeof(NetworkView))]
    public class GameInfo : MonoBehaviour
    {
        public List<PlayerInfo> PlayerInfos = new List<PlayerInfo>();
        public bool Started { get; set; }

        [RPC]
        public void RegisterPlayer(string playername, NetworkPlayer networkPlayer)
        {
            if (!PlayerInfos.Any(info => info.PlayerName == playername || info.Player == networkPlayer))
            {
                PlayerInfos.Add(
                    new PlayerInfo()
                    {
                        Player = networkPlayer,
                        PlayerName = playername
                    });
            }
        }
    }
}
