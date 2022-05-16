using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private LayerMask pickableLayerMask;

    // set to public so we can fix this once the player has connected/spawned
    [SerializeField] public Transform playerCameraTransform;

    [SerializeField] private GameObject pickUpUI;

    [SerializeField] [Min(1)] private float hitRange = 3;

    [SerializeField]
    private Transform pickUpParent;

    [SerializeField]
    private GameObject inHandItem;

    [SerializeField] private InputActionReference interactionInput, dropInput, useInput;

    private RaycastHit hit;

    [SerializeField]
    private AudioSource pickUpSource;

    // public Interactable focus;
    
    private void Start()
    {
        interactionInput.action.performed += PickUp;
        dropInput.action.performed += Drop;
        useInput.action.performed += Use;
    }

    private void Use(InputAction.CallbackContext obj)
    {
        if (inHandItem != null)
        {
            var usable = inHandItem.GetComponent<IUsable>();
            if (usable != null)
            {
                usable.Use(this.gameObject);
            }
        }
    }

    private void Drop(InputAction.CallbackContext obj)
    {
        if (inHandItem != null)
        {
            inHandItem.transform.SetParent(null);
            inHandItem = null;
            // Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            // if (rb != null)
            // {
            //     rb.isKinematic = false;
            // }
        }
    }

    private void PickUp(InputAction.CallbackContext obj)
    {
        if (hit.collider != null && inHandItem == null)
        {
            var pickableItem = hit.collider.GetComponent<IPickable>();
            if (pickableItem != null)
            {
                inHandItem = pickableItem.PickUp();
                inHandItem.transform.SetParent(pickUpParent.transform, pickableItem.KeepWorldPosition);
                if(pickUpSource)
                    pickUpSource.Play();
            }
        }
    }

    private void Update()
    {

        if (!playerCameraTransform) return;
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);

        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
            pickUpUI.SetActive(false);
        }

        if (inHandItem != null)
        {
            return;
        }

        if (Physics.Raycast(
                playerCameraTransform.position,
                playerCameraTransform.forward,
                out hit,
                hitRange,
                pickableLayerMask))
        {
            // var interactable = hit.collider.GetComponent<Interactable>();
            // if (interactable != null)
            // {
            //     SetFocus(interactable);
            // }

            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            pickUpUI.SetActive(true);
        }
        // else
        // {
        //     RemoveFocus();
        // }
    }

    // private void SetFocus(Interactable newFocus)
    // {
    //     if (newFocus != focus)
    //     {
    //         if (focus != null)
    //             focus.OnDefocused();
    //         focus = newFocus;
    //     }
    //
    //     newFocus.OnFocused(transform);
    // }
    //
    // private void RemoveFocus()
    // {
    //     if (focus != null)
    //     {
    //         // Debug.Log("RemoveFocucus Called...");
    //         focus.OnDefocused();
    //     }
    //
    //     focus = null;
    // }

    public void AddHealth(int healthBoost)
    {
        Debug.Log($"Healthboost incoming: {healthBoost}");
    }
}