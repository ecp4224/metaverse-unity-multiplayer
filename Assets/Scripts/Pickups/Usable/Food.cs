using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Designates this is something we can nomnom.
public class Food : MonoBehaviour, IUsable
{
    [field: SerializeField]
    public UnityEvent OnUse { get; private set; }

    public int healthBoost = 1;

    public void Use(GameObject actor)
    {
        actor.GetComponent<PlayerPickup>().AddHealth(healthBoost);
        OnUse?.Invoke();
        Destroy(gameObject);
    }
}
