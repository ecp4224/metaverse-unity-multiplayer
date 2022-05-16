using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour, IPickable
{
    [field: SerializeField] public bool KeepWorldPosition { get; private set; } = true;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public GameObject PickUp()
    {
        if (_rb != null)
        {
            _rb.isKinematic = true;
        }

        return gameObject;
    }
}