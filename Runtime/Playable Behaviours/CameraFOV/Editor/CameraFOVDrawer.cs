using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CameraFOVBehaviour))]
public class CameraFOVDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 1;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty fieldOfViewProp = property.FindPropertyRelative("fieldOfView");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, fieldOfViewProp);
    }
}
#endif