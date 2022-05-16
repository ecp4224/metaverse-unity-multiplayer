using UnityEditor;
using UnityEngine;

namespace Epibyte.ConceptVR
{
    [CustomPropertyDrawer(typeof(ScaleAxis))]
    public class ScaleAxisDrawer : PropertyDrawer
    {
        const int propertyWidth = 35;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty scaleX = property.FindPropertyRelative("scaleX");
            SerializedProperty scaleY = property.FindPropertyRelative("scaleY");
            SerializedProperty scaleZ = property.FindPropertyRelative("scaleZ");

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            scaleX.boolValue = EditorGUI.ToggleLeft(
                new Rect(position.x, position.y, propertyWidth, position.height),
                "X",
                scaleX.boolValue);
            scaleY.boolValue = EditorGUI.ToggleLeft(
                new Rect(position.x + propertyWidth * 1, position.y, propertyWidth, position.height),
                "Y",
                scaleY.boolValue);
            scaleZ.boolValue = EditorGUI.ToggleLeft(
                new Rect(position.x + propertyWidth * 2, position.y, propertyWidth, position.height),
                "Z",
                scaleZ.boolValue);
            EditorGUI.EndProperty();
        }
    }
}