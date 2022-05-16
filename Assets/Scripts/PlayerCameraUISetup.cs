using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCameraUISetup : BindableMonoBehavior
{
    // just a way to grab the root camera
    [SerializeField]
    public Transform mainCamera;

    // somehow this doesn't work???
    [Inject] private PlayerPickup pp;

    public void FixPlayerPickup()
    {
       Debug.Log("Setting main camera: " + mainCamera + " : " + pp);
    }


}