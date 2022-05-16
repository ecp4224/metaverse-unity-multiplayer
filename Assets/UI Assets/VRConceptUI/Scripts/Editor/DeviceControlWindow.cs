using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Epibyte.ConceptVR
{
    public class DeviceControlWindow : MonoBehaviour
    {
        static string[] conceptUIPlatformDefines = new string[] {
            "VRCONCEPT_OCULUS", "VRCONCEPT_XR_INTERACTION"
        };

        [MenuItem("Window/VRConcept/Set XR Interaction Toolkit")]
        static void SetXRInteractionToolkit()
        {
            SetPlatformCustomDefine("VRCONCEPT_XR_INTERACTION");
        }

        [MenuItem("Window/VRConcept/Set Oculus Integration")]
        static void SetOculus()
        {
            SetPlatformCustomDefine("VRCONCEPT_OCULUS");
        }

        static void SetPlatformCustomDefine(string define)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Remove concept ui platform defines
            foreach (string item in conceptUIPlatformDefines)
            {
                if (defines.Contains(item))
                {
                    if (defines.Contains((";" + item)))
                    {
                        defines = defines.Replace((";" + item), "");
                    }
                    else
                    {
                        defines = defines.Replace(item, "");
                    }
                }
            }

            if (define != "" && !defines.Contains(define))
            {
                defines += ";" + define;
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }
    }
}
