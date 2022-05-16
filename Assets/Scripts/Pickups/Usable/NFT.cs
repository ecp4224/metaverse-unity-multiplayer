using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Designates and Item is Non Fungibobble.
public class NFT : MonoBehaviour, IUsable
{
    [field: SerializeField]
    public UnityEvent OnUse { get; private set; }

    public void Use(GameObject actor)
    {
        // TODO: CALL THE NFT WALLET ON THE PLAYER?
        Debug.Log("Using NFT up....");
        OnUse?.Invoke();
        // Destroy(gameObject); // optional I guess... if TX fails don't do it?
    }
}
