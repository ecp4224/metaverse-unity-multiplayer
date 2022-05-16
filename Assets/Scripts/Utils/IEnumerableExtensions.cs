using System;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
    public static T FirstOr<T>(this IEnumerable<T> source, T alternate)
    {
        foreach(T t in source)
            return t;
        return alternate;
    }
    
    public static T FirstOr<T>(this IEnumerable<T> source, Func<T, bool> predicate, T alternate)
    {
        foreach(T t in source)
            if (predicate(t))
                return t;
        return alternate;
    }
}