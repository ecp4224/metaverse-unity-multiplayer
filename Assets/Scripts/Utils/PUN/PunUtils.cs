#if PHOTON_UNITY_NETWORKING
 using System.IO;
 using Photon.Pun;
 using UnityEngine;

 public class PunUtils
 {
     public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
     {
         if (PunPrefabFactory.Instance == null)
             PunPrefabFactory.Load();
         
         if (PunPrefabFactory.Instance == null)
             throw new FileNotFoundException("Could not load PunPrefabFactory data");

         string path = PunPrefabFactory.Instance.GetPrefabPath(prefab);
         
         if (path == null)
             throw new FileNotFoundException("Could not find prefab ID for " + prefab.name + ", is it in the Resources folder?");

         return PhotonNetwork.Instantiate(path, position, rotation, group, data);
     }

     public static GameObject InstantiateRoomObject(GameObject prefab, Vector3 position, Quaternion rotation,
         byte group = 0, object[] data = null)
     {
         if (PunPrefabFactory.Instance == null)
             PunPrefabFactory.Load();
         
         if (PunPrefabFactory.Instance == null)
             throw new FileNotFoundException("Could not load PunPrefabFactory data");

         string path = PunPrefabFactory.Instance.GetPrefabPath(prefab);
         
         if (path == null)
             throw new FileNotFoundException("Could not find prefab ID for " + prefab.name + ", is it in the Resources folder?");

         return PhotonNetwork.InstantiateRoomObject(path, position, rotation, group, data);
     }
 }
 #endif