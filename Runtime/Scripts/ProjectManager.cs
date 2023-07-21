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
    void ClearSectionSlideData()
    {

    }

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
            CallOutCanvas = GameObject.FindWithTag("CallOut Canvas").GetComponent<CanvasGroup>();
            ContrastLayer = GameObject.FindWithTag("Contrast Layer").GetComponent<CanvasGroup>();

            if(MasterControlPanel != null)
            {
                MasterControlPanel.GeneratedContainer = uI_Manager.GenerateControlPanel(MasterControlPanel);
                uI_Manager.CurrentControlPanel = MasterControlPanel;
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
                toolkitManager.WaterMark.SetActive(true);
            }
            else
            {
                toolkitManager.WaterMark.SetActive(false);
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

    public void SwitchEditorCameraCallOuts(PresentationSection section)
    {

        if (ActiveSection.CallOutGameObjects != null)
        {
            for (int i = 0; i < ActiveSection.CallOutGameObjects.Count; i++)
            {
                section.CallOutGameObjects[i].SetActive(true);
            }
            for (int i = 0; i < Sections.Count; i++)
            {
                if (Sections[i] != ActiveSection)
                {
                    for (int j = 0; j < Sections[i].CallOutGameObjects.Count; j++)
                    {
                        Sections[i].CallOutGameObjects[j].SetActive(false);
                    }
                }
            }
            if(section.CallOutPoints.Count != 0)
            {
                for (int i = 0; i < ActiveSection.CallOutPoints.Count; i++)
                {
                    if(ActiveSection.CallOutPoints[i] != null)
                    {
                        ActiveSection.CallOutPoints[i].SetActive(true);
                    }
                }
                for (int i = 0; i < Sections.Count; i++)
                {
                    if (Sections[i] != ActiveSection)
                    {
                        for (int j = 0; j < Sections[i].CallOutPoints.Count; j++)
                        {
                            if (Sections[i].CallOutPoints[j] != null)
                            {
                                Sections[i].CallOutPoints[j].SetActive(false);
                            }
                            else
                            {
                                Sections[i].CallOutGameObjects.RemoveAt(j);                             
                            }
                        }
                    }
                }
            }
        }
        if(ActiveSection.CallOutGameObjects == null)
        {
            for (int i = 0; i < Sections.Count; i++)
            {
                if (Sections[i] != ActiveSection)
                {
                    if(Sections[i].CallOutGameObjects != null)
                    {
                        for (int j = 0; j < Sections[i].CallOutGameObjects.Count; j++)
                        {
                            Sections[i].CallOutGameObjects[j].SetActive(false);
                        }
                    }
                }
            }
            if (section.CallOutPoints.Count != 0)
            {
                for (int i = 0; i < Sections.Count; i++)
                {
                    if (Sections[i] != ActiveSection)
                    {
                        for (int j = 0; j < Sections[i].CallOutPoints.Count; j++)
                        {
                            Sections[i].CallOutPoints[j].SetActive(false);
                        }
                    }
                }
            }
        }
    }
    public void SwitchRunTimeCallOuts(PresentationSection section, bool Change)
    {
        StartCoroutine(LabelSwitch(section, Change));
        if (Change)
        {
            if (section.CallOutGameObjects.Count > 0)
            {
                for (int i = 0; i < section.CallOuts.Count; i++)
                {
                    section.CallOuts[i].OpenLabel();
                }
            }
            if (section.VisualLabels.Count > 0)
            {
                for (int i = 0; i < section.VisualLabels.Count; i++)
                {
                    section.VisualLabels[i].OpenLabel();
                }
            }
            
        }
        if (!Change)
        {
            if (section.CallOutGameObjects.Count > 0)
            {
                for (int i = 0; i < section.CallOuts.Count; i++)
                {
                    section.CallOuts[i].CloseLabel();
                }
            }
            if (section.VisualLabels.Count > 0)
            {
                for (int i = 0; i < section.VisualLabels.Count; i++)
                {
                    section.VisualLabels[i].CloseLabel();
                }
            }
        }
    }


    public static float CallOutsValue;
    public static bool CallOutIsTransitioning;
    public bool CanSwitchEditorCamera;
    public IEnumerator LabelSwitch(PresentationSection section, bool Active)
    {
        CallOutIsTransitioning = true;
        if (Active)
        {
            for (int i = 0; i < section.CallOutGameObjects.Count; i++)
            {
                
                section.CallOutGameObjects[i].SetActive(true);
            }
        }
        
        yield return new WaitForSeconds(section.CallOutSettings.settings.TransitionDuration);
        
        CallOutIsTransitioning = false;

        if (!Active)
        {
            for (int i = 0; i < section.CallOutGameObjects.Count; i++)
            {
                section.CallOutGameObjects[i].SetActive(false);
                
        
            }
        }
    }
    
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
        if(CanSwitchEditorCamera)
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
                if(ActiveSection != null)
                {
                    if (ActiveSection.TweenCameras.Count > 0)
                    {
                        for (int j = 0; j < ActiveSection.TweenCameras.Count; j++)
                        {
                            ActiveSection.TweenCameras[j].gameObject.SetActive(false);
                        }
                    }
                }

                for (int i = 0; i < Sections.Count; i++)
                {
                    if (Sections[i].Camera.VirtualCamera == VirtualCamera)
                    {
                        ActiveSection = Sections[i];
                        ActiveSection.Camera.RayColor = ActiveSection.Camera.SelectedRayColor;
                        ActiveSectionIndex = ActiveSection.SectionID;
                    }

                    if (Sections[i].Camera.VirtualCamera != VirtualCamera)
                    {
                        Sections[i].Camera.RayColor = Sections[i].Camera.DeselectedRayColor;
                    }

                }
                if (ActiveSection != null)
                {
                    UI_Manager uI_Manager = GameObject.FindWithTag("UIManager").GetComponent<UI_Manager>();


                    if(ActiveSection.TweenCameras.Count > 0)
                    {
                        for(int i =0 ; i < ActiveSection.TweenCameras.Count; i++)
                        {
                            ActiveSection.TweenCameras[i].gameObject.SetActive(true);
                        }
                    }
                }

                SwitchEditorCameraCallOuts(ActiveSection);
            }
#endif  
            CanSwitchEditorCamera = false;
        }
    }

    /*public void RenderVideoSequence()
    {
        CinemahcineManager.PlayTimeline(ActiveSection);
    }*/

    #endregion

    public VisualElement MasterControlPanelContainer;

    

}




