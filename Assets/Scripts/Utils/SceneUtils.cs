using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtils
{
    public static IEnumerable<GameObject> AllGameObjects(this Scene scene)
    {
        List<GameObject> rootObjects = new List<GameObject>();
        scene.GetRootGameObjects(rootObjects);

        return rootObjects.SelectMany(obj => obj.GetComponentsInChildren<Transform>(true)).Select(t => t.gameObject).Concat(DontDestroyOnLoadAccessor.Instance.GetAllRootsOfDontDestroyOnLoad());
    }
}