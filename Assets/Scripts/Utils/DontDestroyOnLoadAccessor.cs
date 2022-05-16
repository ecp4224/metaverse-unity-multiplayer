using UnityEngine;

public class DontDestroyOnLoadAccessor : Singleton<DontDestroyOnLoadAccessor>
{
    public GameObject[] GetAllRootsOfDontDestroyOnLoad() {
        return this.gameObject.scene.GetRootGameObjects();
    }
}