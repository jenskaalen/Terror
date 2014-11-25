using System;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;

public class DoorBehaviour : MonoBehaviour
{
    //public Animation OpenAnimation, CloseAnimation;
    public bool IsClosed = true;
    public bool StateIsChanging;
    private Animator _animator;
    private AnimationInfo[] _animationInfos;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Close();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Open();
        }
    }

    [RPC]
    public void S_Interact_Door()
    {
        if (StateIsChanging)
            return;

        if (IsClosed)
            networkView.RPC("Open", RPCMode.All, true, 0.75f);
        else
            networkView.RPC("Close", RPCMode.All, true, 0.75f);
    }

    //[RPC]
    //public void S_Open()
    //{
    //    if (StateIsChanging)
    //        return;

    //    networkView.RPC("Open", RPCMode.All, true, 0.75f);
    //}

    //[RPC]
    //public void S_Close()
    //{
    //    if (StateIsChanging)
    //        return;

    //    networkView.RPC("Close", RPCMode.All, true, 0.75f);
    //}

    [RPC]
    public void Open()
    {
        _animator.Play("opendoor");
        StartCoroutine(ChangingState(false, 0.75f));
    }

    [RPC]
    public void Close()
    {
        _animator.Play("closedoor");
        StartCoroutine(ChangingState(true, 0.75f));
    }

    private IEnumerator ChangingState(bool isClosed, float stateChangeDuration)
    {
        StateIsChanging = true;

        yield return new WaitForSeconds(stateChangeDuration);

        IsClosed = isClosed;
        StateIsChanging = false;
    }
}
