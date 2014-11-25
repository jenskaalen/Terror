using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts;
using Assets.Scripts.GameObjects;
using Assets.Scripts.Util;
using UnityEngine;
using System.Collections;

/// <summary>
/// Baseclass for all player controlled objects - humans and monsters
/// </summary>
[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(NetworkedFPSController))]
[RequireComponent(typeof(Chat))]
public class PlayerControl : MonoBehaviour
{
    public List<Key> Keys;

    public NetworkPlayer TheOwner;
    protected int WalkHash, IdleHash, StrafeLeft, StrafeRight, LastPlayedAnimation;
    private Animator _animator;
    protected bool IsOwner = false;
    public Camera MainCamera;
    protected GameInfo GameInfo;
    protected PlayerInfo PlayerInfo {
        get { return GameInfo.PlayerInfos.FirstOrDefault(info => info.Player == TheOwner); }
    }

    protected NetworkedFPSController MovementController;

    protected virtual void Awake()
    {
        Screen.lockCursor = true;
        _animator = GetComponentInChildren<Animator>();
        WalkHash = Animator.StringToHash("walking");
        IdleHash = Animator.StringToHash("idle");
        StrafeLeft = Animator.StringToHash("left_strafe_walking");
        StrafeRight = Animator.StringToHash("right_strafe_walking");
        MainCamera = GetComponentInChildren<Camera>();
        MainCamera.enabled = false;
        GameInfo = FindObjectOfType<GameInfo>();
        MovementController = FindObjectOfType<NetworkedFPSController>();
    }

    protected virtual void Update()
    {
        Screen.lockCursor = !Input.GetKey(KeyCode.Escape);

        if (IsOwner)
        {
            HandleButtonInputs();
        }
    }

    protected virtual void HandleButtonInputs()
    {
        float hMove = Input.GetAxis("Horizontal");
        float vMove = Input.GetAxis("Vertical");

        if (Network.isClient)
        {
            networkView.RPC("ServerPlayMovementAnimations", RPCMode.Server, hMove, vMove);
        }
        else
            ServerPlayMovementAnimations(hMove, vMove);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider hitCollider = UnityExtensions.ForwardRay(MainCamera.transform, 1f);

            if (hitCollider.tag == "door")
            {
                var door = hitCollider.gameObject.GetComponent<DoorBehaviour>();

                if (Network.isClient)
                    door.networkView.RPC("S_Interact_Door", RPCMode.Server);
                else
                    door.S_Interact_Door();

            }
        }
    }

    [RPC]
    protected virtual void SetPlayer(NetworkPlayer player)
    {
        TheOwner = player;

        var movement = GetComponent<NetworkedFPSController>();
        movement.SetOwner(player);

        var playerInfo = GameInfo.PlayerInfos.FirstOrDefault(info => info.Player == player);
        playerInfo.PlayerControl = this;

        if (player == Network.player)
        {
            IsOwner = true;

            if (MainCamera != null)
                MainCamera.enabled = true;
        }
        else
        {
            GetComponent<AudioListener>().enabled = false;
            GetComponent<NetworkMouseLook>().enabled = false;
            MainCamera.GetComponent<NetworkMouseLook>().enabled = false;
        }
    }

    [RPC]
    protected virtual void SetFlashLights()
    {
        var currentPlayerCharacter = GameInfo.PlayerInfos.FirstOrDefault(info => info.Player == Network.player);

        Debug.Log(String.Format("Setting flashlight for {0}", name));

        if (currentPlayerCharacter == null)
        {
            Debug.LogError(String.Format("Could not find a set player for the current object: {0}", name));
            return;
        }

        if (currentPlayerCharacter.PlayerControl == null)
        {
            Debug.LogError(String.Format("Could not find a set PlayerControl for the owner of this object: {0}", name));
            return;
        }

        foreach (var playerControl in GameInfo.PlayerInfos.Where(info => info.PlayerControl is HumanController))
        {
            var flashlight = playerControl.PlayerControl.GetComponentInChildren<LightShafts>();
            flashlight.m_Cameras = new Camera[] { currentPlayerCharacter.PlayerControl.MainCamera };
            flashlight.m_CurrentCamera = currentPlayerCharacter.PlayerControl.MainCamera;
            flashlight.Start();
        }
    }

    [RPC]
    protected virtual void ServerPlayMovementAnimations(float hMove, float vMove)
    {
        if (hMove != 0 || vMove != 0)
        {
            if (vMove == 0)
            {
                if (hMove < 0)
                    networkView.RPC("PlayAnimation", RPCMode.All, StrafeLeft);
                else
                    networkView.RPC("PlayAnimation", RPCMode.All, StrafeRight);
            }
            else
                networkView.RPC("PlayAnimation", RPCMode.All, WalkHash);
        }
        else
        {
            networkView.RPC("PlayAnimation", RPCMode.All, IdleHash);
        }
    }

    [RPC]
    public void PlayAnimation(int hash)
    {
        if (IsOwner)
            return;

        _animator.Play(hash);
    }

    [RPC]
    public void PickupKey(NetworkViewID id)
    {
        if (Keys == null)
            Keys = new List<Key>();

        var key = NetworkView.Find(id).gameObject.GetComponent<Key>();

        Keys.Add(key);
        //hide the key
        key.renderer.enabled = false;
        GameMessages.AddMessage(String.Format("{0} picked up {1} key", PlayerInfo.PlayerName, key.Name));
    }

    [RPC]
    protected virtual void Die()
    {
        gameObject.SetActive(false);
        //Destroy(this.gameObject);
        //install spectating
        GameMessages.AddMessage(String.Format("{0} died", PlayerInfo.PlayerName));
        CheckForMonsterVictory();
    }

    private void CheckForMonsterVictory()
    {
        var humanPlayers = FindObjectsOfType<HumanController>();

        if (humanPlayers.All(controller => controller.Dead))
            EndGameHandling.MonsterVictory();
    }

    protected void SetSpeed(float speed)
    {
        MovementController.MaxSpeed = speed;
    }
}
