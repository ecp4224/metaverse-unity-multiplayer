using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddressDisplay : BindableMonoBehavior
{
    [BindComponentInParent]
    private Player _player;
    
    [BindComponent]
    private TextMesh _textMesh;

    private void Start()
    {
        if (_player.isLocalPlayer)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_player.PlayerAddress == null)
            return;
        
        var truncatedAddress = _player.PlayerAddress.Length >= 8 ? _player.PlayerAddress.Substring(0, 8) + "..." : _player.PlayerAddress;
        if (_textMesh.text != truncatedAddress)
        {
            _textMesh.text = _player.Username + "\n" + truncatedAddress;
        }
    }
}
