using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using VRUiKits.Utils;

public class ConnectVRPanel : BindableMonoBehavior
{
    [Inject]
    private NetworkManager manager;

    public UIKitInputField ipField;
    public UIKitInputField usernameField;
    
    public void Connect()
    {
        var ip = ipField.text;

        manager.networkAddress = ip;
        
        manager.StartClient();
        
        HideUI();
    }

    public void Host()
    {
        manager.StartHost();
        
        HideUI();
    }

    public void HostServerOnly()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            throw new PlatformNotSupportedException();
        }
        
        manager.StartServer();
        
        HideUI();
    }

    private void FixedUpdate()
    {
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
            {
                NetworkClient.AddPlayer();
            }
        }
    }

    private void HideUI()
    {
        Player.LocalUsername = usernameField.text;
        
        transform.parent.gameObject.SetActive(false);
    }
}
