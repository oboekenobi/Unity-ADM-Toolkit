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

    //private PresentationSectionSceneGUI sceneGUI;

    private PresentationSection targetSection;
    private ProjectManager manager;

    public void SetTarget(PresentationSection section)
    {
        targetSection = section;
    }

    private void OnEnable()
    {
        //sceneGUI = new PresentationSectionSceneGUI();
        //SceneView.duringSceneGui += OnSceneGUI;
        //sceneGUI.SetTarget((PresentationSection)target);
        SetTarget((PresentationSection)target);
    }

    private void OnDisable()
    {
        /*if (sceneGUI != null)
        {
            DestroyImmediate(sceneGUI);
        }*/
        //SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI()
    {
        if (!Application.isPlaying)
        {
            PresentationSection camera = targetSection;

            

            if (camera == null)
            {
                return;
            }

            if (manager == null)
            {
                manager = GameObject.FindFirstObjectByType<ProjectManager>();
            }

            if (manager.SceneCameras != null)
            {
                manager.uI_Manager.PreviousPresentationSection = manager.ActiveSection;
                manager.CanSwitchEditorCamera = true;
                /*if (manager.ActiveSection != targetSection)
                {
                   
                    manager.EditorCameraSwitch(camera.sectionCamera.VirtualCamera);
                }*/
            }
            if (manager.ActiveSection != targetSection)
            {
                manager.uI_Manager.PreviousPresentationSection = manager.ActiveSection;
                manager.CanSwitchEditorCamera = true;

                manager.EditorCameraSwitch(camera.sectionCamera.VirtualCamera);
  
                manager.inputManager.SetDefaultCinemachineCamera();


                //manager.ActiveSection.director.time = manager.ActiveSection.director.duration;
                manager.ActiveSection.director.time = 0;
                manager.ActiveSection.director.RebuildGraph();
                manager.ActiveSection.director.Play();
                manager.ActiveSection.director.playableGraph.GetRootPlayable(0);

                //lock the Timeline and release it if the current selected camera is different
                TimelineState.SetLockStatus(false);
                TimelineState.SetLockStatus(true);
            }
            else
            {

            }
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        InitializeEditor();

        if (!Application.isPlaying)
        {
            m_UXML.CloneTree(root);
        }

        //creates foldout menu of the entire default way the script is displayed in the inspector, minus the already composed propertyfield
        var foldout = new Foldout() { viewDataKey = "PresentationSectionEditor" };
        InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
        //root.Add(foldout);

        return root;
    }

    private void InitializeEditor()
    {
        root = new VisualElement();
    }

    #region GizmoLabel

    public static ProjectManager StaticManager;
    public static GUIStyle centeredTextStyle = new GUIStyle();
    public PresentationSection lastSection;
    public bool RegisterLock;
    private static void GrabManager()
    {
        StaticManager = GameObject.FindFirstObjectByType<ProjectManager>();
        centeredTextStyle.alignment = TextAnchor.MiddleCenter;
        centeredTextStyle.normal.textColor = Color.white;
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
    static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
    {
        if (StaticManager == null)
        {
            GrabManager();
        }
        else
        {

            foreach(PresentationSection section in StaticManager.Sections)
            {
                if(section != null)
                {
                    Handles.Label(section.transform.position + Vector3.up * (0.8f * section.GizmoCameraScale),
                    section.transform.name, centeredTextStyle);
                }
            }
            /*for (int i = 0; i < StaticManager.Sections.Count; i++)
            {
                //Handles.color = Color.blue;
                if (StaticManager.Sections[i].VirtualCamera != null)
                {
                    Handles.Label(StaticManager.SceneCameras[i].CameraChild.transform.position + Vector3.up * (0.8f * StaticManager.ActiveSection.GizmoCameraScale),
                    StaticManager.SceneCameras[i].CameraChild.name, centeredTextStyle);
                }
            }*/
        }
    }

    #endregion



}






#endif