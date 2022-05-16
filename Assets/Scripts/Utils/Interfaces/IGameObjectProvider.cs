using System;
using UnityEngine;

public interface IGameObjectProvider
{
    GameObject GenerateGameObject(Func<GameObject, Vector3, Quaternion, GameObject> customInstantiateFunction = null);
}