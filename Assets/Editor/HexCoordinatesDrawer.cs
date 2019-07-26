using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HexCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        HexCoordinates coordinates = new HexCoordinates(
            property.FindPropertyRelative("x").intValue,
            property.FindPropertyRelative("z").intValue
        );

        position = EditorGUI.PrefixLabel(position, label);  //为了渲染前缀
        GUI.Label(position, coordinates.ToString());
    }
}
