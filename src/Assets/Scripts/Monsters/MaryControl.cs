using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class MaryControl : PlayerControl
    {
        public float EnergyLevel
        {
            get { return _energyLevel; }
            set {
                _energyLevel = value > MaxEnergyLevel ? MaxEnergyLevel : value;
            }
        }

        private const float MaxEnergyLevel = 100f;
        private float _frenzyEnergyRequired = 50;
        /// <summary>
        /// Gained per gamesecond
        /// </summary>
        private float _energyGainRate = 0.3f;
        private float _energyLevel;
        private int _frenzySpeed;
        private float _frenzyDuration;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();

            if (_frenzyEnergyRequired > EnergyLevel && EnergyLevel + _energyGainRate > +_frenzyEnergyRequired)
                GameMessages.AddMessage("Frenzy is now ready");

            EnergyLevel += _energyGainRate * Time.deltaTime;
        }

        protected override void HandleButtonInputs()
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
                AttemptFrenzy();
            }


            if (Input.GetKeyDown("Fire1"))
            {

                if (Network.isClient)
                    networkView.RPC("AttemptKill", RPCMode.Server);
                else
                    AttemptKill();
            }
        }

        private void AttemptFrenzy()
        {
            if (EnergyLevel < _frenzyEnergyRequired)
            {
                GameMessages.AddMessage("Not enough energy for frenzy");
            }
            else
            {
                EnergyLevel -= _frenzyEnergyRequired;
                ActivateFrenzy();
            }
        }

        private void ActivateFrenzy()
        {
            StartCoroutine(Frenzy());
        }

        [RPC]
        private void AttemptKill()
        {
            Debug.Log("Attempting kill");

            //todo: edit this to make sure we're looking at the target
            // remember the server does not necessarily own mary
            Ray ray = new Ray(MainCamera.transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "human" && hit.distance < 2f)
                {
                    var playerControl = hit.transform.GetComponent<PlayerControl>();
                    playerControl.networkView.RPC("Die", RPCMode.All);
                }
            }
        }

        [RPC]
        protected override void ServerPlayMovementAnimations(float hMove, float vMove)
        {
            if (hMove != 0 || vMove != 0)
            {
                if (vMove == 0)
                {
                    if (hMove < 0)
                        networkView.RPC("PlayAnimation", RPCMode.All, WalkHash);
                    else
                        networkView.RPC("PlayAnimation", RPCMode.All, WalkHash);
                }
                else
                    networkView.RPC("PlayAnimation", RPCMode.All, WalkHash);
            }
            else
            {
                networkView.RPC("PlayAnimation", RPCMode.All, WalkHash);
            }
        }

        private IEnumerator Frenzy()
        {
            float originalSpeed = MovementController.MaxSpeed;
            
            SetSpeed(_frenzySpeed);
            yield return new WaitForSeconds(_frenzyDuration);
            SetSpeed(originalSpeed);
        }
    }
}
