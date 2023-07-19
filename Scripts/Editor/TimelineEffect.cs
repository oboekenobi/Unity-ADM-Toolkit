using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineEffect : EditorWindow
{
    Editor embeddedInspector;

    [MenuItem("Window/Timeline Effect Inspector")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TimelineEffect));
    }

    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TimelineEffect window = (TimelineEffect)EditorWindow.GetWindow(typeof(TimelineEffect));
    }


    private void OnGUI()
    {
        //PlayableBehaviour sel = (PlayableBehaviour)Selection.activeObject(typeof(PlayableBehaviour));
    }


    void RecycleInspector(Object target)
    {
        if (embeddedInspector != null) DestroyImmediate(embeddedInspector);
        embeddedInspector = Editor.CreateEditor(target);
    }

    PlayableBehaviour currentEffect;


    private void OnSelectionChange()
    {
        foreach (UnityEngine.Object obj in Selection.objects)
        {
            Debug.Log(obj);
            if (obj.GetType().Equals(typeof(PlayableBehaviour)))
            {
                //currentEffect = obj as PlayableBehaviour;
                break;
            }
        }
    }
}
