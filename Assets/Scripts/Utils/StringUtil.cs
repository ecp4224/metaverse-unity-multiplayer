using UnityEngine;
using System.Collections;

public static class StringUtil {
	
    public static string ToProperCase(this string str) {
        str = str.ToLower();

        str = str.Substring(0, 1).ToUpper() + str;

        return str;
    }
}
