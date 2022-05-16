using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// DEPRECATED UNLESS WE WANT SOME KIND OF STATUS POPUP OR HOVER OVER BEHAVIOUR
public class ItemPickup : Interactable
{
    public override void Interact()
    {
        base.Interact();
        Pickup();
    }

    private void Pickup()
    {
        Debug.Log("Picking up Item...");
    }
}