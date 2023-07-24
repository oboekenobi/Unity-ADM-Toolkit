#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using ADM.UISystem;

[CustomEditor(typeof(PresentationSection))]
public class PresentationSectionEditor : Editor
{

    public static VisualElement root { get; set; }

    [SerializeField]
    public VisualTreeAsset m_UXML;

    private PresentationSectionSceneGUI sceneGUI;

    private void OnEnable()
    {
        sceneGUI = new PresentationSectionSceneGUI();
        sceneGUI.SetTarget((PresentationSection)target);
    }

    private void OnDisable()
    {
        if (sceneGUI != null)
        {
            DestroyImmediate(sceneGUI);
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        InitializeEditor();

        m_UXML.CloneTree(root);

        //creates foldout menu of the entire default way the script is displayed in the inspector, minus the already composed propertyfield
        var foldout = new Foldout() { viewDataKey = "PresentationSectionEditor" };
        InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
        root.Add(foldout);

        return root;
    }

    private void InitializeEditor()
    {
        root = new VisualElement();
    }

    #region GizmoLabel

    public static ProjectManager manager;
    public static GUIStyle centeredTextStyle = new GUIStyle();
    public PresentationSection lastSection;
    public bool RegisterLock;
    private static void GrabManager()
    {
        manager = GameObject.FindFirstObjectByType<ProjectManager>();
        centeredTextStyle.alignment = TextAnchor.MiddleCenter;
        centeredTextStyle.normal.textColor = Color.white;
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
    static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
    {
        if (manager == null)
        {
            GrabManager();
        }
        else
        {
            for (int i = 0; i < manager.SceneCameras.Count; i++)
            {
                //Handles.color = Color.blue;
                if (manager.SceneCameras[i].VirtualCamera != null)
                {
                    Handles.Label(manager.SceneCameras[i].CameraChild.transform.position + Vector3.up * 0.5f,
                    manager.SceneCameras[i].CameraChild.name, centeredTextStyle);
                }
            }
        }
        //ProjectManager manager = GameObject.FindObjectOfType<ProjectManager>();

        //Draw here
    }

    #endregion

}


#endif