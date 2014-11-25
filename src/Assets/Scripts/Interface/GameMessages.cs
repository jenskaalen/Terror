using UnityEngine;
using System.Collections;

public class GameMessages : MonoBehaviour
{
    private static MessageFlow _messageFlow =_messageFlow = new MessageFlow();

    // Use this for initialization
    void Start()
    {
        _messageFlow.MessagesShown = 6;
    }

    public static void AddMessage(string message)
    {
        Debug.Log("Added message flow");
        _messageFlow.AddMessage(message, Time.timeSinceLevelLoad);
    }

    private void OnGUI()
    {
        if (_messageFlow.ShouldDisplayMessages(Time.timeSinceLevelLoad))
        {
            GUILayout.BeginArea(new Rect(50f, Screen.height / 1.5f, Screen.width / 6f, Screen.height / 4f ));
            GUILayout.BeginVertical();

            foreach (string message in _messageFlow.VisibleMessages)
            {
                GUILayout.Label(message);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
