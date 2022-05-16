using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class GameObjectUtils
{
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                             BindingFlags.Default;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos) {
            if (pinfo.CanWrite) {
                try {
                    if (pinfo.PropertyType.IsClonableType())
                    {
                        var clonable = (ICloneable)pinfo.GetValue(other, null);
                        
                        pinfo.SetValue(comp, clonable.Clone(), null);
                    }
                    else
                    {
                        if (!pinfo.PropertyType.IsClonableOrValueType())
                            Debug.LogError("Property " + pinfo.Name + " is a non-cloneable class reference. Changes to this Property will cause changes to the original Component.");
                    
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I don't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos) {
            if (finfo.FieldType.IsClonableType())
            {
                var cloneable = (ICloneable) finfo.GetValue(other);
                finfo.SetValue(comp, cloneable.Clone());
            }
            else
            {
                if (!finfo.FieldType.IsClonableOrValueType())
                    Debug.LogError("Field " + finfo.Name + " is a non-cloneable class reference. Changes to this Field will cause changes to the original Component.");
                finfo.SetValue(comp, finfo.GetValue(other));
            }
        }
        return comp as T;
    }
    
    public static T GetCopyOf<T>(this ScriptableObject comp, T other) where T : ScriptableObject
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                             BindingFlags.Default;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos) {
            if (pinfo.CanWrite) {
                try {
                    if (pinfo.PropertyType.IsClonableType())
                    {
                        var clonable = (ICloneable)pinfo.GetValue(other, null);
                        
                        pinfo.SetValue(comp, clonable == null ? null : clonable.Clone(), null);
                    }
                    else
                    {
                        if (!pinfo.PropertyType.IsClonableOrValueType())
                            Debug.LogError("Property " + pinfo.Name + " is a non-cloneable class reference. Changes to this Property will cause changes to the original Component.");
                        
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I don't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos) {
            if (finfo.FieldType.IsClonableType())
            {
                var cloneable = (ICloneable) finfo.GetValue(other);
                finfo.SetValue(comp, cloneable == null ? null : cloneable.Clone());
            }
            else
            {
                if (!finfo.FieldType.IsClonableOrValueType())
                    Debug.LogError("Field " + finfo.Name + " is a non-cloneable class reference. Changes to this Field will cause changes to the original Component.");
                
                finfo.SetValue(comp, finfo.GetValue(other));
            }
        }
        return comp as T;
    }
    
    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    public static Component AddComponent(this GameObject go, Type type, Component toAdd)
    {
        return go.AddComponent(type).GetCopyOf(toAdd);
    }

    public static ScriptableObject CloneRawInstance(ScriptableObject toClone)
    {
        return ScriptableObject.CreateInstance(toClone.GetType()).GetCopyOf(toClone);
    }

    public static T CloneInstance<T>(T toClone) where T : ScriptableObject
    {
        return ScriptableObject.CreateInstance<T>().GetCopyOf(toClone);
    }

    public static Transform GetRoot(this Transform go)
    {
        if (go.parent == null)
            return go;

        return go.parent.GetRoot();
    }
    
    public static GameObject FindObject(this GameObject parent, string name)
    {
        Transform[] trs= parent.GetComponentsInChildren<Transform>(true);
        foreach(Transform t in trs){
            if(t.name == name){
                return t.gameObject;
            }
        }
        return null;
    }

    public static string GetPath(this Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return current.parent.GetPath() + "/" + current.name;
    }

    public static bool IsClonableType(this Type type)
    {
        return typeof(ICloneable).IsAssignableFrom(type);
    }

    public static bool IsClonableOrValueType(this Type type)
    {
        return type.IsClonableType() || type.IsValueType;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var result = gameObject.GetComponent<T>();
        if (result != null)
            return result;
        return gameObject.AddComponent<T>();
    }
    
    public static List<T> Find<T>()
    {
        List<T> interfaces = new List<T>();
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach( var rootGameObject in rootGameObjects )
        {
            T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
            foreach( var childInterface in childrenInterfaces )
            {
                interfaces.Add(childInterface);
            }
        }
        return interfaces;
    }
}