using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CalloutBinding))]
public class CalloutBindingEditor : Editor
{

    private static CalloutBinding targetBinding;
    public static GUIStyle centeredTextStyle = new GUIStyle();

    public void SetTarget()
    {
        targetBinding = (CalloutBinding)target;
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        SetTarget();
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;

    }

    void OnSceneGUI(SceneView sceneView)
    {

    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
    static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
    {
        /*SetTarget();
        centeredTextStyle.alignment = TextAnchor.MiddleCenter;
        centeredTextStyle.normal.textColor = Color.white;

        Handles.Label(targetBinding.transform.position + Vector3.up * 0.5f, targetBinding.QueryName, centeredTextStyle);*/

    }
}
