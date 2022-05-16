#if PHOTON_UNITY_NETWORKING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;

public static class PunEasySync
{
    private static readonly BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic 
                                                                     | BindingFlags.Instance 
                                                                     | BindingFlags.Default;
    
    private static Dictionary<Type, FieldInfo[]> cache = new Dictionary<Type, FieldInfo[]>();
    
    public static void SyncVars(this IPunObservable observable, PhotonStream stream, PhotonMessageInfo info)
    {
        var type = observable.GetType();

        if (!cache.ContainsKey(type))
            cache.Add(type, type.GetFields(FLAGS).Where(f => f.GetCustomAttribute<PunSyncAttribute>() != null).ToArray());

        var fields = cache[type];

        if (stream.IsWriting)
        {
            foreach (var field in fields)
            {
                var value = field.GetValue(observable);
                stream.SendNext(value);
            }
        }
        else
        {
            foreach (var field in fields)
            {
                var value = stream.ReceiveNext();
                field.SetValue(observable, value);
            }
        }
    }
}
#endif