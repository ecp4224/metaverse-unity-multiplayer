#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class PunTypeRegister : MonoBehaviour
{
    private static PunTypeRegister _instance;

    public static PunTypeRegister Instance
    {
        get { return _instance; }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        PhotonPeer.RegisterType(typeof(Room), 33, Room.Serialize, Room.Deserialize);
        PhotonPeer.RegisterType(typeof(Biome), 34, Biome.Serialize, Biome.Deserialize);
        PhotonPeer.RegisterType(typeof(List<ValueFuture<PlayerController>>), 35, PlayerController.SerializeList,
            PlayerController.DeserializeList);
        PhotonPeer.RegisterType(typeof(PlayerController), 36, PlayerController.Serialize, PlayerController.Deserialize);
    }
}
#endif