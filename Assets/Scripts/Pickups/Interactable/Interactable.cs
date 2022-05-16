using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DEPRECATED NOW I THINK
public abstract class Interactable : MonoBehaviour
{
    public float radius = 3f;

    private bool _isFocus = false;
    private Transform _player;
    private bool _hasInteracted = false;
    private void Update()
    {
        // Debug.Log($"Update: focus: ${_isFocus} interacted: {_hasInteracted} player: {_player} ");
        if (_isFocus && !_hasInteracted)
        {
              _hasInteracted = true;
              Interact();
        }
    }

    public virtual void Interact()
    {
        // override in each type of interactable.
        Debug.Log($"Interacting with: {transform.name}");
    }

    public void OnFocused(Transform playerTransform)
    {
        // Debug.Log($"OnFocused Called: {playerTransform.gameObject.name} focus:{_isFocus} interacted:{_hasInteracted} ");
        _isFocus = true;
        // we only change this if its a new incoming one.
        if (_player != playerTransform)
        {
            _player = playerTransform;
            _hasInteracted = false;     
        }
        
    }

    public void OnDefocused()
    {
        // Debug.Log($"OnDefocused Called: {_player.gameObject.name} focus:{_isFocus} interacted:{_hasInteracted} ");
        _isFocus = false;
        _player = null;
        _hasInteracted = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
