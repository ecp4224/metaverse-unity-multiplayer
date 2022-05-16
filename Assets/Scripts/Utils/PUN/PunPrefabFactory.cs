#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PunPrefabFactory
{
    public static PunPrefabFactory Instance { get; private set; }
    
    [SerializeField]
    [JsonProperty]
    private Dictionary<string, string> prefabData = new Dictionary<string, string>();

    [JsonIgnore]
    public bool Dirty { get; private set; }

    public static void Delete()
    {
        if (File.Exists("Assets/Resources/prefab_data.json"))
            File.Delete("Assets/Resources/prefab_data.json");
    }
    
    public static void Load()
    {
        var jsonData = Resources.Load<TextAsset>("prefab_data");

        PunPrefabFactory factory;
        if (jsonData != null)
        {
            factory = JsonConvert.DeserializeObject<PunPrefabFactory>(jsonData.text);
        }
        else
        {
            if (Application.isEditor && File.Exists("Assets/Resources/prefab_data.json"))
            {
                var jsonText = File.ReadAllText("Assets/Resources/prefab_data.json");
                factory = JsonConvert.DeserializeObject<PunPrefabFactory>(jsonText);
            }
            else
            {
                factory = new PunPrefabFactory();
            }
        }
        
        Instance = factory;
    }

    public void Save()
    {
        if (!Dirty)
            return;
        
        if (Application.isEditor)
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText("Assets/Resources/prefab_data.json", json);
            Debug.Log("Saving with " + prefabData.Count + " items in prefabData");

            Dirty = false;
        }
        else
        {
            throw new IOException("Can't execute Save at runtime !");
        }
    }

    public void StorePrefab(GameObject prefab, string path)
    {
        var prefabId = prefab.GetComponent<PunPrefabId>();
        if (prefabId == null || string.IsNullOrWhiteSpace(prefabId.prefabId) || prefabId.prefabId == Guid.Empty.ToString())
        {
            Debug.Log("Prefab " + prefab.name + " @ " + path + " needs proper ID");
            if (prefabId == null)
            {
                Debug.Log("Prefab " + prefab.name + " @ " + path + " needs PunPrefabId Component");
                prefabId = prefab.AddComponent<PunPrefabId>();
            }

            string guid;
            do
            {
                guid = Guid.NewGuid().ToString();
            } while (prefabData.ContainsKey(guid));

            Debug.Log("Prefab " + prefab.name + " @ " + path + " has new ID " + guid);
            prefabId.prefabId = guid;
        }

        var id = prefabId.prefabId;

        if (prefabData.ContainsKey(id))
        {
            return;
        }

        var oldAmount = prefabData.Count;
        prefabData.Add(id, path);
        Dirty = oldAmount != prefabData.Count;
    }

    public string GetPrefabPath(GameObject prefab)
    {
        var prefabId = prefab.GetComponent<PunPrefabId>();
        if (prefabId == null)
        {
            return null;
        }
        
        var id = prefabId.prefabId;

        if (!prefabData.ContainsKey(id))
            return null;

        return prefabData[id];
    }
}
#endif