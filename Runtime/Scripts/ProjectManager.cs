using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using UnityEngine.UIElements;
using ADM.UISystem;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class ProjectManager : MonoBehaviour
{

    #region Editor Variable Checkers
    void SlideNullChecker()
    {
        
    }

    #endregion


    [SerializeField]
    public projectType ProjectType;

    [Serializable]
    public enum projectType {Static, Basic, ModelViewing}

    [HideInInspector]
    public PresentationSection ActiveSection;
    public PresentationSection LastActiveSection;
    //[HideInInspector]
    public int ActiveSectionIndex;
    //[HideInInspector]
    public List<VirtualCameraObject> SceneCameras = new List<VirtualCameraObject>();
    [Tooltip("Removes all 3D Interactivity and optimizes project to focus only on the Animation & Camera moves")]
    public bool StaticPresentation;
    public bool StaticToggleBar;

    public bool OpenMenuOnStart;
    [HideInInspector]
    public bool NullSlides;
    [HideInInspector]
    public bool CanCheckSlides;
    public bool IncludeWatermark;
    public bool CanRecordSequence;

    public string ProjectVersionName;
    public string ProjectVersionNotes;
    public Color BackgroundColor;

    public static CanvasGroup ContrastLayer;

    public UI_Manager uI_Manager;

    public ADMToolkitManager toolkitManager;
    public ControlPanel MasterControlPanel;


    public static CanvasGroup CallOutCanvas;
    public GameObject CameraFolder;

    #region Static Variables
    public static ProjectManager _instance;
    public static bool CanSwitchCamera;
    public static bool PerspCamera;
    public static bool LabelHighlited;
    public static bool previewCameraFramed;
    //public static 
    #endregion

    
    [HideInInspector]
    public CinemachineBrain Brain;
    public Transform MainCameraTransform;
    public InputManager inputManager;

    public string currentProject;

    public string DefaultGraphicsSetting;
    public string DefaultThemeSetting;
    public string DefaultScreenSetting;
    public float DefaultMouseSensitivitySetting;
    public float DefaultPenStrokeSetting;
    public bool DefaultPopupWarningSettings = true;

#if UNITY_EDITOR
    private void OnEnable()
    {
/*        if (!Application.isPlaying)
        {
            toolkitManager = GameObject.FindFirstObjectByType<ADMToolkitManager>();
            if (inputManager == null)
            {
                inputManager = GameObject.FindFirstObjectByType<InputManager>();
            }
            else if (inputManager.projectManager == null)
            {
                inputManager.projectManager = this;
            }

            if (uI_Manager == null)
            {
                uI_Manager = GameObject.FindFirstObjectByType<UI_Manager>();
            }
            else if (uI_Manager.projectManager == null)
            {
                uI_Manager.projectManager = this;
            }
        }*/
    }
#endif

    private void Awake()
    {

        if (!Application.isPlaying)
        {
            toolkitManager = GameObject.FindFirstObjectByType<ADMToolkitManager>();
            if (inputManager == null)
            {
                inputManager = GameObject.FindFirstObjectByType<InputManager>();
            }
            else if (inputManager.projectManager == null)
            {
                inputManager.projectManager = this;
            }

            if (uI_Manager == null)
            {
                uI_Manager = GameObject.FindFirstObjectByType<UI_Manager>();
            }
            else if (uI_Manager.projectManager == null)
            {
                uI_Manager.projectManager = this;
            }
        }
        if (Application.isPlaying)
        {
            Brain = GameObject.FindWithTag("Camera Manager").GetComponent<CinemachineBrain>();

            if(MasterControlPanel != null)
            {
                MasterControlPanel.GeneratedContainer = uI_Manager.GenerateControlPanel(MasterControlPanel);
            }

        }



        OrderSections();

        if(Sections.Count > 0)
        {
            ActiveSection = Sections[0];
        }
       
    }

    public void OrderSections()
    {
        for(int i = 0; i<Sections.Count; i++)
        {
            Sections[i].SectionID = i;
        }
    }

    [Space(10)]
    [Header("Section Management")]
    [Space(10)]
    public List<PresentationSection> Sections = new List<PresentationSection>();

    private bool canLoadToggles;
    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            for(int i = 0; i < SceneCameras.Count; i++)
            {
                if (SceneCameras[i] == null)
                {
                    SceneCameras.RemoveAt(i);
                }
            }

            if (Input.GetKey(KeyCode.Q))
            {

            }
        }

        SlideNullChecker();

        
        
        
#endif
       
    }
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (IncludeWatermark)
            {
                VisualElement m_waterMark = uI_Manager.uIDocument.rootVisualElement.Q<VisualElement>("Watermark");
                if(m_waterMark != null)
                {
                    m_waterMark.style.display = DisplayStyle.Flex;
                }
            }
            else
            {
                VisualElement m_waterMark = uI_Manager.uIDocument.rootVisualElement.Q<VisualElement>("Watermark");
                if (m_waterMark != null)
                {
                    m_waterMark.style.display = DisplayStyle.None;
                }
            }

            Camera.main.backgroundColor = BackgroundColor;

            if(uI_Manager != null)
            {
                uI_Manager.LoadDefaultPlayerPrefs();
            }
        }


        //BackgroundMaterial.SetColor("_Top", BackgroundColor);


        //CameraMovement.RestructureCameras(Sections);
    }

   
    #region Editor Cinemachine Camera Previewing



    public static float CallOutsValue;
    public static bool CallOutIsTransitioning;
    public bool CanSwitchEditorCamera;
   
    
    public void EditorCameraSwitch(CinemachineVirtualCamera VirtualCamera)
    {
        LastActiveSection = ActiveSection;
        if (Application.isPlaying)
        {
            VirtualCamera.Priority = 10;
            foreach (VirtualCameraObject cam in SceneCameras)
            {
                if (cam.VirtualCamera != VirtualCamera)
                {
                    cam.VirtualCamera.Priority = 0;
                }
            }
        }
        if (CanSwitchEditorCamera)
        {
            VirtualCamera.Priority = 10;
            foreach (VirtualCameraObject cam in SceneCameras)
            {
                if (cam.VirtualCamera != VirtualCamera)
                {
                    cam.VirtualCamera.Priority = 0;
                }
            }



#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (ActiveSection == null)
                {
                    ActiveSection = Sections[0];
                }
                if (ActiveSection.TweenCameras.Count > 0)
                {
                    for (int j = 0; j < ActiveSection.TweenCameras.Count; j++)
                    {
                        ActiveSection.TweenCameras[j].gameObject.SetActive(false);
                    }
                }
                if (ActiveSection.CallOutPoints.Count > 0)
                {
                    foreach (GameObject point in ActiveSection.CallOutPoints)
                    {
                        if (point != null)
                        {
                            point.SetActive(false);
                        }
                    }
                }

                for (int i = 0; i < Sections.Count; i++)
                {
                    if (Sections[i].sectionCamera.VirtualCamera == VirtualCamera)
                    {
                        if (ActiveSection != null)
                        {
                            VisualElement Container = ActiveSection.CalloutCanvasDocument.rootVisualElement.Q<VisualElement>("RootCalloutCanvas");
                            Container.RemoveFromClassList("activeCanvas");
                            Container.AddToClassList("inactiveCanvas");
                            ActiveSection = Sections[i];
                            VisualElement ActiveContainer = ActiveSection.CalloutCanvasDocument.rootVisualElement.Q<VisualElement>("RootCalloutCanvas");
                            ActiveContainer.RemoveFromClassList("inactiveCanvas");
                            ActiveContainer.AddToClassList("activeCanvas");
                            ActiveSection.sectionCamera.RayColor = ActiveSection.sectionCamera.SelectedRayColor;
                            ActiveSectionIndex = ActiveSection.SectionID;
                        }

                    }

                    if (Sections[i].sectionCamera.VirtualCamera != VirtualCamera)
                    {
                        Sections[i].sectionCamera.RayColor = Sections[i].sectionCamera.DeselectedRayColor;
                    }

                }
                if (ActiveSection != null)
                {
                    UI_Manager uI_Manager = GameObject.FindWithTag("UIManager").GetComponent<UI_Manager>();


                    if (ActiveSection.TweenCameras.Count > 0)
                    {
                        for (int i = 0; i < ActiveSection.TweenCameras.Count; i++)
                        {
                            ActiveSection.TweenCameras[i].gameObject.SetActive(true);
                        }
                    }

                    foreach (GameObject point in ActiveSection.CallOutPoints)
                    {
                        if (point != null)
                        {
                            point.SetActive(true);
                        }
                    }
                }
            }

            CanSwitchEditorCamera = false;
#endif
        }
    }

#if UNITY_EDITOR
    public static float GetGizmoSize(Vector3 position)
    {
        Camera current = Camera.current;
        position = Gizmos.matrix.MultiplyPoint(position);

        if (current)
        {
            Transform transform = current.transform;
            Vector3 position2 = transform.position;
            float z = Vector3.Dot(position - position2, transform.TransformDirection(new Vector3(0f, 0f, 1f)));
            Vector3 a = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(0f, 0f, z)));
            Vector3 b = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(1f, 0f, z)));
            float magnitude = (a - b).magnitude;
            return 80f / Mathf.Max(magnitude, 0.0001f);
        }

        return 20f;
    }
#endif



    /*public void RenderVideoSequence()
    {
        CinemahcineManager.PlayTimeline(ActiveSection);
    }*/

    #endregion

    public VisualElement MasterControlPanelContainer;

    

}




