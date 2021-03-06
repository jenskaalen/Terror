﻿using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
//[AddComponentMenu("Camera-Control/Mouse Look")]
[RequireComponent(typeof(NetworkView))]
public class NetworkMouseLook : MonoBehaviour
{

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    //AXES CANT BE CHANGED AT RUNTIME SINCE THEY ARE NOT NETWORK UPDATED
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

    void Update()
    {
        // this might be pretty heavy
        if (Network.isClient)
            networkView.RPC("SendRotation", RPCMode.Server, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        else
            SendRotation(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    [RPC]
    public void SendRotation(float xAxis, float yAxis)
    {
        networkView.RPC("UpdatePlayers", RPCMode.All, xAxis, yAxis);
    }

    [RPC]
    public void UpdatePlayers(float xAxis, float yAxis)
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + xAxis * sensitivityX;

            rotationY += yAxis * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            //transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            var updatedEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, xAxis * sensitivityX, 0);
        }
        else
        {
            rotationY += yAxis * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }
}