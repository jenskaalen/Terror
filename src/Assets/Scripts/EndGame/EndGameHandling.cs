using System;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class EndGameHandling : MonoBehaviour
{
    public static bool HumansWon, MonstersWon;
    public static NetworkView NetworkView;

    // Use this for initialization
    void Start()
    {
        NetworkView = networkView;
    }

    // Update is called once per frame
    void OnGUI()
    {
        if (HumansWon)
        {
            DisplayHumanVictory();
        }
        else if (MonstersWon)
        {
            DisplayMonsterVictory();
        }
    }

    private void DisplayHumanVictory()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Humans fuken won");
        //GUILayout.Label("Ping");
        //GUILayout.Label("Character");
        GUILayout.EndHorizontal();
    }

    private void DisplayMonsterVictory()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Monsters fuken won");
        //GUILayout.Label("Ping");
        //GUILayout.Label("Character");
        GUILayout.EndHorizontal();
    }

    public static void HumanVictory()
    {
        HumansWon = true;
        MonstersWon = false;
        DeactivateAllObjects();
    }

    public static void MonsterVictory()
    {
        //NetworkView.RPC("DeactivatePlayerObjects", RPCMode.All);
        HumansWon = false;
        MonstersWon = true;
        DeactivateAllObjects();
    }

    public static void GoBackToLobby()
    {
        var gameinfo = FindObjectOfType<GameInfo>();
        var gamemanager = FindObjectOfType<GameManager>();

        Destroy(gameinfo);
        Destroy(gamemanager);
        
        Application.LoadLevel("lobby");
    }

    private static void DeactivateAllObjects()
    {
        var gameinfo = FindObjectOfType<GameInfo>();

        foreach (var player in gameinfo.PlayerInfos)
        {
            player.PlayerControl.gameObject.SetActive(false);
        }
    }
}

