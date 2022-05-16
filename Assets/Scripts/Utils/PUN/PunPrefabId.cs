#if PHOTON_UNITY_NETWORKING
using Sirenix.OdinInspector;
using UnityEngine;

public class PunPrefabId : MonoBehaviour
{
    [InfoBox("This Component is used for keeping track of Prefab locations in the Resources folder. Please don't remove")]
    [ReadOnly]
    public string prefabId;
}
#endif