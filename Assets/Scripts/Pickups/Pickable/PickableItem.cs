using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour, IPickable
{
    [field: SerializeField]
    public bool KeepWorldPosition { get; private set; }

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
        transform.position = Vector3.zero;
        // transform.rotation = Quaternion.identity;
        return gameObject;
    }
}
