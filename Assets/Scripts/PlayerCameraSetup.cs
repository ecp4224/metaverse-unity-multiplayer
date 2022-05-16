using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerCameraSetup : BindableMonoBehavior
{
    private Player localPlayer;
    public Player LocalPlayer
    {
        get
        {
            return localPlayer;
        }
        set
        {
            localPlayer = value;
            SetupPlayer();
        }
    }

    public CinemachineVirtualCamera fpsCamera;
    
    public CinemachineVirtualCamera thirdCamera;

    [Inject] private XRRig vrTracker;

    private void Start()
    {

        SetupPlayer();
    }

    private void SetupVRPlayer()
    {
        /*foreach (Transform obj in gameObject.transform)
        {
            if (obj == gameObject.transform)
                continue;
                
            obj.gameObject.SetActive(false);
        }*/
        
        vrTracker.transform.SetParent(localPlayer.transform);
        vrTracker.transform.position = localPlayer.CameraRoot.transform.position;
    }

    private void SetupPlayer()
    {
        if (localPlayer != null)
        {
            if (Player.IsUsingVR())
            {
                SetupVRPlayer();
                return;
            }
            
            vrTracker.gameObject.SetActive(false);
            
            thirdCamera.Follow = localPlayer.CameraRoot;

            localPlayer.CameraChange.FirstCam = fpsCamera.gameObject;
            localPlayer.CameraChange.ThirdCam = thirdCamera.gameObject;

            foreach (Transform obj in gameObject.transform)
            {
                obj.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform obj in gameObject.transform)
            {
                if (obj == gameObject.transform)
                    continue;
                
                obj.gameObject.SetActive(false);
            }
        }
    }
}
