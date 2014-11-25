using System.Collections.Generic;
using System.Linq;

public class MessageFlow
{
    private readonly List<string> _messages = new List<string>();
    public List<string> VisibleMessages;
    public int MessagesShown = 10;
    private float _timeToShowMessages = 10;
    private float _showMessagesUntil;

    public float TimeToShowMessages
    {
        get { return _timeToShowMessages; }
        set { _timeToShowMessages = value; }
    }

    public bool ShouldDisplayMessages(float time)
    {
        return time < _showMessagesUntil;
    }

    public MessageFlow()
    {
        VisibleMessages = new List<string>();
    }

    /// <summary>
    /// Adds the message and updates how long until messages will be hidden
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="time"></param>
    public void AddMessage(string msg, float time)
    {
        _showMessagesUntil = time + _timeToShowMessages;

        _messages.Add(msg);
        int messagesToGet = _messages.Count > MessagesShown ? MessagesShown : _messages.Count;
        VisibleMessages = _messages.GetRange(_messages.Count - messagesToGet, messagesToGet).ToList();
    }
}
