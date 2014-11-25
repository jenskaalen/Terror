using System;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public class ConnectionsBox : MonoBehaviour
{
    private GameInfo _gameInfo;

    // Use this for initialization
    void Start()
    {
        _gameInfo = FindObjectOfType<GameInfo>();
    }

    // Update is called once per frame
    void OnGUI()
    {
        //_show = Input.GetKeyDown(KeyCode.Tab);

        if (Input.GetKey(KeyCode.Tab))
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2.5f, Screen.height / 2.5f, Screen.width / 3f, Screen.height / 3f), "Connected players", GUI.skin.window);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            GUILayout.Label("Ping");
            GUILayout.Label("Character");
            GUILayout.Label("Status");

            GUILayout.EndHorizontal();

            foreach (var player in _gameInfo.PlayerInfos)
            {

                GUILayout.BeginHorizontal();
                GUILayout.Label(player.PlayerName);

                string pingVal = Network.GetAveragePing(player.Player) > 0
                    ? Network.GetAveragePing(player.Player).ToString()
                    : "--";

                GUILayout.Label(pingVal);
                GUILayout.Label(player.CharacterPicked);
                GUILayout.Label(player.IsConnected ? "Connected" : "Disconnected");

                GUILayout.EndHorizontal();
            };
            
            GUILayout.EndArea();
        }

            //GUI.Box(new Rect(50, 50, 200, 350), "Connected players" );
    }
}
