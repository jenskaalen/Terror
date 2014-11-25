using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class HumanController : PlayerControl
{
    //flashlight..
    private LightShafts _lightEffect;
    private Light _flashlight;
    public bool Dead;

    private HumanController _currentSpectated;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        _lightEffect = MainCamera.GetComponentInChildren<LightShafts>();
        _flashlight = MainCamera.GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void HandleButtonInputs()
    {
        base.HandleButtonInputs();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("interact button pressed");
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "key" && hit.distance < 3f)
                {
                    //if (Network.isServer)
                    //    Interact(hit.transform.gameObject);
                    //else
                    //    networkView.RPC("Interact", RPCMode.Server, hit.transform.gameObject);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ClickFlashLight();
            _flashlight.enabled = !_flashlight.enabled;
        }

        if (Dead && Input.GetButtonDown("Fire1"))
        {
            SpectateNext();
        }
    }

    private void SpectateNext()
    {
        if (_currentSpectated == null)
            throw new NotImplementedException();

    }

    [RPC]
    public void ClickFlashLight()
    {
        networkView.RPC("ToggleFlashlight", RPCMode.All);
    }

    [RPC]
    public void ToggleFlashlight()
    {
        if (IsOwner)
            return;

        _flashlight.enabled = !_flashlight.enabled;
        _lightEffect.enabled = !_lightEffect.enabled;
    }

    [RPC]
    protected override void SetPlayer(NetworkPlayer player)
    {
        base.SetPlayer(player);

        if (IsOwner)
            _lightEffect.enabled = false;
    }

    //handle spectating
    protected void Spectate(NetworkPlayer player)
    {

        throw new NotImplementedException();
    }

    [RPC]
    protected override void Die()
    {
        base.Die();
    }
}
