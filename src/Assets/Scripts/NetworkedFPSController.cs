using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkView))]
    public class NetworkedFPSController : MonoBehaviour
    {
        public float MaxSpeed = 5f;

        private float _lastHMove, _lastVMove;
        private NetworkPlayer _owner;
        private bool _isOwner;
        private CharacterController _controller;

        //non-owner stuff
        public Vector3 _moveToPosition, _originPosition;
        public float _movementStart;
        public float _secondsMovedTotal;
        public float _estimatedTimeToArrive;


        private double _lastNetworkUpdate;
        //private double _lastUpdate = 0f;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _lastNetworkUpdate = Network.time;
        }

        private void Update()
        {
            //prediction is used for the owner and no interpolation used
            if (Network.isClient && !_isOwner)
            {
                HandleInterpolation();
                return;
            }

            if (Network.isServer && !_isOwner)
            {
                LocalMove(_lastHMove, _lastVMove);
            }
            else if (_isOwner)
            {
                float hMove = Input.GetAxis("Horizontal");
                float vMove = Input.GetAxis("Vertical");

                //TODO: check if these can be safely removed
                //negatory - needed for delta checking
                //_lastHMove = hMove;
                //_lastVMove = vMove;

                if (Network.isClient)
                {
                    LocalMove(hMove, vMove);
                    networkView.RPC("ServerMove", RPCMode.Server, hMove, vMove);
                }
                else
                {
                    LocalMove(hMove, vMove);    
                }

                //TODO: put movement updates in something like this clause so we wont be doing unnecessary uopdates
                //if (hMove != _lastHMove && vMove != _lastVMove || (hMove != 0 && vMove != 0))
                //{
                //}   
            }
        }

        private void LocalMove(float hMove, float vMove)
        {
            var direction = new Vector3(hMove, 0, vMove);
            Vector3 worldDirectionModifiedBySpeed = transform.TransformDirection(direction) * MaxSpeed;
            _controller.SimpleMove(worldDirectionModifiedBySpeed);
        }

        [RPC]
        private void ServerMove(float hMove, float vMove)
        {
            _lastHMove = hMove;
            _lastVMove = vMove;
            //LocalMove(hMove, vMove);
            //KISSable - rework it later
        }

        [RPC]
        public void SetOwner(NetworkPlayer player)
        {
            _owner = player;

            if (Network.player == player)
            {
                _isOwner = true;
            }
        }

        [RPC]
        private void SetMoveTarget(Vector3 target)
        {
            _moveToPosition = target;
            _originPosition = transform.position;
            //_estimatedTimeToArrive = Vector3.Distance(_originPosition, _moveToPosition);
            _estimatedTimeToArrive = Vector3.Distance(_originPosition, _moveToPosition) / MaxSpeed;
            _secondsMovedTotal = 0f;
        }

        private void HandleInterpolation()
        {
            //TODO: this
            //update position with lerp
            //if (transform.position != _moveToPosition || _secondsMovedTotal < _estimatedTimeToArrive)
            //{
            //}
            _secondsMovedTotal += Time.deltaTime;
            float movedFactor = _secondsMovedTotal / _estimatedTimeToArrive;

            if (movedFactor > 1)
                movedFactor = 1;

            transform.position = Vector3.Lerp(_originPosition, _moveToPosition, movedFactor);
        }

        public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            if (stream.isWriting && _isOwner)
            {
                var rotation = transform.rotation;
                var serverPosition = transform.position;
                stream.Serialize(ref rotation);
                stream.Serialize(ref serverPosition);
                _lastNetworkUpdate = Network.time;
            }
            else
            {
                Quaternion rotation = transform.rotation;
                var serverPosition = transform.position;
                stream.Serialize(ref rotation);
                stream.Serialize(ref serverPosition);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * 20);

                //transform.position = serverPosition;

                if (_isOwner)
                {
                    //we use prediction and only adjust the position if it needs to be recalibrated
                    float differenceFromServer = Vector3.Distance(transform.position, serverPosition);

                    //TODO: make some better attempt at prediction here
                    //... k * speed / network.sendrate. possibly?
                    if (differenceFromServer > 3f)
                    {
                        transform.position = serverPosition;
                        Debug.Log("Position difference from server too big, adjusting");
                    }
                }
                else
                {
                    SetMoveTarget(serverPosition);
                }
            }
        }
    }
}
